using System;
using System.Collections;
using System.Collections.Generic;

namespace EmitMapper.Utils;

/// <summary>
///   类型族
///   <para>2010/12/21</para>
///   <para>THINKPADT61</para>
///   <para>tangjingbo</para>
/// </summary>
public static class TypeHome
{
  public static readonly Type Void = typeof(void);

  /// <summary>
  /// </summary>
  public static readonly Type Int16 = Metadata<short>.Type;

  public static readonly Type Short = Int16;

  /// <summary>
  /// </summary>
  public static readonly Type Int32 = Metadata<int>.Type;

  public static readonly Type Int = Int32;

  /// <summary>
  /// </summary>
  public static readonly Type Int64 = Metadata<long>.Type;

  public static readonly Type Long = Int64;

  /// <summary>
  /// </summary>
  public static readonly Type UInt64 = Metadata<ulong>.Type;

  public static readonly Type ULong = UInt64;

  /// <summary>
  /// </summary>
  public static readonly Type Single = Metadata<float>.Type;

  public static readonly Type Float = Single;

  /// <summary>
  /// </summary>
  public static readonly Type UInt32 = Metadata<uint>.Type;

  public static readonly Type UInt = UInt32;

  /// <summary>
  ///   Type UInt16
  /// </summary>
  public static readonly Type UInt16 = Metadata<ushort>.Type;

  public static readonly Type UShort = UInt16;
  public static readonly Type ArrayList = Metadata<ArrayList>.Type;
  public static readonly Type List1 = typeof(List<>);
  public static readonly Type IEnumerable = Metadata<IEnumerable>.Type;
  public static readonly Type IList = Metadata<IList>.Type;
  public static readonly Type IList1 = typeof(IList<>);

  #region 类型常量

  /// <summary>
  /// </summary>
  public static readonly Type AccessViolationException = Metadata<AccessViolationException>.Type;

  public static readonly Type Action = Metadata<Action>.Type;


  /// <summary>
  /// </summary>
  public static readonly Type AppDomain = Metadata<AppDomain>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type AppDomainUnloadedException = Metadata<AppDomainUnloadedException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ApplicationException = Metadata<ApplicationException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ApplicationId = Metadata<ApplicationId>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ArgumentException = Metadata<ArgumentException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ArgumentNullException = Metadata<ArgumentNullException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ArgumentOutOfRangeException = Metadata<ArgumentOutOfRangeException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ArithmeticException = Metadata<ArithmeticException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type Array = Metadata<Array>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ArrayTypeMismatchException = Metadata<ArrayTypeMismatchException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type AssemblyLoadEventArgs = Metadata<AssemblyLoadEventArgs>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type AssemblyLoadEventHandler = Metadata<AssemblyLoadEventHandler>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type AsyncCallback = Metadata<AsyncCallback>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type Attribute = Metadata<Attribute>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type AttributeTargets = Metadata<AttributeTargets>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type AttributeUsageAttribute = Metadata<AttributeUsageAttribute>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type BadImageFormatException = Metadata<BadImageFormatException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type Base64FormattingOptions = Metadata<Base64FormattingOptions>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type Boolean = Metadata<bool>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type Byte = Metadata<byte>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type Bytes = typeof(byte[]);

  /// <summary>
  /// </summary>
  public static readonly Type CannotUnloadAppDomainException = Metadata<CannotUnloadAppDomainException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type Char = Metadata<char>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type CharEnumerator = Metadata<CharEnumerator>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type CLSCompliantAttribute = Metadata<CLSCompliantAttribute>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ConsoleCancelEventArgs = Metadata<ConsoleCancelEventArgs>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ConsoleCancelEventHandler = Metadata<ConsoleCancelEventHandler>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ConsoleColor = Metadata<ConsoleColor>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ConsoleKey = Metadata<ConsoleKey>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ConsoleKeyInfo = Metadata<ConsoleKeyInfo>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ConsoleModifiers = Metadata<ConsoleModifiers>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ConsoleSpecialKey = Metadata<ConsoleSpecialKey>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ContextBoundObject = Metadata<ContextBoundObject>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ContextStaticAttribute = Metadata<ContextStaticAttribute>.Type;

  public static readonly Type Convert = typeof(Convert);

  /// <summary>
  /// </summary>
  public static readonly Type DataMisalignedException = Metadata<DataMisalignedException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type DateTime = Metadata<DateTime>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type DateTimeKind = Metadata<DateTimeKind>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type DateTimeOffset = Metadata<DateTimeOffset>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type DayOfWeek = Metadata<DayOfWeek>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type DBNull = Metadata<DBNull>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type Decimal = Metadata<decimal>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type Delegate = Metadata<Delegate>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type DivideByZeroException = Metadata<DivideByZeroException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type DllNotFoundException = Metadata<DllNotFoundException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type Double = Metadata<double>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type DuplicateWaitObjectException = Metadata<DuplicateWaitObjectException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type EntryPointNotFoundException = Metadata<EntryPointNotFoundException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type Enum = Metadata<Enum>.Type;

  //public readonly static Type Environment = Meta<Environment>.Type;
  /// <summary>
  /// </summary>
  public static readonly Type EnvironmentVariableTarget = Metadata<EnvironmentVariableTarget>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type EventArgs = Metadata<EventArgs>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type EventHandler = Metadata<EventHandler>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type Exception = Metadata<Exception>.Type;

  //public static readonly Type ExecutionEngineException = Meta<ExecutionEngineException>.Type;
  /// <summary>
  /// </summary>
  public static readonly Type FieldAccessException = Metadata<FieldAccessException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type FlagsAttribute = Metadata<FlagsAttribute>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type FormatException = Metadata<FormatException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type GCCollectionMode = Metadata<GCCollectionMode>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type GCNotificationStatus = Metadata<GCNotificationStatus>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type Guid = Metadata<Guid>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type IndexOutOfRangeException = Metadata<IndexOutOfRangeException>.Type;

  //public readonly static Type InsufficientExecutionStackException = Meta<InsufficientExecutionStackException>.Type;
  /// <summary>
  /// </summary>
  public static readonly Type InsufficientMemoryException = Metadata<InsufficientMemoryException>.Type;


  /// <summary>
  /// </summary>
  public static readonly Type IntPtr = Metadata<IntPtr>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type InvalidCastException = Metadata<InvalidCastException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type InvalidOperationException = Metadata<InvalidOperationException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type InvalidProgramException = Metadata<InvalidProgramException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type InvalidTimeZoneException = Metadata<InvalidTimeZoneException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type LoaderOptimization = Metadata<LoaderOptimization>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type LoaderOptimizationAttribute = Metadata<LoaderOptimizationAttribute>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type LocalDataStoreSlot = Metadata<LocalDataStoreSlot>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type MarshalByRefObject = Metadata<MarshalByRefObject>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type Math = typeof(Math);

  /// <summary>
  /// </summary>
  public static readonly Type MemberAccessException = Metadata<MemberAccessException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type MethodAccessException = Metadata<MethodAccessException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type MidpointRounding = Metadata<MidpointRounding>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type MissingFieldException = Metadata<MissingFieldException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type MissingMemberException = Metadata<MissingMemberException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type MissingMethodException = Metadata<MissingMethodException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ModuleHandle = Metadata<ModuleHandle>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type MTAThreadAttribute = Metadata<MTAThreadAttribute>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type MulticastDelegate = Metadata<MulticastDelegate>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type MulticastNotSupportedException = Metadata<MulticastNotSupportedException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type NonSerializedAttribute = Metadata<NonSerializedAttribute>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type NotFiniteNumberException = Metadata<NotFiniteNumberException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type NotImplementedException = Metadata<NotImplementedException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type NotSupportedException = Metadata<NotSupportedException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type NullReferenceException = Metadata<NullReferenceException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type Object = Metadata<object>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ObjectDisposedException = Metadata<ObjectDisposedException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ObsoleteAttribute = Metadata<ObsoleteAttribute>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type OperatingSystem = Metadata<OperatingSystem>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type OperationCanceledException = Metadata<OperationCanceledException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type OutOfMemoryException = Metadata<OutOfMemoryException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type OverflowException = Metadata<OverflowException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ParamArrayAttribute = Metadata<ParamArrayAttribute>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type PlatformID = Metadata<PlatformID>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type PlatformNotSupportedException = Metadata<PlatformNotSupportedException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type Random = Metadata<Random>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type RankException = Metadata<RankException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ResolveEventArgs = Metadata<ResolveEventArgs>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ResolveEventHandler = Metadata<ResolveEventHandler>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type RuntimeFieldHandle = Metadata<RuntimeFieldHandle>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type RuntimeMethodHandle = Metadata<RuntimeMethodHandle>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type RuntimeTypeHandle = Metadata<RuntimeTypeHandle>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type SByte = Metadata<sbyte>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type SerializableAttribute = Metadata<SerializableAttribute>.Type;

  //public readonly static Type SpecialFolder = Meta<SpecialFolder>.Type;
  //public readonly static Type SpecialFolderOption = Meta<SpecialFolderOption>.Type;
  /// <summary>
  /// </summary>
  public static readonly Type StackOverflowException = Metadata<StackOverflowException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type STAThreadAttribute = Metadata<STAThreadAttribute>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type String = Metadata<string>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type StringComparer = Metadata<StringComparer>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type StringComparison = Metadata<StringComparison>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type StringSplitOptions = Metadata<StringSplitOptions>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type SystemException = Metadata<SystemException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ThreadStaticAttribute = Metadata<ThreadStaticAttribute>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type TimeoutException = Metadata<TimeoutException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type TimeSpan = Metadata<TimeSpan>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type TimeZoneInfo = Metadata<TimeZoneInfo>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type TimeZoneNotFoundException = Metadata<TimeZoneNotFoundException>.Type;

  //public readonly static Type TransitionTime = Meta<TransitionTime>.Type;
  //public readonly static Type Tuple = Meta<Tuple>.Type;
  /// <summary>
  /// </summary>
  public static readonly Type Type = Metadata<Type>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type TypeInitializationException = Metadata<TypeInitializationException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type TypeLoadException = Metadata<TypeLoadException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type TypeUnloadedException = Metadata<TypeUnloadedException>.Type;


  /// <summary>
  /// </summary>
  public static readonly Type UIntPtr = Metadata<UIntPtr>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type UnauthorizedAccessException = Metadata<UnauthorizedAccessException>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type UnhandledExceptionEventArgs = Metadata<UnhandledExceptionEventArgs>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type UnhandledExceptionEventHandler = Metadata<UnhandledExceptionEventHandler>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type ValueType = Metadata<ValueType>.Type;

  /// <summary>
  /// </summary>
  public static readonly Type Version = Metadata<Version>.Type;

  //public readonly static Type Void = TypeHome.Void;
  /// <summary>
  /// </summary>
  public static readonly Type WeakReference = Metadata<WeakReference>.Type;

  #endregion //类型常量
}