using System.Linq.Expressions;

namespace EmitMapper.Utils;

/// <summary>
///   The versatile lambda.
/// </summary>
/// <typeparam name="T"></typeparam>
public class VersatileLambda<T>
  where T : class
{
	private readonly Expression<T> _expression;

	private readonly Lazy<T> _funcLazy;

	/// <summary>
	///   Initializes a new instance of the <see cref="VersatileLambda&lt;T&gt;" /> class.
	/// </summary>
	/// <param name="expression">The expression.</param>
	public VersatileLambda(Expression<T> expression)
	{
		if (expression is null)
		{
			throw new ArgumentNullException(nameof(expression));
		}

		_expression = expression;
		_funcLazy = new Lazy<T>(expression.Compile);
	}

	public static implicit operator Expression<T>(VersatileLambda<T> lambda)
	{
		return lambda?._expression;
	}

	public static implicit operator T(VersatileLambda<T> lambda)
	{
		return lambda?._funcLazy.Value;
	}

	/// <summary>
	///   As the expression.
	/// </summary>
	/// <returns><![CDATA[Expression<T>]]></returns>
	public Expression<T> AsExpression()
	{
		return this;
	}

	/// <summary>
	///   As the lambda.
	/// </summary>
	/// <returns>A T.</returns>
	public T AsLambda()
	{
		return this;
	}
}