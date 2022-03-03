using System;
using System.Reflection;
using EmitMapper.EmitInvoker.Delegates;
using EmitMapper.EmitInvoker.Methods;
using EmitMapper.Utils;
using Shouldly;
using Xunit;

namespace EmitMapper.Tests;

public class EmitInvokerTest
{
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

  [Fact]
  public void EmitInvokerTest_TestCall2()
  {
    var caller = (DelegateInvokerFunc0)DelegateInvoker.GetDelegateInvoker((Func<int>)(() => 3));
    caller.CallFunc().ShouldBe(3);

    var caller2 = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker((Func<int, int, int>)((l, r) => l + r));

    // DynamicAssemblyManager.SaveAssembly();
    Assert.Equal(5, caller2.CallFunc(2, 3));
  }

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

  public int InvokeTest1()
  {
    return 3;
  }

  public static int InvokeTest2(int par)
  {
    return par;
  }
}