using System.Linq;
using System.Reflection;

namespace EmitMapper.Utils;

public readonly struct ConstructorParameters
{
    public readonly ConstructorInfo Constructor;
    public readonly ParameterInfo[] Parameters;

    public ConstructorParameters(ConstructorInfo constructor)
    {
        Constructor = constructor;
        Parameters = constructor.GetParameters();
    }

    public int ParametersCount => Parameters.Length;

    public bool AllParametersOptional()
    {
        return Parameters.All(p => p.IsOptional);
    }
}