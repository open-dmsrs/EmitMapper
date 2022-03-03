using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace EmitMapper.Utils;
/// <summary>
/// The switch expressions.
/// </summary>

public static class SwitchExpressions
{
  /// <summary>
  /// Cases the SwitchExpression.
  /// </summary>
  /// <typeparam name="TSwitch"></typeparam>
  /// <typeparam name="TResult"></typeparam>
  /// <param name="matches">The matches.</param>
  /// <param name="valueFactory">The value factory.</param>
  /// <exception cref="ArgumentNullException"></exception>
  /// <returns><![CDATA[SwitchExpression<TSwitch, TResult>]]></returns>
  public static SwitchExpression<TSwitch, TResult> Case<TSwitch, TResult>(
    TSwitch matches,
    Func<TSwitch, TResult> valueFactory)
  {
    if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

    return SwitchExpression<TSwitch, TResult>.CreateCase(matches, valueFactory);
  }

  /// <summary>
  /// Cases the TypeCaseSwitchExpression.
  /// </summary>
  /// <typeparam name="TType"></typeparam>
  /// <typeparam name="TResult"></typeparam>
  /// <param name="valueFactory">The value factory.</param>
  /// <exception cref="ArgumentNullException"></exception>
  /// <returns><![CDATA[TypeCaseSwitchExpression<TResult>]]></returns>
  public static TypeCaseSwitchExpression<TResult> Case<TType, TResult>(Func<TType, TResult> valueFactory)
  {
    if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

    return new TypeCaseSwitchExpression<TType, TResult>(valueFactory);
  }

  /// <summary>
  /// Defaults the CompletedSwitchExpression.
  /// </summary>
  /// <typeparam name="TResult"></typeparam>
  /// <param name="result">The result.</param>
  /// <returns><![CDATA[CompletedSwitchExpression<TResult>]]></returns>
  public static CompletedSwitchExpression<TResult> Default<TResult>(TResult result)
  {
    return new CompletedSwitchExpression<TResult>(result);
  }

  /// <summary>
  /// Defaults the throw.
  /// </summary>
  /// <param name="exception">The exception.</param>
  /// <returns>A DefaultThrowExpression.</returns>
  public static DefaultThrowExpression DefaultThrow(Exception exception = null)
  {
    var exceptionToThrow = exception ?? new InvalidOperationException("switch expression did not match");
    if (exceptionToThrow.StackTrace != null) ExceptionDispatchInfo.Capture(exceptionToThrow).Throw();

    throw exceptionToThrow;
  }

  /// <summary>
  /// Switches the SwitchExpression.
  /// </summary>
  /// <typeparam name="TSwitch"></typeparam>
  /// <param name="on">The on.</param>
  /// <param name="comparer">The comparer.</param>
  /// <returns><![CDATA[SwitchExpression<TSwitch>]]></returns>
  public static SwitchExpression<TSwitch> Switch<TSwitch>(TSwitch on, IEqualityComparer<TSwitch> comparer = null)
  {
    return new SwitchExpression<TSwitch>(on, comparer ?? EqualityComparer<TSwitch>.Default);
  }

  public struct BooleanSwitchExpression<TResult>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="BooleanSwitchExpression&lt;TResult&gt;"/> class.
    /// </summary>
    /// <param name="condition">If true, condition.</param>
    /// <param name="valueFactory">The value factory.</param>
    internal BooleanSwitchExpression(bool condition, Func<TResult> valueFactory)
    {
      Condition = condition;
      ValueFactory = valueFactory;
    }

    /// <summary>
    /// Gets a value indicating whether condition.
    /// </summary>
    internal bool Condition { get; }

    /// <summary>
    /// Gets the value factory.
    /// </summary>
    internal Func<TResult> ValueFactory { get; }
  }

  public readonly struct CompletedSwitchExpression<TResult>
  {
    private readonly bool completed;

    private readonly TResult value;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompletedSwitchExpression&lt;TResult&gt;"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    internal CompletedSwitchExpression(TResult value)
    {
      this.value = value;
      completed = true;
    }

    public TResult Value
    {
      get
      {
        if (!completed) throw new InvalidOperationException("the switch has not completed");

        return value;
      }
    }

    public static CompletedSwitchExpression<TResult> operator |(
      CompletedSwitchExpression<TResult> first,
      CompletedSwitchExpression<TResult> second)
    {
      if (first.completed) throw new InvalidOperationException("use ||, not | to combine switch cases");

      return second;
    }

    public static bool operator false(CompletedSwitchExpression<TResult> @switch)
    {
      return !@switch.completed;
    }

    public static implicit operator TResult(CompletedSwitchExpression<TResult> @switch)
    {
      return @switch.Value;
    }

    public static bool operator true(CompletedSwitchExpression<TResult> @switch)
    {
      return @switch.completed;
    }
  }

  public struct DefaultThrowExpression
  {
  }

  public readonly struct SwitchExpression<TSwitch>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SwitchExpression&lt;TSwitch&gt;"/> class.
    /// </summary>
    /// <param name="on">The on.</param>
    /// <param name="comparer">The comparer.</param>
    internal SwitchExpression(TSwitch on, IEqualityComparer<TSwitch> comparer)
    {
      On = on;
      Comparer = comparer;
    }

    /// <summary>
    /// Gets the comparer.
    /// </summary>
    internal IEqualityComparer<TSwitch> Comparer { get; }

    /// <summary>
    /// Gets the on.
    /// </summary>
    internal TSwitch On { get; }

    public SwitchExpression<TSwitch, TResult> Case<TResult>(TSwitch matches, Func<TSwitch, TResult> valueFactory)
    {
      return SwitchExpression<TSwitch, TResult>.CreateCase(this) | SwitchExpressions.Case(matches, valueFactory);
    }

    public SwitchExpression<TSwitch, TResult> Case<TType, TResult>(Func<TType, TResult> valueFactory)
    {
      return SwitchExpression<TSwitch, TResult>.CreateCase(this) | SwitchExpressions.Case(valueFactory);
    }
  }

  public struct SwitchExpression<TSwitch, TResult>
  {
    private object objectState;

    private State state;

    private TSwitch switchValue;

    private TResult value;

    /// <summary>
    /// The state.
    /// </summary>
    private enum State : byte
    {
      None,

      CaseWithValueFactory,

      TrueBooleanCase,

      FalseBooleanCase,

      TypeCase,

      AwaitingMatch,

      Completed
    }

    public TResult Value
    {
      get
      {
        switch (state)
        {
          case State.Completed:
            return value;
          case State.AwaitingMatch:
            throw new InvalidOperationException("the switch has not yet matched");
          default:
            throw new InvalidOperationException(
              "this switch must be used as part of an || chain and cannot be used to start such a chain");
        }
      }
    }

    public static SwitchExpression<TSwitch, TResult> operator |(
      SwitchExpression<TSwitch, TResult> first,
      SwitchExpression<TSwitch, TResult> second)
    {
      if (first.state != State.AwaitingMatch)
        throw new InvalidOperationException("use ||, not | to combine switch cases");

      switch (second.state)
      {
        case State.Completed:
          return second;
        case State.CaseWithValueFactory:
          return ((IEqualityComparer<TSwitch>)first.objectState).Equals(first.switchValue, second.switchValue)
            ? new SwitchExpression<TSwitch, TResult>
            {
              value = ((Func<TSwitch, TResult>)second.objectState)(first.switchValue), state = State.Completed
            }
            : first; // still not matched
        case State.FalseBooleanCase:
          return first; // still not matched
        case State.TrueBooleanCase:
          return new SwitchExpression<TSwitch, TResult>
          {
            value = ((Func<TSwitch, TResult>)second.objectState)(first.switchValue), state = State.Completed
          };
        case State.TypeCase:
          TResult typeCaseResult;

          return ((TypeCaseSwitchExpression<TResult>)second.objectState).TryGetResult(
            first.switchValue,
            out typeCaseResult)
            ? new SwitchExpression<TSwitch, TResult> { value = typeCaseResult, state = State.Completed }
            : first; // still not matched
        default:
          throw new InvalidOperationException("right-hand side of || is not a valid case");
      }
    }

    public static bool operator false(SwitchExpression<TSwitch, TResult> switchState)
    {
      return switchState.state != State.Completed;
    }

    public static implicit operator SwitchExpression<TSwitch, TResult>(BooleanSwitchExpression<TResult> @switch)
    {
      return new SwitchExpression<TSwitch, TResult>
      {
        objectState = @switch.ValueFactory, state = @switch.Condition ? State.TrueBooleanCase : State.FalseBooleanCase
      };
    }

    public static implicit operator SwitchExpression<TSwitch, TResult>(TypeCaseSwitchExpression<TResult> @switch)
    {
      if (@switch == null) throw new InvalidOperationException(nameof(@switch));

      return CreateCase(@switch);
    }

    public static implicit operator SwitchExpression<TSwitch, TResult>(DefaultThrowExpression @switch)
    {
      throw new InvalidOperationException(
        $"{Metadata<DefaultThrowExpression>.TypeName}s should be constructed via the factory method and not used directly");
    }

    public static implicit operator CompletedSwitchExpression<TResult>(SwitchExpression<TSwitch, TResult> @switch)
    {
      return @switch.state == State.Completed ? new CompletedSwitchExpression<TResult>(@switch.value) : default;
    }

    public static bool operator true(SwitchExpression<TSwitch, TResult> switchState)
    {
      return switchState.state == State.Completed;
    }

    /// <summary>
    /// Creates the case.
    /// </summary>
    /// <param name="matches">The matches.</param>
    /// <param name="valueFactory">The value factory.</param>
    /// <returns><![CDATA[SwitchExpression<TSwitch, TResult>]]></returns>
    internal static SwitchExpression<TSwitch, TResult> CreateCase(TSwitch matches, Func<TSwitch, TResult> valueFactory)
    {
      return new SwitchExpression<TSwitch, TResult>
      {
        switchValue = matches, objectState = valueFactory, state = State.CaseWithValueFactory
      };
    }

    /// <summary>
    /// Creates the case.
    /// </summary>
    /// <param name="switch">The switch.</param>
    /// <returns><![CDATA[SwitchExpression<TSwitch, TResult>]]></returns>
    internal static SwitchExpression<TSwitch, TResult> CreateCase(TypeCaseSwitchExpression<TResult> @switch)
    {
      return new SwitchExpression<TSwitch, TResult> { objectState = @switch, state = State.TypeCase };
    }

    /// <summary>
    /// Creates the case.
    /// </summary>
    /// <param name="switch">The switch.</param>
    /// <returns><![CDATA[SwitchExpression<TSwitch, TResult>]]></returns>
    internal static SwitchExpression<TSwitch, TResult> CreateCase(SwitchExpression<TSwitch> @switch)
    {
      return new SwitchExpression<TSwitch, TResult>
      {
        switchValue = @switch.On, objectState = @switch.Comparer, state = State.AwaitingMatch
      };
    }
  }

  /// <summary>
  /// The type case switch expression.
  /// </summary>
  /// <typeparam name="TResult"></typeparam>
  public abstract class TypeCaseSwitchExpression<TResult>
  {
    /// <summary>
    /// Tries the get result.
    /// </summary>
    /// <typeparam name="TSwitch"></typeparam>
    /// <param name="switchValue">The switch value.</param>
    /// <param name="value">The value.</param>
    /// <returns>A bool.</returns>
    internal abstract bool TryGetResult<TSwitch>(TSwitch switchValue, out TResult value);
  }

  /// <summary>
  /// The type case switch expression.
  /// </summary>
  /// <typeparam name="TType"></typeparam>
  /// <typeparam name="TResult"></typeparam>
  private sealed class TypeCaseSwitchExpression<TType, TResult> : TypeCaseSwitchExpression<TResult>
  {
    private readonly Func<TType, TResult> resultFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeCaseSwitchExpression&lt;TType, TResult&gt;"/> class.
    /// </summary>
    /// <param name="resultFactory">The result factory.</param>
    public TypeCaseSwitchExpression(Func<TType, TResult> resultFactory)
    {
      this.resultFactory = resultFactory;
    }

    /// <summary>
    /// Tries the get result.
    /// </summary>
    /// <typeparam name="TSwitch"></typeparam>
    /// <param name="switchValue">The switch value.</param>
    /// <param name="value">The value.</param>
    /// <returns>A bool.</returns>
    internal override bool TryGetResult<TSwitch>(TSwitch switchValue, out TResult value)
    {
      if (switchValue is TType)
      {
        value = resultFactory((TType)(object)switchValue);

        return true;
      }

      value = default;

      return false;
    }
  }
}