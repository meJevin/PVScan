using PVScan.Identity.Domain.Entities;

namespace PVScan.Identity.Infrastructure.Services
{
    public interface IJwtTokenFactory
    {
        Task<RefreshToken> GenerateRefreshTokenAsync(RefreshTokenGenerationData data);
        Task<string> GenerateAccessTokenAsync(AccessTokenGenerationData data);
    }

    public record RefreshTokenGenerationData(string? FromIp = null);
    public record AccessTokenGenerationData(User User);
}
