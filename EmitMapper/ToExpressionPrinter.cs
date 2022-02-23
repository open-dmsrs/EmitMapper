using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using EmitMapper.Utils;

namespace EmitMapper;

public static class ToExpressionPrinter
{
  private const string NotSupportedExpression = "// NOT_SUPPORTED_EXPRESSION: ";

  /// <summary>
  ///   Prints the expression in its constructing syntax -
  ///   helpful to get the expression from the debug session and put into it the code for the test.
  /// </summary>
  public static string ToExpressionString(this Expression expr, TryPrintConstant tryPrintConstant = null)
  {
    return expr.ToExpressionString(out var _, out var _, out var _, tryPrintConstant: tryPrintConstant);
  }

  /// <summary>
  ///   Prints the expression in its constructing syntax -
  ///   helpful to get the expression from the debug session and put into it the code for the test.
  ///   In addition, returns the gathered expressions, parameters ad labels.
  /// </summary>
  public static string ToExpressionString(this Expression expr,
    out List<ParameterExpression> paramsExprs, out List<Expression> uniqueExprs, out List<LabelTarget> lts,
    bool stripNamespace = false, Func<Type, string, string> printType = null, int identSpaces = 2,
    TryPrintConstant tryPrintConstant = null)
  {
    var sb = new StringBuilder(1024);
    sb.Append("var expr = ");
    paramsExprs = new List<ParameterExpression>();
    uniqueExprs = new List<Expression>();
    lts = new List<LabelTarget>();

    sb = expr.CreateExpressionString(
      sb,
      paramsExprs,
      uniqueExprs,
      lts,
      2,
      stripNamespace,
      printType,
      identSpaces,
      tryPrintConstant).Append(';');

    if (lts.Count > 0)
      sb.Insert(0, $"var l = new LabelTarget[{lts.Count}]; // the labels{Environment.NewLine}");

    if (uniqueExprs.Count > 0)
      sb.Insert(0, $"var e = new Expression[{uniqueExprs.Count}]; // the unique expressions{Environment.NewLine}");

    if (paramsExprs.Count > 0)
      sb.Insert(
        0,
        $"var p = new ParameterExpression[{paramsExprs.Count}]; // the parameter expressions{Environment.NewLine}");

    return sb.ToString();
  }

  internal static StringBuilder CreateExpressionString(this Expression e, StringBuilder sb,
    List<ParameterExpression> paramsExprs, List<Expression> uniqueExprs, List<LabelTarget> lts,
    int lineIdent = 0, bool stripNamespace = false, Func<Type, string, string> printType = null, int identSpaces = 2,
    TryPrintConstant tryPrintConstant = null)
  {
    switch (e.NodeType)
    {
      case ExpressionType.Constant:
      {
        var x = (ConstantExpression)e;
        sb.Append("Constant(");

        if (tryPrintConstant != null)
        {
          var s = tryPrintConstant(x);

          if (s != null)
            return sb.Append(s).Append(')');
        }

        if (x.Value == null)
        {
          sb.Append("null");

          if (x.Type != Metadata<object>.Type)
            sb.Append(", ").AppendTypeof(x.Type, stripNamespace, printType);
        }
        else if (x.Value is Type t)
        {
          sb.AppendTypeof(t, stripNamespace, printType);
        }
        else
        {
          sb.Append(x.Value.ToCode(CodePrinter.DefaultConstantValueToCode, stripNamespace, printType));

          if (x.Value.GetType() != x.Type)
            sb.Append(", ").AppendTypeof(x.Type, stripNamespace, printType);
        }

        return sb.Append(')');
      }
      case ExpressionType.Parameter:
      {
        var x = (ParameterExpression)e;
        sb.Append("Parameter(").AppendTypeof(x.Type, stripNamespace, printType);

        if (x.IsByRef)
          sb.Append(".MakeByRefType()");

        if (x.Name != null)
          sb.Append(", \"").Append(x.Name).Append('"');

        return sb.Append(')');
      }
      case ExpressionType.New:
      {
        var x = (NewExpression)e;
        var args = x.Arguments;

        if (args.Count == 0 && e.Type.IsValueType)
          return sb.Append("New(").AppendTypeof(e.Type, stripNamespace, printType).Append(')');

        sb.Append("New( // ").Append(args.Count).Append(" args");

        var ctorIndex = x.Constructor.DeclaringType.GetTypeInfo().DeclaredConstructors.ToArray()
          .GetFirstIndex(x.Constructor);

        sb.NewLineIdent(lineIdent).AppendTypeof(x.Type, stripNamespace, printType)
          .Append(".GetTypeInfo().DeclaredConstructors.ToArray()[").Append(ctorIndex).Append("],");

        sb.NewLineIdentArgumentExprs(
          args,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant);

        return sb.Append(')');
      }
      case ExpressionType.Call:
      {
        var x = (MethodCallExpression)e;
        sb.Append("Call(");

        sb.NewLineIdentExpr(
          x.Object,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant).Append(", ");

        sb.NewLineIdent(lineIdent).AppendMethod(x.Method, stripNamespace, printType);

        if (x.Arguments.Count > 0)
          sb.Append(',').NewLineIdentArgumentExprs(
            x.Arguments,
            paramsExprs,
            uniqueExprs,
            lts,
            lineIdent,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant);

        return sb.Append(')');
      }
      case ExpressionType.MemberAccess:
      {
        var x = (MemberExpression)e;

        if (x.Member is PropertyInfo p)
        {
          sb.Append("Property(");

          sb.NewLineIdentExpr(
            x.Expression,
            paramsExprs,
            uniqueExprs,
            lts,
            lineIdent,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant).Append(',');

          sb.NewLineIdent(lineIdent).AppendProperty(p, stripNamespace, printType);
        }
        else
        {
          sb.Append("Field(");

          sb.NewLineIdentExpr(
            x.Expression,
            paramsExprs,
            uniqueExprs,
            lts,
            lineIdent,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant).Append(',');

          sb.NewLineIdent(lineIdent).AppendField((FieldInfo)x.Member, stripNamespace, printType);
        }

        return sb.Append(')');
      }

      case ExpressionType.NewArrayBounds:
      case ExpressionType.NewArrayInit:
      {
        var x = (NewArrayExpression)e;

        if (e.NodeType == ExpressionType.NewArrayInit)
        {
          // todo: @feature multi-dimensional array initializers are not supported yet, they also are not supported by the hoisted expression
          if (e.Type.GetArrayRank() > 1)
            sb.NewLineIdent(lineIdent).Append(NotSupportedExpression).Append(e.NodeType).NewLineIdent(lineIdent);

          sb.Append("NewArrayInit(");
        }
        else
        {
          sb.Append("NewArrayBounds(");
        }

        sb.NewLineIdent(lineIdent).AppendTypeof(x.Type.GetElementType(), stripNamespace, printType).Append(", ");

        sb.NewLineIdentArgumentExprs(
          x.Expressions,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant);

        return sb.Append(')');
      }
      case ExpressionType.MemberInit:
      {
        var x = (MemberInitExpression)e;
        sb.Append("MemberInit((NewExpression)(");

        sb.NewLineIdentExpr(
            x.NewExpression,
            paramsExprs,
            uniqueExprs,
            lts,
            lineIdent,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant)
          .Append(')');

        for (var i = 0; i < x.Bindings.Count; i++)
          x.Bindings[i].ToExpressionString(
            sb.Append(", ").NewLineIdent(lineIdent),
            paramsExprs,
            uniqueExprs,
            lts,
            lineIdent + identSpaces,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant);

        return sb.Append(')');
      }
      case ExpressionType.Lambda:
      {
        var x = (LambdaExpression)e;

        sb.Append("Lambda<").Append(x.Type.ToCode(stripNamespace, printType))
          .Append(">( //$"); // bookmark for the lambdas - $ means the cost of the lambda, specifically nested lambda

        sb.NewLineIdentExpr(
          x.Body,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant).Append(',');

        sb.NewLineIdentArgumentExprs(
          x.Parameters,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant);

        return sb.Append(')');
      }
      case ExpressionType.Invoke:
      {
        var x = (InvocationExpression)e;
        sb.Append("Invoke(");

        sb.NewLineIdentExpr(
          x.Expression,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant).Append(',');

        sb.NewLineIdentArgumentExprs(
          x.Arguments,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant);

        return sb.Append(")");
      }
      case ExpressionType.Conditional:
      {
        var x = (ConditionalExpression)e;
        sb.Append("Condition(");

        sb.NewLineIdentExpr(
          x.Test,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant).Append(',');

        sb.NewLineIdentExpr(
          x.IfTrue,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant).Append(',');

        sb.NewLineIdentExpr(
          x.IfFalse,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant).Append(',');

        sb.NewLineIdent(lineIdent).AppendTypeof(x.Type, stripNamespace, printType);

        return sb.Append(')');
      }
      case ExpressionType.Block:
      {
        var x = (BlockExpression)e;
        sb.Append("Block(");
        sb.NewLineIdent(lineIdent).AppendTypeof(x.Type, stripNamespace, printType).Append(',');

        if (x.Variables.Count == 0)
        {
          sb.NewLineIdent(lineIdent).Append("new ParameterExpression[0], ");
        }
        else
        {
          sb.NewLineIdent(lineIdent).Append("new[] {");

          for (var i = 0; i < x.Variables.Count; i++)
            x.Variables[i].ToExpressionString(
              (i > 0 ? sb.Append(',') : sb).NewLineIdent(lineIdent),
              paramsExprs,
              uniqueExprs,
              lts,
              lineIdent + identSpaces,
              stripNamespace,
              printType,
              identSpaces,
              tryPrintConstant);

          sb.NewLineIdent(lineIdent).Append("},");
        }

        sb.NewLineIdentArgumentExprs(
          x.Expressions,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant);

        return sb.Append(')');
      }
      case ExpressionType.Loop:
      {
        var x = (LoopExpression)e;
        sb.Append("Loop(");

        sb.NewLineIdentExpr(
          x.Body,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant);

        if (x.BreakLabel != null)
          x.BreakLabel.ToExpressionString(
            sb.Append(',').NewLineIdent(lineIdent),
            lts,
            lineIdent,
            stripNamespace,
            printType);

        if (x.ContinueLabel != null)
          x.ContinueLabel.ToExpressionString(
            sb.Append(',').NewLineIdent(lineIdent),
            lts,
            lineIdent,
            stripNamespace,
            printType);

        return sb.Append(')');
      }
      case ExpressionType.Index:
      {
        var x = (IndexExpression)e;
        sb.Append(x.Indexer != null ? "MakeIndex(" : "ArrayAccess(");

        sb.NewLineIdentExpr(
          x.Object,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant).Append(", ");

        if (x.Indexer != null)
          sb.NewLineIdent(lineIdent).AppendProperty(x.Indexer, stripNamespace, printType).Append(", ");

        sb.Append("new Expression[] {");

        for (var i = 0; i < x.Arguments.Count; i++)
          (i > 0 ? sb.Append(',') : sb)
            .NewLineIdentExpr(
              x.Arguments[i],
              paramsExprs,
              uniqueExprs,
              lts,
              lineIdent,
              stripNamespace,
              printType,
              identSpaces,
              tryPrintConstant);

        return sb.Append("})");
      }
      case ExpressionType.Try:
      {
        var x = (TryExpression)e;

        if (x.Finally == null)
        {
          sb.Append("TryCatch(");

          sb.NewLineIdentExpr(
            x.Body,
            paramsExprs,
            uniqueExprs,
            lts,
            lineIdent,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant).Append(',');

          x.Handlers.ToExpressionString(
            sb,
            paramsExprs,
            uniqueExprs,
            lts,
            lineIdent,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant);
        }
        else if (x.Handlers == null)
        {
          sb.Append("TryFinally(");

          sb.NewLineIdentExpr(
            x.Body,
            paramsExprs,
            uniqueExprs,
            lts,
            lineIdent,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant).Append(',');

          sb.NewLineIdentExpr(
            x.Finally,
            paramsExprs,
            uniqueExprs,
            lts,
            lineIdent,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant);
        }
        else
        {
          sb.Append("TryCatchFinally(");

          sb.NewLineIdentExpr(
            x.Body,
            paramsExprs,
            uniqueExprs,
            lts,
            lineIdent,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant).Append(',');

          sb.NewLineIdentExpr(
            x.Finally,
            paramsExprs,
            uniqueExprs,
            lts,
            lineIdent,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant).Append(',');

          x.Handlers.ToExpressionString(
            sb,
            paramsExprs,
            uniqueExprs,
            lts,
            lineIdent,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant);
        }

        return sb.Append(')');
      }
      case ExpressionType.Label:
      {
        var x = (LabelExpression)e;
        sb.Append("Label(");
        x.Target.ToExpressionString(sb, lts, lineIdent, stripNamespace, printType);

        if (x.DefaultValue != null)
          sb.Append(',').NewLineIdentExpr(
            x.DefaultValue,
            paramsExprs,
            uniqueExprs,
            lts,
            lineIdent,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant);

        return sb.Append(')');
      }
      case ExpressionType.Goto:
      {
        var x = (GotoExpression)e;
        sb.Append("MakeGoto(").AppendEnum(x.Kind, stripNamespace, printType).Append(',');

        sb.NewLineIdent(lineIdent);
        x.Target.ToExpressionString(sb, lts, lineIdent, stripNamespace, printType).Append(',');

        sb.NewLineIdentExpr(
          x.Value,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant).Append(',');

        sb.NewLineIdent(lineIdent).AppendTypeof(x.Type, stripNamespace, printType);

        return sb.Append(')');
      }
      case ExpressionType.Switch:
      {
        var x = (SwitchExpression)e;
        sb.Append("Switch(");

        sb.NewLineIdentExpr(
          x.SwitchValue,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant).Append(',');

        sb.NewLineIdentExpr(
          x.DefaultBody,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant).Append(',');

        sb.NewLineIdent(lineIdent).AppendMethod(x.Comparison, stripNamespace, printType);

        ToExpressionString(
          x.Cases,
          sb,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant);

        return sb.Append(')');
      }
      case ExpressionType.Default:
      {
        return e.Type == Metadata.Void
          ? sb.Append("Empty()")
          : sb.Append("Default(").AppendTypeof(e.Type, stripNamespace, printType).Append(')');
      }
      case ExpressionType.TypeIs:
      case ExpressionType.TypeEqual:
      {
        var x = (TypeBinaryExpression)e;
        sb.Append(e.NodeType == ExpressionType.TypeIs ? "TypeIs(" : "TypeEqual(");

        sb.NewLineIdentExpr(
          x.Expression,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant).Append(',');

        sb.NewLineIdent(lineIdent).AppendTypeof(x.TypeOperand, stripNamespace, printType);

        return sb.Append(')');
      }
      case ExpressionType.Coalesce:
      {
        var x = (BinaryExpression)e;
        sb.Append("Coalesce(");

        sb.NewLineIdentExpr(
          x.Left,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant).Append(',');

        sb.NewLineIdentExpr(
          x.Right,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant);

        if (x.Conversion != null)
          sb.Append(',').NewLineIdentExpr(
            x.Conversion,
            paramsExprs,
            uniqueExprs,
            lts,
            lineIdent,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant);

        return sb.Append(')');
      }
      case ExpressionType.ListInit:
      {
        var x = (ListInitExpression)e;
        sb.Append("ListInit((NewExpression)(");

        sb.NewLineIdentExpr(
          x.NewExpression,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant).Append(')');

        for (var i = 0; i < x.Initializers.Count; i++)
          x.Initializers[i].ToExpressionString(
            sb.Append(", ").NewLineIdent(lineIdent),
            paramsExprs,
            uniqueExprs,
            lts,
            lineIdent,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant);

        return sb.Append(")");
      }
      case ExpressionType.Extension:
      {
        var reduced = e.Reduce(); // proceed with the reduced expression

        return reduced.CreateExpressionString(
          sb,
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant);
      }
      case ExpressionType.Dynamic:
      case ExpressionType.RuntimeVariables:
      case ExpressionType.DebugInfo:
      case ExpressionType.Quote:
      {
        return sb.NewLineIdent(lineIdent).Append(NotSupportedExpression).Append(e.NodeType).NewLineIdent(lineIdent);
      }
      default:
      {
        var name = Enum.GetName(Metadata<ExpressionType>.Type, e.NodeType);

        if (e is UnaryExpression u)
        {
          sb.Append(name).Append('(');

          sb.NewLineIdentExpr(
            u.Operand,
            paramsExprs,
            uniqueExprs,
            lts,
            lineIdent,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant);

          if (e.NodeType == ExpressionType.Convert ||
              e.NodeType == ExpressionType.ConvertChecked ||
              e.NodeType == ExpressionType.Unbox ||
              e.NodeType == ExpressionType.Throw ||
              e.NodeType == ExpressionType.TypeAs)
            sb.Append(',').NewLineIdent(lineIdent).AppendTypeof(e.Type, stripNamespace, printType);

          if ((e.NodeType == ExpressionType.Convert || e.NodeType == ExpressionType.ConvertChecked)
              && u.Method != null)
            sb.Append(',').NewLineIdent(lineIdent).AppendMethod(u.Method, stripNamespace, printType);
        }

        if (e is BinaryExpression b)
        {
          sb.Append("MakeBinary(").Append(Metadata<ExpressionType>.Type.Name).Append('.').Append(name).Append(',');

          sb.NewLineIdentExpr(
            b.Left,
            paramsExprs,
            uniqueExprs,
            lts,
            lineIdent,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant).Append(',');

          sb.NewLineIdentExpr(
            b.Right,
            paramsExprs,
            uniqueExprs,
            lts,
            lineIdent,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant);

          if (b.IsLiftedToNull || b.Method != null)
          {
            sb.Append(',').NewLineIdent(lineIdent).Append("liftToNull: ").Append(b.IsLiftedToNull.ToCode());
            sb.Append(',').NewLineIdent(lineIdent).AppendMethod(b.Method, stripNamespace, printType);

            if (b.Conversion != null)
              sb.Append(',').NewLineIdentExpr(
                b.Conversion,
                paramsExprs,
                uniqueExprs,
                lts,
                lineIdent,
                stripNamespace,
                printType,
                identSpaces,
                tryPrintConstant);
          }

          if (b.Conversion != null)
            sb.Append(',').NewLineIdentExpr(
              b.Conversion,
              paramsExprs,
              uniqueExprs,
              lts,
              lineIdent,
              stripNamespace,
              printType,
              identSpaces,
              tryPrintConstant);
        }

        return sb.Append(')');
      }
    }
  }

  // Searches first for the expression reference in the `uniqueExprs` and adds the reference to expression by index, 
  // otherwise delegates to `CreateExpressionCodeString`
  internal static StringBuilder ToExpressionString(this Expression expr, StringBuilder sb,
    List<ParameterExpression> paramsExprs, List<Expression> uniqueExprs, List<LabelTarget> lts,
    int lineIdent, bool stripNamespace, Func<Type, string, string> printType, int identSpaces,
    TryPrintConstant tryPrintConstant)
  {
    if (expr is ParameterExpression p)
      return p.ToExpressionString(
        sb,
        paramsExprs,
        uniqueExprs,
        lts,
        lineIdent,
        stripNamespace,
        printType,
        identSpaces,
        tryPrintConstant);

    var i = uniqueExprs.Count - 1;
    while (i != -1 && !ReferenceEquals(uniqueExprs[i], expr)) --i;

    if (i != -1)
      return sb.Append("e[").Append(i)
        // output expression type and kind to help to understand what is it
        .Append(" // ").Append(expr.NodeType.ToString()).Append(" of ")
        .Append(expr.Type.ToCode(stripNamespace, printType))
        .NewLineIdent(lineIdent).Append("]");

    uniqueExprs.Add(expr);
    sb.Append("e[").Append(uniqueExprs.Count - 1).Append("]=");

    return expr.CreateExpressionString(
      sb,
      paramsExprs,
      uniqueExprs,
      lts,
      lineIdent,
      stripNamespace,
      printType,
      identSpaces,
      tryPrintConstant);
  }

  internal static StringBuilder ToExpressionString(this ParameterExpression pe, StringBuilder sb,
    List<ParameterExpression> paramsExprs, List<Expression> uniqueExprs, List<LabelTarget> lts,
    int lineIdent, bool stripNamespace, Func<Type, string, string> printType, int identSpaces,
    TryPrintConstant tryPrintConstant)
  {
    var i = paramsExprs.Count - 1;
    while (i != -1 && !ReferenceEquals(paramsExprs[i], pe)) --i;

    if (i != -1)
      return sb.Append("p[").Append(i)
        .Append(" // (")
        .Append(!pe.Type.IsPrimitive && pe.Type.IsValueType ? "[struct] " : string.Empty)
        .Append(pe.Type.ToCode(stripNamespace, printType))
        .Append(' ').AppendName(pe.Name, pe.Type, pe).Append(')')
        .NewLineIdent(lineIdent).Append(']');

    paramsExprs.Add(pe);
    sb.Append("p[").Append(paramsExprs.Count - 1).Append("]=");

    return pe.CreateExpressionString(
      sb,
      paramsExprs,
      uniqueExprs,
      lts,
      lineIdent,
      stripNamespace,
      printType,
      identSpaces,
      tryPrintConstant);
  }

  internal static StringBuilder ToExpressionString(this LabelTarget lt, StringBuilder sb,
    List<LabelTarget> labelTargets,
    int lineIdent, bool stripNamespace, Func<Type, string, string> printType)
  {
    var i = labelTargets.Count - 1;
    while (i != -1 && !ReferenceEquals(labelTargets[i], lt)) --i;

    if (i != -1)
      return sb.Append("l[").Append(i)
        .Append(" // (").AppendName(lt.Name, lt.Type, lt).Append(')')
        .NewLineIdent(lineIdent).Append(']');

    labelTargets.Add(lt);
    sb.Append("l[").Append(labelTargets.Count - 1).Append("]=Label(");
    sb.AppendTypeof(lt.Type, stripNamespace, printType);

    return (lt.Name != null ? sb.Append(", \"").Append(lt.Name).Append("\"") : sb).Append(")");
  }

  private static StringBuilder ToExpressionString(this IReadOnlyList<CatchBlock> bs, StringBuilder sb,
    List<ParameterExpression> paramsExprs, List<Expression> uniqueExprs, List<LabelTarget> lts,
    int lineIdent, bool stripNamespace, Func<Type, string, string> printType, int identSpaces,
    TryPrintConstant tryPrintConstant)
  {
    if (bs.Count == 0)
      return sb.Append("new CatchBlock[0]");

    for (var i = 0; i < bs.Count; i++)
      bs[i].ToExpressionString(
        (i > 0 ? sb.Append(',') : sb).NewLineIdent(lineIdent),
        paramsExprs,
        uniqueExprs,
        lts,
        lineIdent + identSpaces,
        stripNamespace,
        printType,
        identSpaces,
        tryPrintConstant);

    return sb;
  }

  private static StringBuilder ToExpressionString(this CatchBlock b, StringBuilder sb,
    List<ParameterExpression> paramsExprs, List<Expression> uniqueExprs, List<LabelTarget> lts,
    int lineIdent, bool stripNamespace, Func<Type, string, string> printType, int identSpaces,
    TryPrintConstant tryPrintConstant)
  {
    sb.Append("MakeCatchBlock(");
    sb.NewLineIdent(lineIdent).AppendTypeof(b.Test, stripNamespace, printType).Append(',');

    sb.NewLineIdentExpr(
      b.Variable,
      paramsExprs,
      uniqueExprs,
      lts,
      lineIdent,
      stripNamespace,
      printType,
      identSpaces,
      tryPrintConstant).Append(',');

    sb.NewLineIdentExpr(
      b.Body,
      paramsExprs,
      uniqueExprs,
      lts,
      lineIdent,
      stripNamespace,
      printType,
      identSpaces,
      tryPrintConstant).Append(',');

    sb.NewLineIdentExpr(
      b.Filter,
      paramsExprs,
      uniqueExprs,
      lts,
      lineIdent,
      stripNamespace,
      printType,
      identSpaces,
      tryPrintConstant);

    return sb.Append(')');
  }

  private static StringBuilder ToExpressionString(this IReadOnlyList<SwitchCase> items, StringBuilder sb,
    List<ParameterExpression> paramsExprs, List<Expression> uniqueExprs, List<LabelTarget> lts,
    int lineIdent, bool stripNamespace, Func<Type, string, string> printType, int identSpaces,
    TryPrintConstant tryPrintConstant)
  {
    if (items.Count == 0)
      return sb.Append("new SwitchCase[0]");

    for (var i = 0; i < items.Count; i++)
      items[i].ToExpressionString(
        (i > 0 ? sb.Append(',') : sb).NewLineIdent(lineIdent),
        paramsExprs,
        uniqueExprs,
        lts,
        lineIdent,
        stripNamespace,
        printType,
        identSpaces,
        tryPrintConstant);

    return sb;
  }

  private static StringBuilder ToExpressionString(this SwitchCase s, StringBuilder sb,
    List<ParameterExpression> paramsExprs, List<Expression> uniqueExprs, List<LabelTarget> lts,
    int lineIdent, bool stripNamespace, Func<Type, string, string> printType, int identSpaces,
    TryPrintConstant tryPrintConstant)
  {
    sb.Append("SwitchCase(");

    sb.NewLineIdentExpr(
      s.Body,
      paramsExprs,
      uniqueExprs,
      lts,
      lineIdent,
      stripNamespace,
      printType,
      identSpaces,
      tryPrintConstant).Append(',');

    sb.NewLineIdentArgumentExprs(
      s.TestValues,
      paramsExprs,
      uniqueExprs,
      lts,
      lineIdent,
      stripNamespace,
      printType,
      identSpaces,
      tryPrintConstant);

    return sb.Append(')');
  }

  private static StringBuilder ToExpressionString(this MemberBinding mb, StringBuilder sb,
    List<ParameterExpression> paramsExprs, List<Expression> uniqueExprs, List<LabelTarget> lts,
    int lineIdent, bool stripNamespace, Func<Type, string, string> printType, int identSpaces,
    TryPrintConstant tryPrintConstant)
  {
    if (mb is MemberAssignment ma)
    {
      sb.Append("Bind(");
      sb.NewLineIdent(lineIdent).AppendMember(mb.Member, stripNamespace, printType).Append(", ");

      sb.NewLineIdentExpr(
        ma.Expression,
        paramsExprs,
        uniqueExprs,
        lts,
        lineIdent,
        stripNamespace,
        printType,
        identSpaces,
        tryPrintConstant);

      return sb.Append(")");
    }

    if (mb is MemberMemberBinding mmb)
    {
      sb.NewLineIdent(lineIdent).Append(NotSupportedExpression).Append(nameof(MemberMemberBinding))
        .NewLineIdent(lineIdent);

      sb.Append("MemberBind(");
      sb.NewLineIdent(lineIdent).AppendMember(mb.Member, stripNamespace, printType);

      for (var i = 0; i < mmb.Bindings.Count; i++)
        mmb.Bindings[i].ToExpressionString(
          sb.Append(", ").NewLineIdent(lineIdent),
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant);

      return sb.Append(")");
    }

    if (mb is MemberListBinding mlb)
    {
      sb.NewLineIdent(lineIdent).Append(NotSupportedExpression).Append(nameof(MemberListBinding))
        .NewLineIdent(lineIdent);

      sb.Append("ListBind(");
      sb.NewLineIdent(lineIdent).AppendMember(mb.Member, stripNamespace, printType);

      for (var i = 0; i < mlb.Initializers.Count; i++)
        mlb.Initializers[i].ToExpressionString(
          sb.Append(", ").NewLineIdent(lineIdent),
          paramsExprs,
          uniqueExprs,
          lts,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant);

      return sb.Append(")");
    }

    return sb;
  }

  private static StringBuilder ToExpressionString(this ElementInit ei, StringBuilder sb,
    List<ParameterExpression> paramsExprs, List<Expression> uniqueExprs, List<LabelTarget> lts,
    int lineIdent, bool stripNamespace, Func<Type, string, string> printType, int identSpaces,
    TryPrintConstant tryPrintConstant)
  {
    sb.Append("ElementInit(");
    sb.NewLineIdent(lineIdent).AppendMethod(ei.AddMethod, stripNamespace, printType).Append(", ");

    sb.NewLineIdentArgumentExprs(
      ei.Arguments,
      paramsExprs,
      uniqueExprs,
      lts,
      lineIdent,
      stripNamespace,
      printType,
      identSpaces,
      tryPrintConstant);

    return sb.Append(")");
  }
}