using System;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.Mappers;
using EmitMapper.Utils;

namespace EmitMapper;

/// <summary>
///   Class which maintains an assembly for created object Mappers
/// </summary>
public class DynamicAssemblyManager
{
  private static readonly AssemblyBuilder _AssemblyBuilder;

  private static readonly AssemblyName _AssemblyName;

  private static readonly object _LockObject = new();

  private static readonly ModuleBuilder _ModuleBuilder;

  static DynamicAssemblyManager()
  {
    // var curAssemblyName = Assembly.GetExecutingAssembly().GetName();
    var curAssemblyName = Assembly.GetAssembly(Metadata<DynamicAssemblyManager>.Type)?.GetName();

#if !SILVERLIGHT
    _AssemblyName = new AssemblyName("EmitMapperAssembly");
    _AssemblyName.SetPublicKey(curAssemblyName.GetPublicKey());
    _AssemblyName.SetPublicKeyToken(curAssemblyName.GetPublicKeyToken());
    _AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(_AssemblyName, AssemblyBuilderAccess.RunAndCollect);

    _ModuleBuilder = _AssemblyBuilder.DefineDynamicModule(_AssemblyName.Name + ".dll");
#else
			assemblyName = new AssemblyName("EmitMapperAssembly.SL");
			assemblyName.KeyPair = kp;
			assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
				  assemblyName,
				  AssemblyBuilderAccess.Run
				  );
			moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, true);
#endif
  }

  /// <summary>
  ///   Saves assembly with created Mappers to file. This method is useful for debugging purpose.
  /// </summary>
  public static void SaveAssembly()
  {
    lock (_LockObject)
    {
      throw new NotSupportedException("DynamicAssemblyManager.SaveAssembly");

      // assemblyBuilder.Save(assemblyName.Name + ".dll");
    }
  }

  internal static TypeBuilder DefineMapperType(string typeName)
  {
    lock (_LockObject)
    {
      return _ModuleBuilder.DefineType(
        CorrectTypeName(typeName + Guid.NewGuid().ToString().Replace("-", string.Empty)),
        TypeAttributes.Public,
        Metadata<MapperForClass>.Type,
        null);
    }
  }

  internal static TypeBuilder DefineType(string typeName, Type parent)
  {
    lock (_LockObject)
    {
      return _ModuleBuilder.DefineType(CorrectTypeName(typeName), TypeAttributes.Public, parent, null);
    }
  }

  private static string CorrectTypeName(string typeName)
  {
    if (typeName.Length >= 1042)
      typeName = "type_" + typeName.Substring(0, 900) + Guid.NewGuid().ToString().Replace("-", string.Empty);

    return typeName;
  }
}