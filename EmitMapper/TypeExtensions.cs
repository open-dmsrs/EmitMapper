using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.Mappers;

#if !UnitTest
namespace EmitMapper
#else
namespace EmitMapperTests
#endif
{
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, Func<object>> Creators = new(
            Environment.ProcessorCount,
            1024);
        public static object Create(this Type @this)
        {
            if (Creators.ContainsKey(@this))
            {
                return Creators[@this];
            }

            lock (Creators)
            {
                if (Creators.ContainsKey(@this))
                {
                    return Creators[@this];
                }
                return Creators.GetOrAdd(@this,
                    k => Expression.Lambda<Func<object>>(Expression.New(@this))
                        .Compile()
                );
            }
        }

        //public static MethodInfo GetMethod(this Type type, string methodName)
        //{
        //    return type.GetTypeInfo().GetMethod(methodName);
        //}

        //public static MethodInfo GetMethod(this Type type, string methodName, BindingFlags flags)
        //{
        //    return type.GetTypeInfo().GetMethod(methodName, flags);
        //}

        //public static MethodInfo GetMethod(this Type type, string methodName, Type[] types)
        //{
        //    return type.GetTypeInfo().GetMethod(methodName, types);
        //}

        //public static MethodInfo[] GetMethods(this Type type, BindingFlags flags)
        //{
        //    return type.GetTypeInfo().GetMethods(flags);
        //}

        //public static PropertyInfo GetProperty(this Type type, string propertyName)
        //{
        //    return type.GetTypeInfo().GetProperty(propertyName);
        //}

        //public static ConstructorInfo GetConstructor(this Type type, Type[] types)
        //{
        //    return type.GetTypeInfo().GetConstructor(types);
        //}

        //public static ConstructorInfo GetConstructor(this Type type, int a, Type[] types)
        //{
        //    return type.GetTypeInfo().GetConstructor(types);
        //}

        //public static ConstructorInfo GetConstructor(this Type type, BindingFlags b, Type[] types, object c)
        //{
        //    // todo: need to complete
        //    // throw new NotImplementedException();
        //    return type.GetTypeInfo().GetConstructor(types); //.FirstOrDefault(x =true);
        //}

        //public static FieldInfo GetField(this Type type, string name)
        //{
        //    return type.GetTypeInfo().GetField(name);
        //}

        //public static FieldInfo GetField(this Type type, string name, BindingFlags bfs)
        //{
        //    return type.GetTypeInfo().GetField(name, bfs);
        //}

        //public static FieldInfo[] GetFields(this Type type)
        //{
        //    return type.GetTypeInfo().GetFields();
        //}

        //public static PropertyInfo[] GetProperties(this Type type)
        //{
        //    return type.GetTypeInfo().GetProperties();
        //}

        //public static MemberInfo[] GetMembers(this Type type)
        //{
        //    return type.GetTypeInfo().GetMembers();
        //}

        //public static MemberInfo[] GetMember(this Type type, string name)
        //{
        //    return type.GetTypeInfo().GetMember(name);
        //}

        //public static MemberInfo[] GetMembers(this Type type, BindingFlags bfs)
        //{
        //    return type.GetTypeInfo().GetMembers(bfs);
        //}

        //public static Type[] GetGenericArguments(this Type type)
        //{
        //    return type.GetTypeInfo().GetGenericArguments();
        //}

        //public static bool IsValueType(this Type type)
        //{
        //    return type.GetTypeInfo().IsValueType;
        //}

        //public static bool IsGenericType(this Type type)
        //{
        //    return type.GetTypeInfo().IsGenericType;
        //}

        //public static bool IsAssignableFrom(this Type type, Type c)
        //{
        //    return type.GetTypeInfo().IsAssignableFrom(c);
        //}

        //public static bool IsEnum(this Type type)
        //{
        //    return type.GetTypeInfo().IsEnum;
        //}

        //public static bool IsGenericTypeDefinition(this Type type)
        //{
        //    return type.GetTypeInfo().IsGenericTypeDefinition;
        //}

        //public static bool IsInterface(this Type type)
        //{
        //    return type.GetTypeInfo().IsInterface;
        //}

        //public static Type[] GetInterfaces(this Type type)
        //{
        //    return type.GetTypeInfo().GetInterfaces();
        //}

        //public static bool IsSubclassOf(this Type type, Type target)
        //{
        //    return type.GetTypeInfo().IsSubclassOf(target);
        //}
    }

    /// <summary>
    /// 类型族
    /// <para>2010/12/21</para>
    /// 	<para>THINKPADT61</para>
    /// 	<para>tangjingbo</para>
    /// </summary>
    public static class TypeHome
    {
        #region 类型常量

        /// <summary>
        /// 
        /// </summary>
        public static readonly Type AccessViolationException = Meta<AccessViolationException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Action = Meta<Action>.Type;


        /// <summary>
        /// 
        /// </summary>
        public static readonly Type AppDomain = Meta<AppDomain>.Type;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Type AppDomainUnloadedException = Meta<AppDomainUnloadedException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ApplicationException = Meta<ApplicationException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ApplicationId = Meta<ApplicationId>.Type;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ArgumentException = Meta<ArgumentException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ArgumentNullException = Meta<ArgumentNullException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ArgumentOutOfRangeException = Meta<ArgumentOutOfRangeException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ArithmeticException = Meta<ArithmeticException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Array = Meta<Array>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ArrayTypeMismatchException = Meta<ArrayTypeMismatchException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type AssemblyLoadEventArgs = Meta<AssemblyLoadEventArgs>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type AssemblyLoadEventHandler = Meta<AssemblyLoadEventHandler>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type AsyncCallback = Meta<AsyncCallback>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Attribute = Meta<Attribute>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type AttributeTargets = Meta<AttributeTargets>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type AttributeUsageAttribute = Meta<AttributeUsageAttribute>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type BadImageFormatException = Meta<BadImageFormatException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Base64FormattingOptions = Meta<Base64FormattingOptions>.Type;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Boolean = Meta<Boolean>.Type;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Byte = Meta<Byte>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Bytes = typeof(Byte[]);
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type CannotUnloadAppDomainException = Meta<CannotUnloadAppDomainException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Char = Meta<Char>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type CharEnumerator = Meta<CharEnumerator>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type CLSCompliantAttribute = Meta<CLSCompliantAttribute>.Type;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ConsoleCancelEventArgs = Meta<ConsoleCancelEventArgs>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ConsoleCancelEventHandler = Meta<ConsoleCancelEventHandler>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ConsoleColor = Meta<ConsoleColor>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ConsoleKey = Meta<ConsoleKey>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ConsoleKeyInfo = Meta<ConsoleKeyInfo>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ConsoleModifiers = Meta<ConsoleModifiers>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ConsoleSpecialKey = Meta<ConsoleSpecialKey>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ContextBoundObject = Meta<ContextBoundObject>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ContextStaticAttribute = Meta<ContextStaticAttribute>.Type;
        public static readonly Type Convert = typeof(Convert);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Type DataMisalignedException = Meta<DataMisalignedException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type DateTime = Meta<DateTime>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type DateTimeKind = Meta<DateTimeKind>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type DateTimeOffset = Meta<DateTimeOffset>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type DayOfWeek = Meta<DayOfWeek>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type DBNull = Meta<DBNull>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Decimal = Meta<Decimal>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Delegate = Meta<Delegate>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type DivideByZeroException = Meta<DivideByZeroException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type DllNotFoundException = Meta<DllNotFoundException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Double = Meta<double>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type DuplicateWaitObjectException = Meta<DuplicateWaitObjectException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type EntryPointNotFoundException = Meta<EntryPointNotFoundException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Enum = Meta<Enum>.Type;
        //public readonly static Type Environment = Meta<Environment>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type EnvironmentVariableTarget = Meta<EnvironmentVariableTarget>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type EventArgs = Meta<EventArgs>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type EventHandler = Meta<EventHandler>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Exception = Meta<Exception>.Type;
        //public static readonly Type ExecutionEngineException = Meta<ExecutionEngineException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type FieldAccessException = Meta<FieldAccessException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type FlagsAttribute = Meta<FlagsAttribute>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type FormatException = Meta<FormatException>.Type;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Type GCCollectionMode = Meta<GCCollectionMode>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type GCNotificationStatus = Meta<GCNotificationStatus>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Guid = Meta<Guid>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type IndexOutOfRangeException = Meta<IndexOutOfRangeException>.Type;
        //public readonly static Type InsufficientExecutionStackException = Meta<InsufficientExecutionStackException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type InsufficientMemoryException = Meta<InsufficientMemoryException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Int16 = Meta<Int16>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Int32 = Meta<Int32>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Int64 = Meta<Int64>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type IntPtr = Meta<IntPtr>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type InvalidCastException = Meta<InvalidCastException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type InvalidOperationException = Meta<InvalidOperationException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type InvalidProgramException = Meta<InvalidProgramException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type InvalidTimeZoneException = Meta<InvalidTimeZoneException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type LoaderOptimization = Meta<LoaderOptimization>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type LoaderOptimizationAttribute = Meta<LoaderOptimizationAttribute>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type LocalDataStoreSlot = Meta<LocalDataStoreSlot>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type MarshalByRefObject = Meta<MarshalByRefObject>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Math = typeof(Math);
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type MemberAccessException = Meta<MemberAccessException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type MethodAccessException = Meta<MethodAccessException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type MidpointRounding = Meta<MidpointRounding>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type MissingFieldException = Meta<MissingFieldException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type MissingMemberException = Meta<MissingMemberException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type MissingMethodException = Meta<MissingMethodException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ModuleHandle = Meta<ModuleHandle>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type MTAThreadAttribute = Meta<MTAThreadAttribute>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type MulticastDelegate = Meta<MulticastDelegate>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type MulticastNotSupportedException = Meta<MulticastNotSupportedException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type NonSerializedAttribute = Meta<NonSerializedAttribute>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type NotFiniteNumberException = Meta<NotFiniteNumberException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type NotImplementedException = Meta<NotImplementedException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type NotSupportedException = Meta<NotSupportedException>.Type;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Type NullReferenceException = Meta<NullReferenceException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Object = Meta<object>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ObjectDisposedException = Meta<ObjectDisposedException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ObsoleteAttribute = Meta<ObsoleteAttribute>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type OperatingSystem = Meta<OperatingSystem>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type OperationCanceledException = Meta<OperationCanceledException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type OutOfMemoryException = Meta<OutOfMemoryException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type OverflowException = Meta<OverflowException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ParamArrayAttribute = Meta<ParamArrayAttribute>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type PlatformID = Meta<PlatformID>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type PlatformNotSupportedException = Meta<PlatformNotSupportedException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Random = Meta<Random>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type RankException = Meta<RankException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ResolveEventArgs = Meta<ResolveEventArgs>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ResolveEventHandler = Meta<ResolveEventHandler>.Type;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Type RuntimeFieldHandle = Meta<RuntimeFieldHandle>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type RuntimeMethodHandle = Meta<RuntimeMethodHandle>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type RuntimeTypeHandle = Meta<RuntimeTypeHandle>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type SByte = Meta<SByte>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type SerializableAttribute = Meta<SerializableAttribute>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Single = Meta<Single>.Type;
        //public readonly static Type SpecialFolder = Meta<SpecialFolder>.Type;
        //public readonly static Type SpecialFolderOption = Meta<SpecialFolderOption>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type StackOverflowException = Meta<StackOverflowException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type STAThreadAttribute = Meta<STAThreadAttribute>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type String = Meta<String>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type StringComparer = Meta<StringComparer>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type StringComparison = Meta<StringComparison>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type StringSplitOptions = Meta<StringSplitOptions>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type SystemException = Meta<SystemException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ThreadStaticAttribute = Meta<ThreadStaticAttribute>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type TimeoutException = Meta<TimeoutException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type TimeSpan = Meta<TimeSpan>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type TimeZone = Meta<TimeZone>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type TimeZoneInfo = Meta<TimeZoneInfo>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type TimeZoneNotFoundException = Meta<TimeZoneNotFoundException>.Type;
        //public readonly static Type TransitionTime = Meta<TransitionTime>.Type;
        //public readonly static Type Tuple = Meta<Tuple>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Type = Meta<Type>.Type;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Type TypeInitializationException = Meta<TypeInitializationException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type TypeLoadException = Meta<TypeLoadException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type TypeUnloadedException = Meta<TypeUnloadedException>.Type;
        /// <summary>
        /// Type UInt16
        /// </summary>
        public static readonly Type UInt16 = Meta<UInt16>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type UInt32 = Meta<UInt32>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type UInt64 = Meta<UInt64>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type UIntPtr = Meta<UIntPtr>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type UnauthorizedAccessException = Meta<UnauthorizedAccessException>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type UnhandledExceptionEventArgs = Meta<UnhandledExceptionEventArgs>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type UnhandledExceptionEventHandler = Meta<UnhandledExceptionEventHandler>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type ValueType = Meta<ValueType>.Type;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type Version = Meta<Version>.Type;
        //public readonly static Type Void = TypeHome.Void;
        /// <summary>
        /// 
        /// </summary>
        public static readonly Type WeakReference = Meta<WeakReference>.Type;

        #endregion //类型常量

        public static readonly Type Void = typeof(void);
        public static readonly Type Short = Int16;
        public static readonly Type Int = Int32;
        public static readonly Type Long = Int64;
        public static readonly Type ULong = UInt64;
        public static readonly Type Float = Single;
        public static readonly Type UInt = UInt32;
        public static readonly Type UShort = UInt16;
        public static readonly Type ArrayList = Meta<ArrayList>.Type;
        public static readonly Type List1 = typeof(List<>);
        public static readonly Type IEnumerable = Meta<IEnumerable>.Type;
        public static readonly Type IList = Meta<IList>.Type;
        public static readonly Type IList1 = typeof(IList<>);
    }

    internal static class Meta<T> //Avoid lock & string metadata description
    {
        public static readonly Type Type = typeof(T);

        public static FieldInfo Field<TX>(Expression<Func<T, TX>> expression)
        {
            return (expression.Body as MemberExpression)?.Member as FieldInfo;
        }

        public static PropertyInfo Property<TX>(Expression<Func<T, TX>> expression)
        {
            return (expression.Body as MemberExpression)?.Member as PropertyInfo;
        }

        public static MethodInfo Method(Expression<Action<T>> expression)
        {
            return (expression.Body as MethodCallExpression)?.Method;
        }

        public static MethodInfo Method<TX>(Expression<Func<T, TX>> expression)
        {
            return (expression.Body as MethodCallExpression)?.Method;
        }
    }

    class B
    {

        public int Test() => 0;
        public int Test(int a) => 1;
        public int Test(int a, int b) => 2;

        void M()
        {
            MethodInfo methodInfo = Meta<B>.Method(b => (Func<int, int, int>)((i, i1) => b.Test(i, i1)));
        }

    }


}