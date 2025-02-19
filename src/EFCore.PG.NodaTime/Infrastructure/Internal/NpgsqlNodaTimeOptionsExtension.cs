using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal;

// ReSharper disable once CheckNamespace
namespace Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

public class NpgsqlNodaTimeOptionsExtension : IDbContextOptionsExtension
{
    private DbContextOptionsExtensionInfo? _info;

    public virtual void ApplyServices(IServiceCollection services)
        => services.AddEntityFrameworkNpgsqlNodaTime();

    public virtual DbContextOptionsExtensionInfo Info
        => _info ??= new ExtensionInfo(this);

    public virtual void Validate(IDbContextOptions options)
    {
        var internalServiceProvider = options.FindExtension<CoreOptionsExtension>()?.InternalServiceProvider;
        if (internalServiceProvider is not null)
        {
            using (var scope = internalServiceProvider.CreateScope())
            {
                if (scope.ServiceProvider.GetService<IEnumerable<IRelationalTypeMappingSourcePlugin>>()
                        ?.Any(s => s is NpgsqlNodaTimeTypeMappingSourcePlugin) != true)
                {
                    throw new InvalidOperationException($"{nameof(NpgsqlNodaTimeDbContextOptionsBuilderExtensions.UseNodaTime)} requires {nameof(NpgsqlNodaTimeServiceCollectionExtensions.AddEntityFrameworkNpgsqlNodaTime)} to be called on the internal service provider used.");
                }
            }
        }
    }

    private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
    {
        public ExtensionInfo(IDbContextOptionsExtension extension)
            : base(extension)
        {
        }

        private new NpgsqlNodaTimeOptionsExtension Extension
            => (NpgsqlNodaTimeOptionsExtension)base.Extension;

        public override bool IsDatabaseProvider => false;

        public override int GetServiceProviderHashCode() => 0;

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            => true;

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            => debugInfo["Npgsql:" + nameof(NpgsqlNodaTimeDbContextOptionsBuilderExtensions.UseNodaTime)] = "1";

        public override string LogFragment => "using NodaTime ";
    }
}