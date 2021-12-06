using EmitMapper.EmitInvoker.Delegates;
using EmitMapper.EmitInvoker.Methods;
using System;
using System.Reflection;
using Xunit;

namespace EmitMapperTests
{
    public class EmitInvokerTest
    {
        [Fact]
        public void EmitInvokerTest_TestCall1()
        {
            int i = 0;
            DelegateInvokerAction_0 caller = (DelegateInvokerAction_0)DelegateInvoker.GetDelegateInvoker((Action)(() => i++));
            caller.CallAction();
            caller.CallAction();
            caller.CallAction();
            Assert.Equal(3, i);

            DelegateInvokerAction_0 caller2 = (DelegateInvokerAction_0)DelegateInvoker.GetDelegateInvoker((Action)(() => i += 2));
            caller2.CallAction();
            caller2.CallAction();
            Assert.Equal(7, i);
        }

        [Fact]
        public void EmitInvokerTest_TestCall2()
        {
            DelegateInvokerFunc_0 caller = (DelegateInvokerFunc_0)DelegateInvoker.GetDelegateInvoker((Func<int>)(() => 3));
            Assert.Equal(3, caller.CallFunc());

            DelegateInvokerFunc_2 caller2 = (DelegateInvokerFunc_2)DelegateInvoker.GetDelegateInvoker((Func<int, int, int>)((l, r) => l + r));
            //DynamicAssemblyManager.SaveAssembly();
            Assert.Equal(5, caller2.CallFunc(2, 3));
        }

        public int InvokeTest1()
        {
            return 3;
        }

        public static int InvokeTest2(int par)
        {
            return par;
        }

        [Fact]
        public void EmitInvokerTest_TestCall3()
        {
            MethodInvokerFunc_0 caller = (MethodInvokerFunc_0)MethodInvoker.GetMethodInvoker(this, GetType().GetMethod("InvokeTest1"));
            Assert.Equal(3, caller.CallFunc());

            MethodInvokerFunc_1 caller2 = (MethodInvokerFunc_1)MethodInvoker.GetMethodInvoker(this, GetType().GetMethod("InvokeTest2", BindingFlags.Static | BindingFlags.Public));
            Assert.Equal(5, caller2.CallFunc(5));

        }

    }
}
