using System.Linq.Expressions;

namespace EmitMapper;

/// <summary>Output the constant to C# string or should return `null`</summary>
public delegate string TryPrintConstant(ConstantExpression e);