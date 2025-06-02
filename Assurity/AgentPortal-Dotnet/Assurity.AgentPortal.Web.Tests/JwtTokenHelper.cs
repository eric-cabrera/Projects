namespace Assurity.AgentPortal.Web.Tests;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Moq;

public static class JwtTokenHelper
{
    public static string GenerateJwtToken(IEnumerable<Claim> claims, string audience)
    {
        string issuer = Guid.NewGuid().ToString();
        SecurityKey securityKey;
        SigningCredentials signingCredentials;

        RandomNumberGenerator s_rng = RandomNumberGenerator.Create();

        byte[] s_key = new byte[32];

        s_rng.GetBytes(s_key);

        securityKey = new SymmetricSecurityKey(s_key) { KeyId = Guid.NewGuid().ToString() };

        signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityTokenHandler s_tokenHandler = new JwtSecurityTokenHandler();

        return s_tokenHandler.WriteToken(new JwtSecurityToken(issuer, audience, claims, null, DateTime.UtcNow.AddMinutes(20), signingCredentials));
    }

    public static Mock<IServiceProvider> GetMockServiceProviderWithAccessToken(ClaimsPrincipal claimsPrincipal, string mockAccessToken)
    {
        var authResult = AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, string.Empty));
        authResult.Properties.StoreTokens(new[]
        {
            new AuthenticationToken { Name = "access_token", Value = mockAccessToken }
        });

        var mockAuthenticationService = new Mock<IAuthenticationService>();
        mockAuthenticationService.Setup(service => service.AuthenticateAsync(It.IsAny<HttpContext>(), null)).ReturnsAsync(authResult);

        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider.Setup(provider => provider.GetService(typeof(IAuthenticationService))).Returns(mockAuthenticationService.Object);

        return mockServiceProvider;
    }
}
