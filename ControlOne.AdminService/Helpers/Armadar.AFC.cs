using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Armadar.Helpers
{
    public class AFC
    {
        public static void setCustomError(AuthenticationFailedContext context)
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";

            var err = "";

            if (context.Exception.GetType() == typeof(SecurityTokenValidationException))
            {
                err = "invalid token";
            }
            else if (context.Exception.GetType() == typeof(SecurityTokenInvalidIssuerException))
            {
                err = "invalid issuer";
            }
            else if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                err = "token expired";
            }
            else if (context.Exception.GetType() == typeof(SecurityTokenInvalidSignatureException))
            {
                err = "invalid signature";
            }

            var resp = new
            {
                code = context.Response.StatusCode,
                message = err,
            };

            context.Response.WriteAsync(JsonConvert.SerializeObject(resp, Formatting.Indented));
            //context.Response.WriteAsync(context.Exception.ToString()).Wait();
        }
    }
}