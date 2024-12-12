using Microsoft.Extensions.Options;

namespace NOTEKEEPER.Api.Models;

public class JwtSettingSetup : IConfigureOptions<JwtSetting>
{
    private readonly IConfiguration _configuration;

    public JwtSettingSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(JwtSetting options)
    {
        _configuration.GetSection(nameof(JwtSetting)).Bind(options);
    }
}