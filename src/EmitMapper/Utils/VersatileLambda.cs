using System.Linq.Expressions;

namespace EmitMapper.Utils;

/// <summary>
///   The versatile lambda.
/// </summary>
/// <typeparam name="T"></typeparam>
public class VersatileLambda<T>
  where T : class
{
	private readonly Expression<T> expression;

	private readonly Lazy<T> funcLazy;

	/// <summary>
	///   Initializes a new instance of the <see cref="VersatileLambda&lt;T&gt;" /> class.
	/// </summary>
	/// <param name="expression">The expression.</param>
	public VersatileLambda(Expression<T> expression)
	{
		ArgumentNullException.ThrowIfNull(expression);

		this.expression = expression;
		funcLazy = new Lazy<T>(expression.Compile);
	}

	public static implicit operator Expression<T>(VersatileLambda<T> lambda)
	{
		return lambda?.expression;
	}

	public static implicit operator T(VersatileLambda<T> lambda)
	{
		return lambda?.funcLazy.Value;
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