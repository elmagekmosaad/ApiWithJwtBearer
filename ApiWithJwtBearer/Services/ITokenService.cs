using System.Security.Claims;

namespace ApiWithJwtBearer.Services
{
    public interface ITokenService
    {
        object GenerateToken(List<Claim> claims);
    }
}