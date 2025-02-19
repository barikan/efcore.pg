using System.Data.Common;

namespace Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

/// <summary>
/// Type mapping for the PostgreSQL 'character' data type. Handles both CLR strings and chars.
/// </summary>
/// <remarks>
/// See: https://www.postgresql.org/docs/current/static/datatype-character.html
/// </remarks>
/// <inheritdoc />
public class NpgsqlCharacterStringTypeMapping : NpgsqlStringTypeMapping
{
    /// <summary>
    /// Static <see cref="ValueComparer{T}"/> for fixed-width character types.
    /// </summary>
    /// <remarks>
    /// <p>
    /// Comparisons of 'character' data as defined in the SQL standard differ dramatically from CLR string
    /// comparisons. This value comparer adjusts for this by only comparing strings after truncating trailing
    /// whitespace.
    /// </p>
    /// <p>
    /// Note that if a value converter is used and the CLR type isn't a string at all, we just use the default
    /// value converter instead.
    /// </p>
    /// </remarks>
    private static readonly ValueComparer<string> CharacterValueComparer =
        new(
            (x, y) => EqualsWithoutTrailingWhitespace(x, y),
            x => GetHashCodeWithoutTrailingWhitespace(x));

    public override ValueComparer Comparer => ClrType == typeof(string) ? CharacterValueComparer : base.Comparer;

    public override ValueComparer KeyComparer => Comparer;

    public NpgsqlCharacterStringTypeMapping(string storeType, int size = 1)
        : this(new RelationalTypeMappingParameters(
            new CoreTypeMappingParameters(typeof(string)),
            storeType,
            StoreTypePostfix.Size,
            System.Data.DbType.StringFixedLength,
            unicode: false,
            size,
            fixedLength: true)) {}

    protected NpgsqlCharacterStringTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters, NpgsqlDbType.Char)
    {
    }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new NpgsqlCharacterStringTypeMapping(new RelationalTypeMappingParameters(
            parameters.CoreParameters,
            parameters.StoreType,
            StoreTypePostfix.Size,
            parameters.DbType,
            parameters.Unicode,
            parameters.Size,
            parameters.FixedLength,
            parameters.Precision,
            parameters.Scale));

    protected override void ConfigureParameter(DbParameter parameter)
    {
        if (parameter.Value is string value)
        {
            parameter.Value = value.TrimEnd();
        }

        base.ConfigureParameter(parameter);
    }

    private static bool EqualsWithoutTrailingWhitespace(string? a, string? b)
        => (a, b) switch
        {
            (null, null) => true,
            (_, null) => false,
            (null, _) => false,
            _ => a.AsSpan().TrimEnd().SequenceEqual(b.AsSpan().TrimEnd())
        };

    private static int GetHashCodeWithoutTrailingWhitespace(string a)
        => a.TrimEnd().GetHashCode();
}