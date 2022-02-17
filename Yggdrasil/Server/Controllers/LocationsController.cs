using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Yggdrasil.Identity;
using Yggdrasil.Models.Locations;
using Yggdrasil.Server.Configuration;
using Yggdrasil.Server.Hubs;
using Yggdrasil.Server.Services;

namespace Yggdrasil.Server.Controllers
{
    /// <summary>
    /// Controller for managing campaigns
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ResponseCache(CacheProfileName = "Default")]
    public class LocationsController : ControllerBase
    {
        /// <summary>
        /// Constructs a new <see cref="LocationsController"/>
        /// </summary>
        /// <param name="logger">Logger to use for outputting log information</param>
        /// <param name="service">Service to use for managing locations</param>
        public LocationsController(ILogger<CampaignsController> logger,
            ILocationsService service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _locationsService = service ?? throw new ArgumentNullException(nameof(service));
        }

        private readonly ILogger _logger;
        private readonly ILocationsService _locationsService;

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.DungeonMaster)]
        public async Task<LocationsList> GetRootLocations(string campaignId)
        {
            return await _locationsService.GetRootLocations(campaignId, HttpContext.RequestAborted);
        }
    }
}
