using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Serverless
{
    public static class AuthorizationValidation
    {
        private static AsyncLazy<DiscoveryResponse> discoveryDocument = new AsyncLazy<DiscoveryResponse>(() =>
        {
            return DiscoveryClient.GetAsync(Environment.GetEnvironmentVariable("IdpAuthority"));
        });

        public static async Task<bool> CheckAuthorization(this HttpRequest req, string resource)
        {
            if (!req.Headers.ContainsKey("Authorization"))
            {
                return false;
            }

            var headerValue = req.Headers["Authorization"];
            var bearerToken = headerValue.ToString();
            bearerToken = bearerToken.Replace("Bearer ", String.Empty);

            if (String.IsNullOrWhiteSpace(bearerToken))
            {
                return false;
            }

            var principal = await ValidateJwt(bearerToken, resource);

            return principal != null;
        }

        private static async Task<ClaimsPrincipal> ValidateJwt(string jwt, string resource)
        {
            var disco = await discoveryDocument;
            var keys = new List<SecurityKey>();

            foreach (var webKey in disco.KeySet.Keys)
            {
                var e = Base64Url.Decode(webKey.E);
                var n = Base64Url.Decode(webKey.N);

                var key = new RsaSecurityKey(new RSAParameters { Exponent = e, Modulus = n })
                {
                    KeyId = webKey.Kid
                };

                keys.Add(key);
            }

            var parameters = new TokenValidationParameters
            {
                ValidIssuer = disco.Issuer,
                ValidAudience = resource,
                IssuerSigningKeys = keys,

                NameClaimType = "sub",
                RoleClaimType = JwtClaimTypes.Role
            };

            var handler = new JwtSecurityTokenHandler();
            handler.InboundClaimTypeMap.Clear();

            ClaimsPrincipal user = null;

            try
            {
                user = handler.ValidateToken(jwt, parameters, out var _);
            }
            catch
            {
            }

            Thread.CurrentPrincipal = user;

            return user;
        }

        /*
        private static async Task<ClaimsPrincipal> ValidateToken(string jwtToken, string issuer, string resource)
        {
            var introspectionClient = new IntrospectionClient(
                Environment.GetEnvironmentVariable("IdpIntrospectionEndpoint"),
                Environment.GetEnvironmentVariable("IdpIntrospectionEndpointClientId"),
                Environment.GetEnvironmentVariable("IdpIntrospectionEndpointClientSecret"));

            var response = await introspectionClient.SendAsync(
                new IntrospectionRequest { Token = jwtToken });

            if (!response.IsActive)
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            handler.InboundClaimTypeMap.Clear();

            var principal = handler.ValidateToken(jwtToken, new TokenValidationParameters()
            {
                ValidIssuer = issuer,
                ValidAudience = resource,
                ValidateIssuerSigningKey = false,
                SignatureValidator = (t, param) => new JwtSecurityToken(t),
                NameClaimType = "sub"

            }, out SecurityToken _);

            Thread.CurrentPrincipal = principal;

            return principal;
        }
        */
    }
}
