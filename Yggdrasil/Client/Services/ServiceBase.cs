using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Yggdrasil.Client.Models;
using Yggdrasil.Client.Pages.Exceptions;

namespace Yggdrasil.Client.Services
{
    /// <summary>
    /// Base class for services with helper methods
    /// </summary>
    public abstract class ServiceBase
    {
        protected ServiceBase(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        private readonly IHttpClientFactory _factory;

        protected HttpClient GetClient()
        {
            return _factory.CreateClient("PublicAPI");
        }

        /// <summary>
        /// Checks an <see cref="HttpResponseMessage"/> for errors and throws exceptions when necessary
        /// </summary>
        /// <param name="response">Response message to test against</param>
        /// <returns>Task for asynchronous completion</returns>
        protected async Task CheckResponseForErrors(HttpResponseMessage response)
        {
            ProblemDetails details = await GetProblemDetails(response);

            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    ThrowUnauthorizedException(details);
                    break;
                case HttpStatusCode.NotFound:
                    ThrowNotFoundException(details);
                    break;
            }
            //  One last check for generic things that could have happened
            response.EnsureSuccessStatusCode();
        }

        private void ThrowNotFoundException(ProblemDetails details)
        {
            if (details != null)
                throw new ProblemException(details);
            throw new Exception();
        }

        private static void ThrowUnauthorizedException(ProblemDetails details)
        {
            if (details != null)
                throw new UnauthorizedException(details);
            throw new UnauthorizedException();
        }

        private async Task<ProblemDetails> GetProblemDetails(HttpResponseMessage response)
        {
            if (string.Equals(response.Content?.Headers?.ContentType?.MediaType, "application/problem+json", StringComparison.Ordinal))
            {
                ProblemDetails details = await Deserialize<ProblemDetails>(response);

                return details;
            }
            return null;
        }

        private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        protected async Task<T> Deserialize<T>(HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(cancellationToken), _serializerOptions, cancellationToken);
        }
    }
}