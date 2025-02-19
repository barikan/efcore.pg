using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite.Scaffolding.Internal;

public class NpgsqlNetTopologySuiteCodeGeneratorPlugin : ProviderCodeGeneratorPlugin
{
    private static readonly MethodInfo _useNetTopologySuiteMethodInfo
        = typeof(NpgsqlNetTopologySuiteDbContextOptionsBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(NpgsqlNetTopologySuiteDbContextOptionsBuilderExtensions.UseNetTopologySuite),
            typeof(NpgsqlDbContextOptionsBuilder),
            typeof(CoordinateSequenceFactory),
            typeof(PrecisionModel),
            typeof(Ordinates),
            typeof(bool));

    public override MethodCallCodeFragment GenerateProviderOptions()
        => new(_useNetTopologySuiteMethodInfo);
}