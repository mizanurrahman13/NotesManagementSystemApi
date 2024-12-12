namespace NOTEKEEPER.Api.Models;

public class JwtSetting
{
    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public TimeSpan TokenLifetime { get; set; }

    public string[] CorsOrigins { get; set; } = Array.Empty<string>();
}
