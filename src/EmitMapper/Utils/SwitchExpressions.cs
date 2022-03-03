using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace EmitMapper.Utils;

public static class SwitchExpressions
{
  public static SwitchExpression<TSwitch, TResult> Case<TSwitch, TResult>(
    TSwitch matches,
    Func<TSwitch, TResult> valueFactory)
  {
    if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

    return SwitchExpression<TSwitch, TResult>.CreateCase(matches, valueFactory);
  }

  public static TypeCaseSwitchExpression<TResult> Case<TType, TResult>(Func<TType, TResult> valueFactory)
  {
    if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

    return new TypeCaseSwitchExpression<TType, TResult>(valueFactory);
  }

  public static CompletedSwitchExpression<TResult> Default<TResult>(TResult result)
  {
    return new CompletedSwitchExpression<TResult>(result);
  }

  public static DefaultThrowExpression DefaultThrow(Exception exception = null)
  {
    var exceptionToThrow = exception ?? new InvalidOperationException("switch expression did not match");
    if (exceptionToThrow.StackTrace != null) ExceptionDispatchInfo.Capture(exceptionToThrow).Throw();

    throw exceptionToThrow;
  }

  public static SwitchExpression<TSwitch> Switch<TSwitch>(TSwitch on, IEqualityComparer<TSwitch> comparer = null)
  {
    return new SwitchExpression<TSwitch>(on, comparer ?? EqualityComparer<TSwitch>.Default);
  }

  public struct BooleanSwitchExpression<TResult>
  {
    internal BooleanSwitchExpression(bool condition, Func<TResult> valueFactory)
    {
      Condition = condition;
      ValueFactory = valueFactory;
    }

    internal bool Condition { get; }

    internal Func<TResult> ValueFactory { get; }
  }

  public readonly struct CompletedSwitchExpression<TResult>
  {
    private readonly bool completed;

    private readonly TResult value;

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
    internal SwitchExpression(TSwitch on, IEqualityComparer<TSwitch> comparer)
    {
      On = on;
      Comparer = comparer;
    }

    internal IEqualityComparer<TSwitch> Comparer { get; }

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

    internal static SwitchExpression<TSwitch, TResult> CreateCase(TSwitch matches, Func<TSwitch, TResult> valueFactory)
    {
      return new SwitchExpression<TSwitch, TResult>
      {
        switchValue = matches, objectState = valueFactory, state = State.CaseWithValueFactory
      };
    }

    internal static SwitchExpression<TSwitch, TResult> CreateCase(TypeCaseSwitchExpression<TResult> @switch)
    {
      return new SwitchExpression<TSwitch, TResult> { objectState = @switch, state = State.TypeCase };
    }

    internal static SwitchExpression<TSwitch, TResult> CreateCase(SwitchExpression<TSwitch> @switch)
    {
      return new SwitchExpression<TSwitch, TResult>
      {
        switchValue = @switch.On, objectState = @switch.Comparer, state = State.AwaitingMatch
      };
    }
  }

  public abstract class TypeCaseSwitchExpression<TResult>
  {
    internal abstract bool TryGetResult<TSwitch>(TSwitch switchValue, out TResult value);
  }

  private sealed class TypeCaseSwitchExpression<TType, TResult> : TypeCaseSwitchExpression<TResult>
  {
    private readonly Func<TType, TResult> resultFactory;

    public TypeCaseSwitchExpression(Func<TType, TResult> resultFactory)
    {
      this.resultFactory = resultFactory;
    }

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