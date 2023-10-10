namespace EmitMapper.Tests;

/// <summary>
///   The emit invoker test.
/// </summary>
public class EmitInvokerTest
{
  /// <summary>
  ///   Emits the invoker test_ test call1.
  /// </summary>
  [Fact]
  public void EmitInvokerTest_TestCall1()
  {
    var i = 0;
    var caller = (DelegateInvokerAction0)DelegateInvoker.GetDelegateInvoker((Action)(() => i++));
    caller.CallAction();
    caller.CallAction();
    caller.CallAction();
    i.ShouldBe(3);

    var caller2 = (DelegateInvokerAction0)DelegateInvoker.GetDelegateInvoker((Action)(() => i += 2));
    caller2.CallAction();
    caller2.CallAction();
    i.ShouldBe(7);
  }

  /// <summary>
  ///   Emits the invoker test_ test call2.
  /// </summary>
  [Fact]
  public void EmitInvokerTest_TestCall2()
  {
    var caller = (DelegateInvokerFunc0)DelegateInvoker.GetDelegateInvoker((Func<int>)(() => 3));
    caller.CallFunc().ShouldBe(3);

    var caller2 = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker((Func<int, int, int>)((l, r) => l + r));

    // DynamicAssemblyManager.SaveAssembly();
    Assert.Equal(5, caller2.CallFunc(2, 3));
  }

  /// <summary>
  ///   Emits the invoker test_ test call3.
  /// </summary>
  [Fact]
  public void EmitInvokerTest_TestCall3()
  {
    var caller = (MethodInvokerFunc0)MethodInvoker.GetMethodInvoker(this, GetType().GetMethodCache("InvokeTest1"));
    caller.CallFunc().ShouldBe(3);

    var caller2 = (MethodInvokerFunc1)MethodInvoker.GetMethodInvoker(
      this,
      GetType().GetMethod("InvokeTest2", BindingFlags.Static | BindingFlags.Public));

    caller2.CallFunc(5).ShouldBe(5);
  }

  /// <summary>
  ///   Invokes the test1.
  /// </summary>
  /// <returns>An int.</returns>
  public int InvokeTest1()
  {
    return 3;
  }

  /// <summary>
  ///   Invokes the test2.
  /// </summary>
  /// <param name="par">The par.</param>
  /// <returns>An int.</returns>
  public static int InvokeTest2(int par)
  {
    return par;
  }
}