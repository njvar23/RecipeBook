namespace RecipeBook.Api.Options;

/// <summary>Bound from the "Jwt" section of appsettings.json. Validated on startup.</summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiresInHours { get; set; } = 8;
}
