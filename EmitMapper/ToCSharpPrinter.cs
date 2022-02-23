using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using EmitMapper.Utils;

namespace EmitMapper;

/// <summary>Converts the expression into the valid C# code representation</summary>
public static class ToCSharpPrinter
{
  private const string NotSupportedExpression = "// NOT_SUPPORTED_EXPRESSION: ";

  /// <summary>Tries hard to convert the expression into the correct C# code</summary>
  public static string ToCSharpString(this Expression expr)
  {
    return expr.ToCSharpString(new StringBuilder(1024), 4, true).Append(';').ToString();
  }

  /// <summary>Tries hard to convert the expression into the correct C# code</summary>
  public static string ToCSharpString(this Expression expr, TryPrintConstant tryPrintConstant)
  {
    return expr.ToCSharpString(new StringBuilder(1024), 4, true, tryPrintConstant: tryPrintConstant).Append(';')
      .ToString();
  }

  /// <summary>Tries hard to convert the expression into the correct C# code</summary>
  public static StringBuilder ToCSharpString(this Expression e, StringBuilder sb,
    int lineIdent = 0, bool stripNamespace = false, Func<Type, string, string> printType = null, int identSpaces = 4,
    TryPrintConstant tryPrintConstant = null)
  {
    switch (e.NodeType)
    {
      case ExpressionType.Constant:
      {
        var x = (ConstantExpression)e;

        if (tryPrintConstant != null)
        {
          var s = tryPrintConstant(x);

          if (s != null)
            return sb.Append(s);
        }

        if (x.Value == null)
          return sb.Append("null");

        if (x.Value is Type t)
          return sb.AppendTypeof(t, stripNamespace, printType);

        if (x.Value.GetType() != x.Type) // add the cast
          sb.Append('(').Append(x.Type.ToCode(stripNamespace, printType)).Append(')');

        // value output may also add the cast for the primitive values
        return sb.Append(x.Value.ToCode(CodePrinter.DefaultConstantValueToCode, stripNamespace, printType));
      }
      case ExpressionType.Parameter:
      {
        return sb.AppendName(((ParameterExpression)e).Name, e.Type, e);
      }
      case ExpressionType.New:
      {
        var x = (NewExpression)e;
        sb.Append("new ").Append(e.Type.ToCode(stripNamespace, printType)).Append('(');
        var args = x.Arguments;

        if (args.Count == 1)
          args[0].ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
        else if (args.Count > 1)
          for (var i = 0; i < args.Count; i++)
          {
            (i > 0 ? sb.Append(',') : sb).NewLineIdent(lineIdent);

            args[i].ToCSharpString(
              sb,
              lineIdent + identSpaces,
              stripNamespace,
              printType,
              identSpaces,
              tryPrintConstant);
          }

        return sb.Append(')');
      }
      case ExpressionType.Call:
      {
        var x = (MethodCallExpression)e;

        if (x.Object != null)
          x.Object.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
        else // for the static method or the static extension method we need to qualify with the class
          sb.Append(x.Method.DeclaringType.ToCode(stripNamespace, printType));

        var name = x.Method.Name;

        // check for the special methods, e.g. property access `get_` or `set_` and output them as properties
        if (x.Method.IsSpecialName)
          if (name.StartsWith("get_") || name.StartsWith("set_"))
            return sb.Append('.').Append(name.Substring(4));

        sb.Append('.').Append(name);

        if (x.Method.IsGenericMethod)
        {
          sb.Append('<');
          var typeArgs = x.Method.GetGenericArguments();

          for (var i = 0; i < typeArgs.Length; i++)
            (i == 0 ? sb : sb.Append(", ")).Append(typeArgs[i].ToCode(stripNamespace, printType));

          sb.Append('>');
        }

        sb.Append('(');
        var pars = x.Method.GetParameters();
        var args = x.Arguments;

        if (args.Count == 1)
        {
          var p = pars[0];

          if (p.ParameterType.IsByRef)
            sb.Append(p.IsOut ? "out " : p.IsIn ? "in" : "ref ");

          args[0].ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
        }
        else if (args.Count > 1)
        {
          for (var i = 0; i < args.Count; i++)
          {
            (i == 0 ? sb : sb.Append(',')).NewLineIdent(lineIdent);
            var p = pars[i];

            if (p.ParameterType.IsByRef)
              sb.Append(p.IsOut ? "out " : p.IsIn ? "in " : "ref ");

            args[i].ToCSharpString(
              sb,
              lineIdent + identSpaces,
              stripNamespace,
              printType,
              identSpaces,
              tryPrintConstant);
          }
        }

        return sb.Append(')');
      }
      case ExpressionType.MemberAccess:
      {
        var x = (MemberExpression)e;

        if (x.Expression != null)
          x.Expression.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
        else
          sb.NewLineIdent(lineIdent).Append(x.Member.DeclaringType.ToCode(stripNamespace, printType));

        return sb.Append('.').Append(x.Member.GetCSharpName());
      }
      case ExpressionType.NewArrayBounds:
      case ExpressionType.NewArrayInit:
      {
        var x = (NewArrayExpression)e;
        sb.Append("new ").Append(e.Type.GetElementType().ToCode(stripNamespace, printType));
        sb.Append(e.NodeType == ExpressionType.NewArrayInit ? "[] {" : "[");

        var exprs = x.Expressions;

        if (exprs.Count == 1)
          exprs[0].ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
        else if (exprs.Count > 1)
          for (var i = 0; i < exprs.Count; i++)
            exprs[i].ToCSharpString(
              (i > 0 ? sb.Append(',') : sb).NewLineIdent(lineIdent),
              lineIdent + identSpaces,
              stripNamespace,
              printType,
              identSpaces,
              tryPrintConstant);

        return sb.Append(e.NodeType == ExpressionType.NewArrayInit ? "}" : "]");
      }
      case ExpressionType.MemberInit:
      {
        var x = (MemberInitExpression)e;
        x.NewExpression.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
        sb.NewLine(lineIdent, identSpaces).Append('{');
        x.Bindings.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);

        return sb.NewLine(lineIdent, identSpaces).Append('}');
      }
      case ExpressionType.ListInit:
      {
        var x = (ListInitExpression)e;
        x.NewExpression.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
        sb.NewLine(lineIdent, identSpaces).Append('{');

        var inits = x.Initializers;

        for (var i = 0; i < inits.Count; ++i)
        {
          (i == 0 ? sb : sb.Append(", ")).NewLineIdent(lineIdent);
          var elemInit = inits[i];
          var args = elemInit.Arguments;

          if (args.Count == 1)
          {
            args.GetArgument(0).ToCSharpString(
              sb,
              lineIdent + identSpaces,
              stripNamespace,
              printType,
              identSpaces,
              tryPrintConstant);
          }
          else
          {
            sb.Append('{');

            for (var j = 0; j < args.Count; ++j)
              args.GetArgument(j).ToCSharpString(
                j == 0 ? sb : sb.Append(", "),
                lineIdent + identSpaces,
                stripNamespace,
                printType,
                identSpaces,
                tryPrintConstant);

            sb.Append('}');
          }
        }

        return sb.NewLine(lineIdent, identSpaces).Append("};");
      }
      case ExpressionType.Lambda:
      {
        var x = (LambdaExpression)e;
        // The result should be something like this (taken from the #237)
        //
        // `(DeserializerDlg<Word>)((ref ReadOnlySequence<Byte> input, Word value, out Int64 bytesRead) => {...})`
        // 
        sb.Append('(').Append(e.Type.ToCode(stripNamespace, printType)).Append(")((");
        var count = x.Parameters.Count;

        if (count > 0)
        {
          var pars = x.Type.FindDelegateInvokeMethod().GetParameters();

          for (var i = 0; i < count; i++)
          {
            if (i > 0)
              sb.Append(", ");

            if (count > 1)
              sb.NewLineIdent(lineIdent);

            var pe = x.Parameters[i];
            var p = pars[i];

            if (pe.IsByRef)
              sb.Append(p.IsOut ? "out " : p.IsIn ? "in " : "ref ");

            sb.Append(pe.Type.ToCode(stripNamespace, printType)).Append(' ');
            sb.AppendName(pe.Name, pe.Type, pe);
          }
        }

        sb.Append(") => //$");
        var body = x.Body;
        var bNodeType = body.NodeType;

        var isBodyExpression = bNodeType != ExpressionType.Block && bNodeType != ExpressionType.Try &&
                               bNodeType != ExpressionType.Loop;

        if (isBodyExpression && x.ReturnType != Metadata.Void)
        {
          sb.NewLineIdentCs(body, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
        }
        else
        {
          sb.NewLine(lineIdent, identSpaces).Append('{');

          // Body handles ident and `;` itself
          if (body is BlockExpression bb)
          {
            bb.BlockToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant, true);
          }
          else
          {
            sb.NewLineIdentCs(body, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);

            if (isBodyExpression)
              sb.AddSemicolonIfFits();
          }

          sb.NewLine(lineIdent, identSpaces).Append('}');
        }

        return sb.Append(')');
      }
      case ExpressionType.Invoke:
      {
        var x = (InvocationExpression)e;
        sb.Append("new ").Append(x.Expression.Type.ToCode(stripNamespace, printType)).Append("(");
        sb.NewLineIdentCs(x.Expression, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
        sb.Append(").Invoke(");

        for (var i = 0; i < x.Arguments.Count; i++)
          (i > 0 ? sb.Append(',') : sb)
            .NewLineIdentCs(x.Arguments[i], lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);

        return sb.Append(")");
      }
      case ExpressionType.Conditional:
      {
        var x = (ConditionalExpression)e;

        if (e.Type == Metadata.Void) // otherwise output as ternary expression
        {
          sb.NewLine(lineIdent, identSpaces);
          sb.Append("if (");
          x.Test.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
          sb.Append(')');
          sb.NewLine(lineIdent, identSpaces).Append('{');

          if (x.IfTrue is BlockExpression tb)
            tb.BlockToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant, false);
          else
            sb.NewLineIdentCs(x.IfTrue, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant)
              .AddSemicolonIfFits();

          sb.NewLine(lineIdent, identSpaces).Append('}');

          if (x.IfFalse.NodeType != ExpressionType.Default || x.IfFalse.Type != Metadata.Void)
          {
            sb.NewLine(lineIdent, identSpaces).Append("else");
            sb.NewLine(lineIdent, identSpaces).Append('{');

            if (x.IfFalse is BlockExpression bl)
              bl.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
            else
              sb.NewLineIdentCs(x.IfFalse, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant)
                .Append(';');

            sb.NewLine(lineIdent, identSpaces).Append('}');
          }
        }
        else
        {
          x.Test.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant).Append(" ?");
          sb.NewLineIdentCs(x.IfTrue, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant).Append(" :");
          sb.NewLineIdentCs(x.IfFalse, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
        }

        return sb;
      }
      case ExpressionType.Block:
      {
        return BlockToCSharpString(
          (BlockExpression)e,
          sb,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant);
      }
      case ExpressionType.Loop:
      {
        var x = (LoopExpression)e;
        sb.NewLine(lineIdent, identSpaces).Append("while (true)");
        sb.NewLine(lineIdent, identSpaces).Append("{");

        if (x.ContinueLabel != null)
        {
          sb.NewLine(lineIdent, identSpaces);
          x.ContinueLabel.ToCSharpString(sb).Append(": ");
        }

        x.Body.ToCSharpString(sb, lineIdent + identSpaces, stripNamespace, printType, identSpaces, tryPrintConstant);

        sb.NewLine(lineIdent, identSpaces).Append("}");

        if (x.BreakLabel != null)
        {
          sb.NewLine(lineIdent, identSpaces);
          x.BreakLabel.ToCSharpString(sb).Append(": ");
        }

        return sb;
      }
      case ExpressionType.Index:
      {
        var x = (IndexExpression)e;
        x.Object.ToCSharpString(sb, lineIdent + identSpaces, stripNamespace, printType, identSpaces, tryPrintConstant);

        var isStandardIndexer = x.Indexer == null || x.Indexer.Name == "Item";

        if (isStandardIndexer)
          sb.Append('[');
        else
          sb.Append('.').Append(x.Indexer.Name).Append('(');

        for (var i = 0; i < x.Arguments.Count; i++)
          x.Arguments[i].ToCSharpString(
            i > 0 ? sb.Append(", ") : sb,
            lineIdent + identSpaces,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant);

        return sb.Append(isStandardIndexer ? ']' : ')');
      }
      case ExpressionType.Try:
      {
        var x = (TryExpression)e;
        var returnsValue = e.Type != Metadata.Void;

        void PrintPart(Expression part)
        {
          if (part is BlockExpression pb)
          {
            pb.BlockToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant, true);
          }
          else
          {
            sb.NewLineIdent(lineIdent);

            if (returnsValue && CanBeReturned(part.NodeType))
              sb.Append("return ");

            part.ToCSharpString(sb, lineIdent + identSpaces, stripNamespace, printType, identSpaces, tryPrintConstant)
              .AddSemicolonIfFits();
          }
        }

        sb.Append("try");
        sb.NewLine(lineIdent, identSpaces).Append('{');
        PrintPart(x.Body);
        sb.NewLine(lineIdent, identSpaces).Append('}');

        var handlers = x.Handlers;

        if (handlers != null && handlers.Count > 0)
          for (var i = 0; i < handlers.Count; i++)
          {
            var h = handlers[i];
            sb.NewLine(lineIdent, identSpaces).Append("catch (");
            var exTypeName = h.Test.ToCode(stripNamespace, printType);
            sb.Append(exTypeName);

            if (h.Variable != null)
              sb.Append(' ').AppendName(h.Variable.Name, h.Variable.Type, h.Variable);

            sb.Append(')');

            if (h.Filter != null)
            {
              sb.Append("when (");
              sb.NewLineIdentCs(h.Filter, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
              sb.NewLine(lineIdent, identSpaces).Append(')');
            }

            sb.NewLine(lineIdent, identSpaces).Append('{');
            PrintPart(h.Body);
            sb.NewLine(lineIdent, identSpaces).Append('}');
          }

        if (x.Finally != null)
        {
          sb.NewLine(lineIdent, identSpaces).Append("finally");
          sb.NewLine(lineIdent, identSpaces).Append('{');
          PrintPart(x.Finally);
          sb.NewLine(lineIdent, identSpaces).Append('}');
        }

        return sb;
      }
      case ExpressionType.Label:
      {
        var x = (LabelExpression)e;
        sb.NewLineIdent(lineIdent);
        x.Target.ToCSharpString(sb).Append(':');

        return
          sb; // we don't output the default value and relying on the Goto Return `return` instead, otherwise we may change the logic of the code
      }
      case ExpressionType.Goto:
      {
        var gt = (GotoExpression)e;

        if (gt.Kind == GotoExpressionKind.Return || gt.Value != null)
        {
          var gtValue = gt.Value;

          if (gtValue == null)
            return sb.Append("return;");

          if (CanBeReturned(gtValue.NodeType))
            sb.Append("return ");

          gtValue.ToCSharpString(sb, lineIdent - identSpaces, stripNamespace, printType, identSpaces, tryPrintConstant);

          return sb;
        }

        return gt.Target.ToCSharpString(sb.Append("goto "));
      }
      case ExpressionType.Switch:
      {
        var x = (SwitchExpression)e;
        sb.Append("switch (");

        x.SwitchValue.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant)
          .Append(')');

        sb.NewLine(lineIdent, identSpaces).Append('{');

        foreach (var cs in x.Cases)
        {
          foreach (var tv in cs.TestValues)
          {
            sb.NewLineIdent(lineIdent).Append("case ");
            tv.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant).Append(':');
          }

          sb.NewLineIdent(lineIdent + identSpaces);

          cs.Body.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant)
            .AddSemicolonIfFits();
        }

        if (x.DefaultBody != null)
        {
          sb.NewLineIdent(lineIdent).Append("default:").NewLineIdent(lineIdent + identSpaces);

          x.DefaultBody.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant)
            .AddSemicolonIfFits();
        }

        return sb.NewLine(lineIdent, identSpaces).Append("}");
      }
      case ExpressionType.Default:
      {
        return e.Type == Metadata.Void
          ? sb // `default(void)` does not make sense in the C#
          : sb.Append("default(").Append(e.Type.ToCode(stripNamespace, printType)).Append(')');
      }
      case ExpressionType.TypeIs:
      case ExpressionType.TypeEqual:
      {
        var x = (TypeBinaryExpression)e;
        sb.Append('(');
        x.Expression.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
        sb.Append(" is ").Append(x.TypeOperand.ToCode(stripNamespace, printType));

        return sb.Append(')');
      }
      case ExpressionType.Coalesce:
      {
        var x = (BinaryExpression)e;
        x.Left.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces);
        sb.Append(" ?? ").NewLineIdent(lineIdent);

        return x.Right.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
      }
      case ExpressionType.Extension:
      {
        var reduced = e.Reduce(); // proceed with the reduced expression

        return reduced.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
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
          var op = u.Operand;

          switch (e.NodeType)
          {
            case ExpressionType.ArrayLength:
              return op.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant)
                .Append(".Length");

            case ExpressionType.Not: // either the bool not or the binary not
              return op.ToCSharpString(
                e.Type == Metadata<bool>.Type ? sb.Append("!(") : sb.Append("~("),
                lineIdent,
                stripNamespace,
                printType,
                identSpaces,
                tryPrintConstant).Append(')');

            case ExpressionType.Convert:
            case ExpressionType.ConvertChecked:
              sb.Append("((").Append(e.Type.ToCode(stripNamespace, printType)).Append(')');

              return op.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant)
                .Append(')');

            case ExpressionType.Decrement:
              return op.ToCSharpString(
                sb.Append('('),
                lineIdent,
                stripNamespace,
                printType,
                identSpaces,
                tryPrintConstant).Append(" - 1)");

            case ExpressionType.Increment:
              return op.ToCSharpString(
                sb.Append('('),
                lineIdent,
                stripNamespace,
                printType,
                identSpaces,
                tryPrintConstant).Append(" + 1)");

            case ExpressionType.Negate:
            case ExpressionType.NegateChecked:
              return op.ToCSharpString(
                sb.Append("(-"),
                lineIdent,
                stripNamespace,
                printType,
                identSpaces,
                tryPrintConstant).Append(')');

            case ExpressionType.PostIncrementAssign:
              return op.ToCSharpString(
                sb.Append('('),
                lineIdent,
                stripNamespace,
                printType,
                identSpaces,
                tryPrintConstant).Append("++)");

            case ExpressionType.PreIncrementAssign:
              return op.ToCSharpString(
                sb.Append("(++"),
                lineIdent,
                stripNamespace,
                printType,
                identSpaces,
                tryPrintConstant).Append(')');

            case ExpressionType.PostDecrementAssign:
              return op.ToCSharpString(
                sb.Append('('),
                lineIdent,
                stripNamespace,
                printType,
                identSpaces,
                tryPrintConstant).Append("--)");

            case ExpressionType.PreDecrementAssign:
              return op.ToCSharpString(
                sb.Append("(--"),
                lineIdent,
                stripNamespace,
                printType,
                identSpaces,
                tryPrintConstant).Append(')');

            case ExpressionType.IsTrue:
              return op.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant)
                .Append("==true");

            case ExpressionType.IsFalse:
              return op.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant)
                .Append("==false");

            case ExpressionType.TypeAs:
              op.ToCSharpString(sb.Append('('), lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);

              return sb.Append(" as ").Append(e.Type.ToCode(stripNamespace, printType)).Append(')');

            case ExpressionType.TypeIs:
              op.ToCSharpString(sb.Append('('), lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);

              return sb.Append(" is ").Append(e.Type.ToCode(stripNamespace, printType)).Append(')');

            case ExpressionType.Throw:
              sb.Append("throw ");

              return op.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant)
                .Append(';');

            case ExpressionType.Unbox: // output it as the cast 
              sb.Append("((").Append(e.Type.ToCode(stripNamespace, printType)).Append(')');

              return op.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant)
                .Append(')');

            default:
              return sb.Append(e); // falling back ro ToString as a closest to C# code output 
          }
        }

        if (e is BinaryExpression b)
        {
          var nodeType = e.NodeType;

          if (nodeType == ExpressionType.ArrayIndex)
          {
            b.Left.ToCSharpString(sb.Append('('), lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant)
              .Append(')');

            return b.Right.ToCSharpString(
              sb.Append("["),
              lineIdent,
              stripNamespace,
              printType,
              identSpaces,
              tryPrintConstant).Append("]");
          }

          if (nodeType == ExpressionType.Assign ||
              nodeType == ExpressionType.PowerAssign ||
              nodeType == ExpressionType.AndAssign ||
              nodeType == ExpressionType.OrAssign ||
              nodeType == ExpressionType.AddAssign ||
              nodeType == ExpressionType.ExclusiveOrAssign ||
              nodeType == ExpressionType.AddAssignChecked ||
              nodeType == ExpressionType.SubtractAssign ||
              nodeType == ExpressionType.SubtractAssignChecked ||
              nodeType == ExpressionType.MultiplyAssign ||
              nodeType == ExpressionType.MultiplyAssignChecked ||
              nodeType == ExpressionType.DivideAssign ||
              nodeType == ExpressionType.LeftShiftAssign ||
              nodeType == ExpressionType.RightShiftAssign ||
              nodeType == ExpressionType.ModuloAssign
             )
          {
            // todo: @perf handle the right part is condition with the blocks for If and/or Else, e.g. see #261 test `Serialize_the_nullable_struct_array` 
            if (b.Right is BlockExpression rightBlock) // it is valid to assign the block and it is used to my surprise
            {
              sb.Append("// { The block result will be assigned to `")
                .Append(
                  b.Left.ToCSharpString(
                    new StringBuilder(),
                    lineIdent,
                    stripNamespace,
                    printType,
                    identSpaces,
                    tryPrintConstant))
                .Append('`');

              rightBlock.BlockToCSharpString(
                sb,
                lineIdent,
                stripNamespace,
                printType,
                identSpaces,
                tryPrintConstant,
                false,
                b);

              return sb.NewLineIdent(lineIdent).Append("// } end of block assignment");
            }

            b.Left.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);

            if (nodeType == ExpressionType.PowerAssign)
            {
              sb.Append(" = System.Math.Pow(");

              b.Left.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant)
                .Append(", ");

              return b.Right.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant)
                .Append(")");
            }

            sb.Append(OperatorToCSharpString(nodeType));

            return b.Right.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
          }

          b.Left.ToCSharpString(sb.Append('('), lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);

          if (nodeType == ExpressionType.Equal)
          {
            if (b.Right is ConstantExpression r && r.Value is bool rb && rb)
              return sb;

            sb.Append(" == ");
          }
          else if (nodeType == ExpressionType.NotEqual)
          {
            if (b.Right is ConstantExpression r && r.Value is bool rb)
              return rb ? sb.Append(" == false") : sb;

            sb.Append(" != ");
          }
          else
          {
            sb.Append(OperatorToCSharpString(nodeType));
          }

          return b.Right.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant)
            .Append(')');
        }

        return sb.Append(e); // falling back ToString and hoping for the best 
      }
    }
  }

  internal static StringBuilder ToCSharpString(this LabelTarget target, StringBuilder sb)
  {
    return sb.AppendName(target.Name, target.Type, target);
  }

  private static StringBuilder AddSemicolonIfFits(this StringBuilder sb)
  {
    var lastChar = sb[sb.Length - 1];

    if (lastChar != ';')
      return sb.Append(";");

    return sb;
  }

  private static StringBuilder BlockToCSharpString(this BlockExpression b, StringBuilder sb,
    int lineIdent = 0, bool stripNamespace = false, Func<Type, string, string> printType = null, int identSpaces = 4,
    TryPrintConstant tryPrintConstant = null, bool inTheLastBlock = false,
    BinaryExpression blockResultAssignment = null)
  {
    var vars = b.Variables;

    if (vars.Count != 0)
      for (var i = 0; i < vars.Count; i++)
      {
        var v = vars[i];
        sb.NewLineIdent(lineIdent);
        sb.Append(v.Type.ToCode(stripNamespace, printType)).Append(' ');
        sb.AppendName(v.Name, v.Type, v).Append(';');
      }

    var exprs = b.Expressions;

    // we don't inline as single expression case because it can always go crazy with assignment, e.g. `var a; a = 1 + (a = 2) + a * 2`

    for (var i = 0; i < exprs.Count - 1; i++)
    {
      var expr = exprs[i];

      // this is basically the return pattern (see #237) so we don't care for the rest of the expressions
      // Note (#300) the sentence above is slightly wrong because that may be a goto to this specific label, so we still need to print the label
      if (expr is GotoExpression gt && gt.Kind == GotoExpressionKind.Return &&
          exprs[i + 1] is LabelExpression label && label.Target == gt.Target)
      {
        sb.NewLineIdent(lineIdent);

        if (gt.Value == null)
          sb.Append("return;");
        else
          gt.Value.ToCSharpString(
            sb.Append("return "),
            lineIdent,
            stripNamespace,
            printType,
            identSpaces,
            tryPrintConstant).AddSemicolonIfFits();

        sb.NewLineIdent(lineIdent);
        label.Target.ToCSharpString(sb).Append(':');

        if (label.DefaultValue == null)
          return sb.AppendLine(); // no return because we may have other expressions after label

        sb.NewLineIdent(lineIdent);

        return label.DefaultValue.ToCSharpString(
          sb.Append("return "),
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant).AddSemicolonIfFits();
      }

      if (expr is BlockExpression bl)
      {
        // Unrolling the block on the same vertical line
        bl.BlockToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant, false);
      }
      else
      {
        sb.NewLineIdent(lineIdent);

        if (expr is LabelExpression) // keep the label on the same vertical line
          expr.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
        else
          expr.ToCSharpString(sb, lineIdent + identSpaces, stripNamespace, printType, identSpaces, tryPrintConstant);

        // Preventing the `};` kind of situation and separating the conditional block with empty line
        if (expr is BlockExpression ||
            expr is ConditionalExpression ||
            expr is TryExpression ||
            expr is LoopExpression ||
            expr is SwitchExpression)
          sb.NewLineIdent(lineIdent);
        else if (!(
                   expr is LabelExpression ||
                   expr is DefaultExpression))
          sb.AddSemicolonIfFits();
      }
    }

    var lastExpr = exprs[exprs.Count - 1];

    if (lastExpr.NodeType == ExpressionType.Default && lastExpr.Type == Metadata.Void)
      return sb;

    if (lastExpr is BlockExpression lastBlock)
      return lastBlock.BlockToCSharpString(
        sb,
        lineIdent,
        stripNamespace,
        printType,
        identSpaces,
        tryPrintConstant,
        inTheLastBlock, // the last block is marked so if only it is itself in the last block
        blockResultAssignment);

    // todo: @wip if the label is already used by the Return GoTo we should skip it output here OR we need to replace the Return Goto `return` with `goto`  
    if (lastExpr is LabelExpression) // keep the last label on the same vertical line
    {
      lastExpr.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);

      if (inTheLastBlock)
        sb.AddSemicolonIfFits(); // the last label forms the invalid C#, so we need at least ';' at the end

      return sb;
    }

    sb.NewLineIdent(lineIdent);

    if (blockResultAssignment != null)
    {
      blockResultAssignment.Left.ToCSharpString(
        sb,
        lineIdent,
        stripNamespace,
        printType,
        identSpaces,
        tryPrintConstant);

      if (blockResultAssignment.NodeType != ExpressionType.PowerAssign)
      {
        sb.Append(OperatorToCSharpString(blockResultAssignment.NodeType));
      }
      else
      {
        sb.Append(" = System.Math.Pow(");

        blockResultAssignment.Left.ToCSharpString(
          sb,
          lineIdent,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant).Append(", ");
      }
    }
    else if (inTheLastBlock && b.Type != Metadata.Void)
    {
      sb.Append("return ");
    }

    if (lastExpr is ConditionalExpression ||
        lastExpr is TryExpression ||
        lastExpr is LoopExpression ||
        lastExpr is SwitchExpression ||
        lastExpr is DefaultExpression d && d.Type == Metadata.Void)
    {
      lastExpr.ToCSharpString(sb, lineIdent + identSpaces, stripNamespace, printType, identSpaces, tryPrintConstant);
    }
    else if (lastExpr.NodeType == ExpressionType.Assign && ((BinaryExpression)lastExpr).Right is BlockExpression)
    {
      lastExpr.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
    }
    else
    {
      lastExpr.ToCSharpString(sb, lineIdent + identSpaces, stripNamespace, printType, identSpaces, tryPrintConstant);

      if (blockResultAssignment?.NodeType == ExpressionType.PowerAssign)
        sb.Append(')');

      sb.AddSemicolonIfFits();
    }

    return sb;
  }

  private static bool CanBeReturned(ExpressionType nt)
  {
    return nt != ExpressionType.Goto &&
           nt != ExpressionType.Throw &&
           nt != ExpressionType.Block &&
           nt != ExpressionType.Try &&
           nt != ExpressionType.Loop;
  }

  private static string GetCSharpName(this MemberInfo m)
  {
    var name = m.Name;

    if (m is FieldInfo fi && m.DeclaringType.IsValueType)
      // btw, `fi.IsSpecialName` returns `false` :/
      if (name[0] == '<') // a backing field for the properties in struct, e.g. <Key>k__BackingField
      {
        var end = name.IndexOf('>');

        if (end > 1)
          name = name.Substring(1, end - 1);
      }

    return name;
  }

  private static string OperatorToCSharpString(ExpressionType nodeType)
  {
    return nodeType switch
    {
      ExpressionType.And => " & ",
      ExpressionType.AndAssign => " &= ",
      ExpressionType.AndAlso => " && ",
      ExpressionType.Or => " | ",
      ExpressionType.OrAssign => " |= ",
      ExpressionType.OrElse => " || ",
      ExpressionType.GreaterThan => " > ",
      ExpressionType.GreaterThanOrEqual => " >= ",
      ExpressionType.LessThan => " < ",
      ExpressionType.LessThanOrEqual => " <= ",
      ExpressionType.Equal => " == ",
      ExpressionType.NotEqual => " != ",
      ExpressionType.Add => " + ",
      ExpressionType.AddChecked => " + ",
      ExpressionType.AddAssign => " += ",
      ExpressionType.AddAssignChecked => " += ",
      ExpressionType.Subtract => " - ",
      ExpressionType.SubtractChecked => " - ",
      ExpressionType.SubtractAssign => " -= ",
      ExpressionType.SubtractAssignChecked => " -= ",
      ExpressionType.Assign => " = ",
      ExpressionType.ExclusiveOr => " ^ ",
      ExpressionType.ExclusiveOrAssign => " ^= ",
      ExpressionType.LeftShift => " << ",
      ExpressionType.LeftShiftAssign => " <<= ",
      ExpressionType.RightShift => " >> ",
      ExpressionType.RightShiftAssign => " >>= ",
      ExpressionType.Modulo => " % ",
      ExpressionType.ModuloAssign => " %= ",
      ExpressionType.Multiply => " * ",
      ExpressionType.MultiplyChecked => " * ",
      ExpressionType.MultiplyAssign => " *= ",
      ExpressionType.MultiplyAssignChecked => " *= ",
      ExpressionType.Divide => " / ",
      ExpressionType.DivideAssign => " /= ",
      _ => "???" // todo: @unclear wanna be good
    };
  }

  private static StringBuilder ToCSharpString(this IReadOnlyList<MemberBinding> bindings, StringBuilder sb,
    int lineIdent = 0, bool stripNamespace = false, Func<Type, string, string> printType = null, int identSpaces = 4,
    TryPrintConstant tryPrintConstant = null)
  {
    foreach (var b in bindings)
    {
      sb.NewLineIdent(lineIdent);
      sb.Append(b.Member.Name).Append(" = ");

      if (b is MemberAssignment ma)
      {
        ma.Expression.ToCSharpString(sb, lineIdent, stripNamespace, printType, identSpaces, tryPrintConstant);
      }
      else if (b is MemberMemberBinding mmb)
      {
        sb.Append("{");

        ToCSharpString(
          mmb.Bindings,
          sb,
          lineIdent + identSpaces,
          stripNamespace,
          printType,
          identSpaces,
          tryPrintConstant);

        sb.NewLineIdent(lineIdent + identSpaces).Append("}");
      }
      else if (b is MemberListBinding mlb)
      {
        sb.Append("{");

        foreach (var i in mlb.Initializers)
        {
          sb.NewLineIdent(lineIdent + identSpaces);

          if (i.Arguments.Count > 1)
            sb.Append("(");

          var n = 0;

          foreach (var a in i.Arguments)
            a.ToCSharpString(
              ++n > 1 ? sb.Append(", ") : sb,
              lineIdent + identSpaces,
              stripNamespace,
              printType,
              identSpaces,
              tryPrintConstant);

          if (i.Arguments.Count > 1)
            sb.Append(")");

          sb.Append(",");
        }

        sb.NewLineIdent(lineIdent + identSpaces).Append("}");
      }

      sb.Append(",");
    }

    return sb;
  }
}