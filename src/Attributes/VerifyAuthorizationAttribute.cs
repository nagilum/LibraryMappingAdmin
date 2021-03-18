using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using DotNetLibraryAdmin.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotNetLibraryAdmin.Attributes
{
    public class VerifyAuthorizationAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Validate authorization of user.
        /// </summary>
        /// <param name="context">Current executing context.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Request.Headers.Keys.Contains("Authorization")
                ? context.HttpContext.Request.Headers["Authorization"].ToString()
                : null;

            if (token == null)
            {
                this.ReturnUnauthorized(context);
                return;
            }

            var tokens = token.Split(' ');

            if (tokens.Length != 2 ||
                tokens[0] != "Bearer")
            {
                this.ReturnUnauthorized(context);
                return;
            }

            token = tokens[1];

            try
            {
                var user = new DatabaseContext()
                    .Users
                    .ToList()
                    .FirstOrDefault(n => n.GetToken() == token);

                if (user == null)
                {
                    throw new Exception("User not found.");
                }
            }
            catch (Exception ex)
            {
                this.ReturnUnauthorized(context, ex.Message);
            }
        }

        /// <summary>
        /// Return an empty unauthorized response.
        /// </summary>
        /// <param name="context">Current executing context.</param>
        /// <param name="content">Object to serialize back to client.</param>
        private void ReturnUnauthorized(ActionExecutingContext context, object content = null)
        {
            content ??= new object();

            context.Result = new ContentResult
            {
                Content = JsonSerializer.Serialize(content),
                ContentType = "application/json"
            };

            context.HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
        }
    }
}