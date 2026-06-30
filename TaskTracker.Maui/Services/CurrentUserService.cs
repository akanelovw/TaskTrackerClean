using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskTracker.Maui.Interfaces;

namespace TaskTracker.Maui.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly TokenStore _tokenStore;

    private string? _cachedToken;
    private string? _userId;
    private string? _role;

    public CurrentUserService(TokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    public string? UserId
    {
        get { EnsureFresh(); return _userId; }
    }

    public string? Role
    {
        get { EnsureFresh(); return _role; }
    }

    public bool IsInRole(string role)
    {
        return string.Equals(Role, role, StringComparison.OrdinalIgnoreCase);
    }

    private void EnsureFresh()
    {
        var token = _tokenStore.Get();

        if (token == _cachedToken)
            return;

        _cachedToken = token;
        _userId = null;
        _role = null;

        if (string.IsNullOrWhiteSpace(token))
            return;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            _userId = jwt.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier)?.Value;

            _role = jwt.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Role)?.Value;
        }
        catch
        {
            _userId = null;
            _role = null;
        }
    }
}