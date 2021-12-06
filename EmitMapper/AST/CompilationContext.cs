namespace EmitMapper.AST;

using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

internal class CompilationContext
{
    public readonly ILGenerator ILGenerator;

    public readonly TextWriter OutputCommands;

    private int stackCount;

    public CompilationContext()
    {
        this.OutputCommands = TextWriter.Null;
        //outputCommands = Console.Out;
    }

    public CompilationContext(ILGenerator ilGenerator)
        : this()
    {
        this.ILGenerator = ilGenerator;
    }

    public void ThrowException(Type exType)
    {
        this.ILGenerator.ThrowException(exType);
    }

    public void Emit(OpCode opCode)
    {
        this.ProcessCommand(opCode, 0, "");
        this.ILGenerator.Emit(opCode);
    }

    public void Emit(OpCode opCode, string str)
    {
        this.ProcessCommand(opCode, 0, str);
        this.ILGenerator.Emit(opCode, str);
    }

    public void Emit(OpCode opCode, int param)
    {
        this.ProcessCommand(opCode, 0, param.ToString());
        this.ILGenerator.Emit(opCode, param);
    }

    public void EmitNewObject(ConstructorInfo ci)
    {
        this.ProcessCommand(OpCodes.Newobj, ci.GetParameters().Length * -1 + 1, ci.ToString());

        this.ILGenerator.Emit(OpCodes.Newobj, ci);
    }

    public void EmitCall(OpCode opCode, MethodInfo mi)
    {
        this.ProcessCommand(
            opCode,
            (mi.GetParameters().Length + 1) * -1 + (mi.ReturnType == typeof(void) ? 0 : 1),
            mi.ToString());

        this.ILGenerator.EmitCall(opCode, mi, null);
    }

    public void Emit(OpCode opCode, FieldInfo fi)
    {
        this.ProcessCommand(opCode, 0, fi.ToString());
        this.ILGenerator.Emit(opCode, fi);
    }

    public void Emit(OpCode opCode, ConstructorInfo ci)
    {
        this.ProcessCommand(opCode, 0, ci.ToString());
        this.ILGenerator.Emit(opCode, ci);
    }

    public void Emit(OpCode opCode, LocalBuilder lb)
    {
        this.ProcessCommand(opCode, 0, lb.ToString());
        this.ILGenerator.Emit(opCode, lb);
    }

    public void Emit(OpCode opCode, Label lb)
    {
        this.ProcessCommand(opCode, 0, lb.ToString());
        this.ILGenerator.Emit(opCode, lb);
    }

    public void Emit(OpCode opCode, Type type)
    {
        this.ProcessCommand(opCode, 0, type.ToString());
        this.ILGenerator.Emit(opCode, type);
    }

    private void ProcessCommand(OpCode opCode, int addStack, string comment)
    {
        var stackChange = this.GetStackChange(opCode.StackBehaviourPop) + this.GetStackChange(opCode.StackBehaviourPush)
                                                                        + addStack;

        this.stackCount += stackChange;
        this.WriteOutputCommand(opCode + " " + comment);
    }

    private int GetStackChange(StackBehaviour beh)
    {
        switch (beh)
        {
            case StackBehaviour.Pop0:
            case StackBehaviour.Push0:
                return 0;

            case StackBehaviour.Pop1:
            case StackBehaviour.Popi:
            case StackBehaviour.Popref:
            case StackBehaviour.Varpop:
                return -1;

            case StackBehaviour.Push1:
            case StackBehaviour.Pushi:
            case StackBehaviour.Pushref:
            case StackBehaviour.Varpush:
                return 1;

            case StackBehaviour.Pop1_pop1:
            case StackBehaviour.Popi_pop1:
            case StackBehaviour.Popi_popi:
            case StackBehaviour.Popi_popi8:
            case StackBehaviour.Popi_popr4:
            case StackBehaviour.Popi_popr8:
            case StackBehaviour.Popref_pop1:
            case StackBehaviour.Popref_popi:
                return -2;

            case StackBehaviour.Push1_push1:
                return 2;

            case StackBehaviour.Popref_popi_pop1:
            case StackBehaviour.Popref_popi_popi:
            case StackBehaviour.Popref_popi_popi8:
            case StackBehaviour.Popref_popi_popr4:
            case StackBehaviour.Popref_popi_popr8:
            case StackBehaviour.Popref_popi_popref:
                return -3;
        }

        return 0;
    }

    private void WriteOutputCommand(string command)
    {
        if (this.OutputCommands != null)
            this.OutputCommands.WriteLine(new string('\t', this.stackCount >= 0 ? this.stackCount : 0) + command);
    }
}