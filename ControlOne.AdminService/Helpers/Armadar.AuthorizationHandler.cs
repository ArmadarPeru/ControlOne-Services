using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Armadar.Helpers
{
    public class CustomAuthorizationHandler : IAuthorizationHandler
    {

        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            try
            {
                var authFilterCtx = (Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext)context.Resource;
                string authHeader = authFilterCtx.HttpContext.Request.Headers["Authorization"];
                if (authHeader == null || !authHeader.Contains("Bearer"))
                {
                    //var token = authHeader.Replace("Bearer", "");
                    // Now token can be used for further authorization

                    var filterContext = context.Resource as AuthorizationFilterContext;
                    var response = filterContext?.HttpContext.Response;

                    response.StatusCode = 403;
                    response.ContentType = new MediaTypeHeaderValue("application/json").ToString();

                    var missingToken = new { code = 403, message = "Missing Token" };
                    //string jsonString = JsonConvert.SerializeObject(missingToken);

                    response.WriteAsync(JsonConvert.SerializeObject(missingToken, Formatting.Indented));
                    //response.WriteAsync(jsonString, Encoding.UTF8);

                    return Task.CompletedTask;
                }
                else
                {
                    return Task.CompletedTask;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class CustomUnauthorizedResult : JsonResult
    {
        public CustomUnauthorizedResult(string message)
            : base(new CustomError(message))
        {
            StatusCode = StatusCodes.Status401Unauthorized;
        }
    }

    public class CustomError
    {
        public string Error { get; }

        public CustomError(string message)
        {
            Error = message;
        }
    }
}
