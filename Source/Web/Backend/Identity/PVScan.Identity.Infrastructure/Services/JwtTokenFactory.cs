using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PVScan.Identity.Infrastructure.Data.Repositories.Interfaces;
using PVScan.Identity.Domain.Entities;
using PVScan.Identity.Infrastructure.Configurations;
using PVScan.Shared;
using PVScan.Shared.Configurations;
using PVScan.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace PVScan.Identity.Infrastructure.Services
{
    public class JwtTokenFactory : IJwtTokenFactory
    {
        private readonly IdentitySettings _identitySettings;
        private readonly SharedIdentitySettings _sharedIdentitySettings;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        
        private RsaSecurityKey? _privateKey = null;

        public JwtTokenFactory(
            IOptions<IdentitySettings> identitySettingsOptions,
            IOptions<SharedIdentitySettings> sharedIdentitySettingsOptions, 
            IServiceScopeFactory serviceScopeFactory)
        {
            _identitySettings = identitySettingsOptions.Value;
            _sharedIdentitySettings = sharedIdentitySettingsOptions.Value;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task<string> GenerateAccessTokenAsync(AccessTokenGenerationData data)
        {
            EnsurePrivateKey();

            var now = DateTime.UtcNow;

            var tokenHandler = new JwtSecurityTokenHandler();

            var signingCredentials = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256Signature);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, data.User.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, JsonSerializer.Serialize(now)),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = signingCredentials,
                Subject = new ClaimsIdentity(claims),
                NotBefore = now,
                Expires = now.Add(_identitySettings.AccessTokenLifetime),
                Audience = _sharedIdentitySettings.Audience,
                Issuer = _sharedIdentitySettings.Issuer,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Task.FromResult(tokenHandler.WriteToken(token));
        }

        private void EnsurePrivateKey()
        {
            if (_privateKey is not null) return;

            var privateKeyProvider = RsaExtensions.ParsePrivateKey(_identitySettings.PrivateKeyPath);
            _privateKey = new RsaSecurityKey(privateKeyProvider);
        }

        public async Task<RefreshToken> GenerateRefreshTokenAsync(RefreshTokenGenerationData data)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var refreshTokenRepository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

            string uniqueTokenValue = await GenerateUniqueRefreshToken(refreshTokenRepository);

            var now = DateTime.UtcNow;

            var expires = now.Add(_identitySettings.RefreshTokenLifetime);

            var toAdd = new RefreshToken(Guid.NewGuid(), uniqueTokenValue,
                expires, now, data.FromIp);

            await refreshTokenRepository.AddAsync(toAdd);

            return toAdd;
        }

        private async Task<string> GenerateUniqueRefreshToken(IRefreshTokenRepository refreshTokenRepository)
        {
            var uniqueTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));

            var alreadyExists = await refreshTokenRepository.Query().AnyAsync(a => a.Token == uniqueTokenValue);

            while (alreadyExists)
            {
                uniqueTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));

                alreadyExists = await refreshTokenRepository.Query().AnyAsync(a => a.Token == uniqueTokenValue);
            }

            return uniqueTokenValue;
        }
    }
}
