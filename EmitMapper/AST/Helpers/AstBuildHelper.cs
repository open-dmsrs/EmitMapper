using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace EmitMapper.AST.Helpers
{
    internal class AstBuildHelper
    {
        public static IAstRefOrValue CallMethod(
            MethodInfo methodInfo,
            IAstRefOrAddr invocationObject,
            List<IAstStackItem> arguments)
        {
            if (methodInfo.ReturnType.IsValueType)
            {
                return new AstCallMethodValue(methodInfo, invocationObject, arguments);
            }
            else
            {
                return new AstCallMethodRef(methodInfo, invocationObject, arguments);
            }
        }

        public static IAstRefOrValue ReadArgumentRV(int argumentIndex, Type argumentType)
        {
            if (argumentType.IsValueType)
            {
                return new AstReadArgumentValue()
                {
                    ArgumentIndex = argumentIndex,
                    ArgumentType = argumentType
                };
            }
            else
            {
                return new AstReadArgumentRef()
                {
                    ArgumentIndex = argumentIndex,
                    ArgumentType = argumentType
                };
            }
        }

        public static IAstRefOrAddr ReadArgumentRA(int argumentIndex, Type argumentType)
        {
            if (argumentType.IsValueType)
            {
                return new AstReadArgumentAddr()
                {
                    ArgumentIndex = argumentIndex,
                    ArgumentType = argumentType
                };
            }
            else
            {
                return new AstReadArgumentRef()
                {
                    ArgumentIndex = argumentIndex,
                    ArgumentType = argumentType
                };
            }
        }
        public static IAstRefOrValue ReadArrayItemRV(IAstRef array, int index)
        {
            if (array.ItemType.IsValueType)
            {
                return new AstReadArrayItemValue()
                {
                    Array = array,
                    Index = index
                };
            }
            else
            {
                return new AstReadArrayItemRef()
                {
                    Array = array,
                    Index = index
                };
            }
        }

        public static IAstRefOrAddr ReadArrayItemRA(IAstRef array, int index)
        {
            if (array.ItemType.IsValueType)
            {
                return new AstReadArrayItemAddr()
                {
                    Array = array,
                    Index = index
                };
            }
            else
            {
                return new AstReadArrayItemRef()
                {
                    Array = array,
                    Index = index
                };
            }
        }

        public static IAstRefOrValue ReadFieldRV(IAstRefOrAddr sourceObject, FieldInfo fieldInfo)
        {
            if (fieldInfo.FieldType.IsValueType)
            {
                return new AstReadFieldValue()
                {
                    FieldInfo = fieldInfo,
                    SourceObject = sourceObject
                };
            }
            else
            {
                return new AstReadFieldRef()
                {
                    FieldInfo = fieldInfo,
                    SourceObject = sourceObject
                };
            }
        }

        public static IAstRefOrValue CastClass(IAstRefOrValue value, Type targetType)
        {
            if (targetType.IsValueType)
            {
                return new AstCastclassValue(value, targetType);
            }
            else
            {
                return new AstCastclassRef(value, targetType);
            }
        }

        public static IAstRefOrAddr ReadFieldRA(IAstRefOrAddr sourceObject, FieldInfo fieldInfo)
        {
            if (fieldInfo.FieldType.IsValueType)
            {
                return new AstReadFieldAddr()
                {
                    FieldInfo = fieldInfo,
                    SourceObject = sourceObject
                };
            }
            else
            {
                return new AstReadFieldRef()
                {
                    FieldInfo = fieldInfo,
                    SourceObject = sourceObject
                };
            }
        }

        public static IAstRefOrValue ReadLocalRV(LocalBuilder loc)
        {
            if (loc.LocalType.IsValueType)
            {
                return new AstReadLocalValue()
                {
                    LocalType = loc.LocalType,
                    LocalIndex = loc.LocalIndex
                };
            }
            else
            {
                return new AstReadLocalRef()
                {
                    LocalType = loc.LocalType,
                    LocalIndex = loc.LocalIndex
                };
            }
        }

        public static IAstRefOrAddr ReadLocalRA(LocalBuilder loc)
        {
            if (loc.LocalType.IsValueType)
            {
                return new AstReadLocalAddr(loc);
            }
            else
            {
                return new AstReadLocalRef()
                {
                    LocalType = loc.LocalType,
                    LocalIndex = loc.LocalIndex
                };
            }
        }

        public static IAstRefOrAddr ReadPropertyRA(IAstRefOrAddr sourceObject, PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType.IsValueType)
            {
                return new
                    AstValueToAddr(
                        new AstReadPropertyValue()
                        {
                            SourceObject = sourceObject,
                            PropertyInfo = propertyInfo
                        }
                    );
            }
            else
            {
                return new AstReadPropertyRef()
                {
                    SourceObject = sourceObject,
                    PropertyInfo = propertyInfo
                };
            }
        }

        public static IAstRefOrValue ReadPropertyRV(IAstRefOrAddr sourceObject, PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType.IsValueType)
            {
                return new AstReadPropertyValue()
                {
                    SourceObject = sourceObject,
                    PropertyInfo = propertyInfo
                };
            }
            else
            {
                return new AstReadPropertyRef()
                {
                    SourceObject = sourceObject,
                    PropertyInfo = propertyInfo
                };
            }
        }

        public static IAstRefOrAddr ReadThis(Type thisType)
        {
            if (thisType.IsValueType)
            {
                return new AstReadThisAddr()
                {
                    ThisType = thisType
                };
            }
            else
            {
                return new AstReadThisRef()
                {
                    ThisType = thisType
                };

            }
        }

        public static IAstRefOrValue ReadMembersChain(IAstRefOrAddr sourceObject, MemberInfo[] membersChain)
        {
            IAstRefOrAddr src = sourceObject;
            IAstRefOrValue result = null;

            for (int i = 0; i < membersChain.Length - 1; ++i)
            {
                src = ReadMemberRA(src, membersChain[i]);
            }
            result = ReadMemberRV(src, membersChain[membersChain.Length - 1]);
            return result;
        }

        public static IAstStackItem ReadMember(IAstRefOrAddr sourceObject, MemberInfo memberInfo)
        {
            if (memberInfo.MemberType == MemberTypes.Method)
            {
                MethodInfo methodInfo = memberInfo.DeclaringType.GetMethod(memberInfo.Name);
                if (methodInfo.ReturnType == null)
                {
                    throw new EmitMapperException("Invalid member:" + memberInfo.Name);
                }
                if (methodInfo.GetParameters().Length > 0)
                {
                    throw new EmitMapperException("Method " + memberInfo.Name + " should not have parameters");
                }
                return (IAstRef)CallMethod(methodInfo, sourceObject, null);
            }
            else if (memberInfo.MemberType == MemberTypes.Field)
            {
                return AstBuildHelper.ReadFieldRA(sourceObject, (FieldInfo)memberInfo);
            }
            else
            {
                return (IAstRef)AstBuildHelper.ReadPropertyRV(sourceObject, (PropertyInfo)memberInfo);
            }
        }

        public static IAstRefOrAddr ReadMemberRA(IAstRefOrAddr sourceObject, MemberInfo memberInfo)
        {
            if (memberInfo.MemberType == MemberTypes.Method)
            {
                MethodInfo methodInfo = memberInfo.DeclaringType.GetMethod(memberInfo.Name);
                if (methodInfo.ReturnType == null)
                {
                    throw new EmitMapperException("Invalid member:" + memberInfo.Name);
                }
                if (methodInfo.GetParameters().Length > 0)
                {
                    throw new EmitMapperException("Method " + memberInfo.Name + " should not have parameters");
                }
                if (methodInfo.ReturnType == null || methodInfo.ReturnType.IsValueType)
                {
                    throw new EmitMapperException("Method " + memberInfo.Name + " should return a reference");
                }
                return (IAstRef)CallMethod(methodInfo, sourceObject, null);
            }
            else if (memberInfo.MemberType == MemberTypes.Field)
            {
                return AstBuildHelper.ReadFieldRA(sourceObject, (FieldInfo)memberInfo);
            }
            else
            {
                PropertyInfo pi = (PropertyInfo)memberInfo;
                if (pi.PropertyType.IsValueType)
                {
                    return AstBuildHelper.ReadPropertyRA(sourceObject, (PropertyInfo)memberInfo);
                }
                return (IAstRef)AstBuildHelper.ReadPropertyRV(sourceObject, (PropertyInfo)memberInfo);
            }
        }


        public static IAstRefOrValue ReadMemberRV(IAstRefOrAddr sourceObject, MemberInfo memberInfo)
        {
            if (memberInfo.MemberType == MemberTypes.Method)
            {
                MethodInfo methodInfo = memberInfo.DeclaringType.GetMethod(memberInfo.Name);
                if (methodInfo.ReturnType == null)
                {
                    throw new EmitMapperException("Invalid member:" + memberInfo.Name);
                }
                if (methodInfo.GetParameters().Length > 0)
                {
                    throw new EmitMapperException("Method " + memberInfo.Name + " should not have parameters");
                }
                return CallMethod(methodInfo, sourceObject, null);
            }
            else if (memberInfo.MemberType == MemberTypes.Field)
            {
                return AstBuildHelper.ReadFieldRV(sourceObject, (FieldInfo)memberInfo);
            }
            else
            {
                return AstBuildHelper.ReadPropertyRV(sourceObject, (PropertyInfo)memberInfo);
            }
        }

        public static IAstNode WriteMembersChain(MemberInfo[] membersChain, IAstRefOrAddr targetObject, IAstRefOrValue value)
        {
            if (membersChain.Length == 1)
            {
                return WriteMember(membersChain[0], targetObject, value);
            }

            IAstRefOrAddr readTarget = targetObject;

            for (int i = 0; i < membersChain.Length - 1; ++i)
            {
                readTarget = ReadMemberRA(readTarget, membersChain[i]);
            }
            return WriteMember(membersChain[membersChain.Length - 1], readTarget, value);
        }

        public static IAstNode WriteMember(MemberInfo memberInfo, IAstRefOrAddr targetObject, IAstRefOrValue value)
        {
            if (memberInfo.MemberType == MemberTypes.Field)
            {
                return new AstWriteField()
                {
                    FieldInfo = (FieldInfo)memberInfo,
                    TargetObject = targetObject,
                    Value = value
                };
            }
            else
            {
                return new AstWriteProperty(targetObject, value, (PropertyInfo)memberInfo);
            }
        }
    }
}