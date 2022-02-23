using System;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.Utils;

namespace EmitMapper;

/// <summary>Reflecting the internal methods to access the more performant for defining the local variable</summary>
public static class ILGeneratorHacks
{
  // The original ILGenerator methods we are trying to hack without allocating the `LocalBuilder`
  /*
        public virtual LocalBuilder DeclareLocal(Type localType)
        {
            return this.DeclareLocal(localType, false);
        }

        public virtual LocalBuilder DeclareLocal(Type localType, bool pinned)
        {
            MethodBuilder methodBuilder = this.m_methodBuilder as MethodBuilder;
            if ((MethodInfo)methodBuilder == (MethodInfo)null)
                throw new NotSupportedException();
            if (methodBuilder.IsTypeCreated())
                throw new InvalidOperationException(SR.InvalidOperation_TypeHasBeenCreated);
            if (localType == (Type)null)
                throw new ArgumentNullException(nameof(localType));
            if (methodBuilder.m_bIsBaked)
                throw new InvalidOperationException(SR.InvalidOperation_MethodBaked);
            this.m_localSignature.AddArgument(localType, pinned);
            LocalBuilder localBuilder = new LocalBuilder(this.m_localCount, localType, (MethodInfo)methodBuilder, pinned);
            ++this.m_localCount;
            return localBuilder;
        }
        */

  private static readonly Func<ILGenerator, Type, int> _getNextLocalVarIndex;
  private static readonly Type _Type = typeof(ILGeneratorHacks);

  static ILGeneratorHacks()
  {
    // the default allocatee method
    _getNextLocalVarIndex = (i, t) => i.DeclareLocal(t).LocalIndex;

    // now let's try to acquire the more efficient less allocating method
    var ilGenTypeInfo = Metadata<ILGenerator>.Type.GetTypeInfo();
    var localSignatureField = ilGenTypeInfo.GetDeclaredField("m_localSignature");

    if (localSignatureField == null)
      return;

    var localCountField = ilGenTypeInfo.GetDeclaredField("m_localCount");

    if (localCountField == null)
      return;

    // looking for the `SignatureHelper.AddArgument(Type argument, bool pinned)`
    MethodInfo addArgumentMethod = null;

    foreach (var m in Metadata<SignatureHelper>.Type.GetTypeInfo().GetDeclaredMethods("AddArgument"))
    {
      var ps = m.GetParameters();

      if (ps.Length == 2 && ps[0].ParameterType == Metadata<Type>.Type && ps[1].ParameterType == Metadata<bool>.Type)
      {
        addArgumentMethod = m;

        break;
      }
    }

    if (addArgumentMethod == null)
      return;

    // our own helper - always available
    var postIncMethod = _Type.GetTypeInfo().GetDeclaredMethod(nameof(PostInc));

    var efficientMethod = new DynamicMethod(
      string.Empty,
      Metadata<int>.Type,
      new[] { Metadata<ExpressionCompiler.ArrayClosure>.Type, Metadata<ILGenerator>.Type, Metadata<Type>.Type },
      Metadata<ExpressionCompiler.ArrayClosure>.Type,
      true);

    var il = efficientMethod.GetILGenerator();

    // emitting `il.m_localSignature.AddArgument(type);`
    il.Emit(OpCodes.Ldarg_1); // load `il` argument (arg_0 is the empty closure object)
    il.Emit(OpCodes.Ldfld, localSignatureField);
    il.Emit(OpCodes.Ldarg_2); // load `type` argument
    il.Emit(OpCodes.Ldc_I4_0); // load `pinned: false` argument
    il.Emit(OpCodes.Call, addArgumentMethod);

    // emitting `return PostInc(ref il.LocalCount);`
    il.Emit(OpCodes.Ldarg_1); // load `il` argument
    il.Emit(OpCodes.Ldflda, localCountField);
    il.Emit(OpCodes.Call, postIncMethod);

    il.Emit(OpCodes.Ret);

    _getNextLocalVarIndex = (Func<ILGenerator, Type, int>)efficientMethod.CreateDelegate(
      Metadata<Func<ILGenerator, Type, int>>.Type,
      ExpressionCompiler.EmptyArrayClosure);

    // todo: @perf do batch Emit by manually calling `EnsureCapacity` once then `InternalEmit` multiple times
    // todo: @perf Replace the `Emit(opcode, int)` with the more specialized `Emit(opcode)`, `Emit(opcode, byte)` or `Emit(opcode, short)` 
    // avoiding internal check for Ldc_I4, Ldarg, Ldarga, Starg then call `PutInteger4` only if needed see https://source.dot.net/#System.Private.CoreLib/src/System/Reflection/Emit/ILGenerator.cs,690f350859394132
    // var ensureCapacityMethod = ilGenTypeInfo.GetDeclaredMethod("EnsureCapacity");
    // var internalEmitMethod   = ilGenTypeInfo.GetDeclaredMethod("InternalEmit");
    // var putInteger4Method    = ilGenTypeInfo.GetDeclaredMethod("PutInteger4");
  }

  /// <summary>Efficiently returns the next variable index, hopefully without unnecessary allocations.</summary>
  public static int GetNextLocalVarIndex(this ILGenerator il, Type t)
  {
    return _getNextLocalVarIndex(il, t);
  }

  internal static int PostInc(ref int i)
  {
    return i++;
  }

  // todo: @perf add MultiOpCodes emit to save on the EnsureCapacity calls
  // todo: @perf create EmitMethod without additional GetParameters call
  /*
        public virtual void EmitCall(OpCode opcode, MethodInfo methodInfo, 
            int stackExchange = (methodInfo.ReturnType != TypeHome.Void ? 1 : 0) - methodInfo.GetParameterTypes().Length - (methodInfo.IsStatic ? 1 : 0))
        { 
            var tk = GetMemberRefToken(methodInfo, null);
 
            EnsureCapacity(7);
            InternalEmit(opcode);
 
            // * move outside of the method
            // Push the return value if there is one.
            // if (methodInfo.ReturnType != TypeHome.Void)
            //     stackchange++;

            // * move outside of the method
            // Pop the parameters.
            // stackchange -= methodInfo.GetParameterTypes().Length;

            // * move outside of the method
            // Pop the this parameter if the method is non-static and the
            // instruction is not newobj.
            // if (!methodInfo.IsStatic)
            //     stackchange--;

            UpdateStackSize(opcode, stackchange);
            PutInteger4(tk);
        }
        */
}