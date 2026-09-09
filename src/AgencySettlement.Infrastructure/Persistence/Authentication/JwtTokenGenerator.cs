using AgencySettlement.Application.Abstractions.Persistence.UserRepository;
using AgencySettlement.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AgencySettlement.Infrastructure.Persistence.Authentication
{
    public sealed class JwtTokenGenerator
     : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var jwt = _configuration
                .GetSection("Jwt");

            var key = jwt["Key"]
                ?? throw new InvalidOperationException(
                    "JWT Key is not configured.");

            var issuer = jwt["Issuer"];

            var audience = jwt["Audience"];

            var expiresInHours =
                jwt.GetValue<int?>("ExpiresInHours") ?? 8;

            var expires =
                DateTime.UtcNow.AddHours(expiresInHours);

            var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new(
                ClaimTypes.MobilePhone,
                user.PhoneNumber)
        };

            if (!string.IsNullOrWhiteSpace(user.FirstName))
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.GivenName,
                        user.FirstName));
            }

            if (!string.IsNullOrWhiteSpace(user.LastName))
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.Surname,
                        user.LastName));
            }

            var securityKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key));

            var credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256);

            var token =
                new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: expires,
                    signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}
