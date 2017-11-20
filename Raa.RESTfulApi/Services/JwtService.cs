using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Raa.RESTfulApi.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Raa.RESTfulApi.Services
{
    public interface IJwtService
    {
        JwtResult CreateToken(ApplicationUser user);
    }
    public class JwtService : IJwtService
    {
        private JwtSettings _settings;

        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        private SymmetricSecurityKey _symmetricSecurityKey;
        private SigningCredentials _signingCredentials;



        public JwtService(IOptions<JwtSettings> settings)
        {

            _settings = settings.Value;

            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            _signingCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        }
        public JwtResult CreateToken(ApplicationUser user)
        {
            var claims = GetValidClaims(user);

            var jwtToken = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
                signingCredentials: _signingCredentials
            );

            var token = _jwtSecurityTokenHandler.WriteToken(jwtToken);

            return new JwtResult()
            {
                Token = token,
                Expires = jwtToken.ValidTo
            };

        }

        private List<Claim> GetValidClaims(ApplicationUser user)
        {

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            };

            var userRoles = user.Roles;
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("roles", userRole));
            }
            return claims;
        }

    }
}
