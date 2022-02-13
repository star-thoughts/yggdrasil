using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Yggdrasil.Server.Identity;
using Yggdrasil.Server.Storage;

namespace Yggdrasil.Server.MiddleWare
{
    /// <summary>
    /// Middleware for handling standard exceptions
    /// </summary>
    public sealed class ExceptionHandlingMiddleware
    {
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        static ExceptionHandlingMiddleware()
        {
            _serializerOptions = new JsonSerializerOptions();
            _serializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        private readonly RequestDelegate _next;
        private static readonly JsonSerializerOptions _serializerOptions;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>")]
        public async Task InvokeAsync(HttpContext context)
        {
            ProblemDetails problem = null;

            try
            {
                await _next(context);
            }
            catch (ItemNotFoundException exc)
            {
                problem = new ProblemDetails()
                {
                    Detail = $"Item {exc.ItemID} could not be found of type {exc.ItemType}",
                    Status = (int)HttpStatusCode.NotFound,
                    Title = "Not Found",
                };
                problem.Extensions["errordetail"] = new { exc.ItemType, exc.ItemID };
            }
            catch (LockoutFailedException exc)
            {
                problem = new ProblemDetails()
                {
                    Detail = $"Failed to set/reset lock on account.",
                    Status = (int)HttpStatusCode.Conflict,
                    Title = "Conflict",
                };
                int pos = 0;
                foreach (IdentityError error in exc.Result.Errors)
                {
                    problem.Extensions[pos.ToString(CultureInfo.InvariantCulture)] = error.Description;
                    pos++;
                }
            }
            catch (LoginException exc)
            {
                problem = new ProblemDetails()
                {
                    Detail = exc.Message,
                    Status = (int)HttpStatusCode.Unauthorized,
                    Title = "Unauthorized",
                };
            }

            if (problem != null)
            {
                problem.Type = @"https://tools.ietf.org/html/rfc7231#section-6.6.1";
                problem.Extensions["traceid"] = Activity.Current?.Id ?? context.TraceIdentifier;
                problem.Extensions["connectionid"] = context.TraceIdentifier;

                context.Response.StatusCode = problem.Status.Value;
                context.Response.ContentType = "application/problem+json";
                await JsonSerializer.SerializeAsync(context.Response.Body, problem);
            }
        }

        public static ProblemDetails CreateProblemDetails(HttpContext context, HttpStatusCode statusCode, string title, string detail, Dictionary<string, string> extensions = null)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            ProblemDetails details = new ProblemDetails()
            {
                Type = @"https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Status = (int)statusCode,
                Detail = detail,
                Title = title,
            };

            details.Extensions["traceid"] = Activity.Current?.Id ?? context.TraceIdentifier;
            details.Extensions["connectionid"] = context.TraceIdentifier;

            if (extensions != null)
            {
                foreach (KeyValuePair<string, string> kvp in extensions)
                    details.Extensions[kvp.Key] = kvp.Value;
            }

            return details;
        }
    }
    /// <summary>
    /// Extension method for adding exception handler to pipeline
    /// </summary>
    public static class ExceptionHandlerMiddlewareExtensions
    {
        /// <summary>
        /// Adds the middleware to the pipeline
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
