using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Threading.Tasks;
using Yggdrasil.Identity;
using Yggdrasil.Models.Locations;
using Yggdrasil.Server.Services;

namespace Yggdrasil.Server.Controllers
{
    /// <summary>
    /// Controller for managing campaigns
    /// </summary>
    [ApiController]
    [Route("api/campaigns/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ResponseCache(CacheProfileName = "Default")]
    public class LocationsController : ControllerCore
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

        /// <summary>
        /// Gets the root locations of the current campaign
        /// </summary>
        /// <returns>A list of locations that form the root of the world</returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.DungeonMaster)]
        public async Task<IActionResult> GetRootLocations()
        {
            return Ok(await _locationsService.GetRootLocations(GetCampaignID(), HttpContext.RequestAborted));
        }

        /// <summary>
        /// Gets details for the given location
        /// </summary>
        /// <param name="locationId">ID of the location to get</param>
        /// <returns>Location details</returns>
        [HttpGet("{locationId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.DungeonMaster)]
        public async Task<IActionResult> GetLocation([Required] string locationId)
        {
            return Ok(await _locationsService.GetLocation(GetCampaignID(), locationId, HttpContext.RequestAborted));
        }

        /// <summary>
        /// Adds a location to the currently loaded campaign
        /// </summary>
        /// <param name="location">Location data for the location to add to the campaign</param>
        /// <returns>Result of the operation, with the ID of the newly created location's ID</returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.DungeonMaster)]
        public async Task<IActionResult> AddLocation([Required] AddLocationData location)
        {
            if (string.IsNullOrWhiteSpace(location.Name))
                return BadRequest();

            string id = await _locationsService.AddLocation(
                GetCampaignID(),
                GetUserName(),
                location.Name,
                location.Description,
                location.ParentId,
                location.Population,
                location.Tags,
                HttpContext.RequestAborted);

            return Ok(new AddLocationResult() { Id = id });
        }

        /// <summary>
        /// Removes the given location from the campaign
        /// </summary>
        /// <param name="locationId">ID of the location to remove</param>
        /// <param name="relocateChildren">Whether or not to relocate children of this location to this location's parent</param>
        /// <returns>Result of the operation</returns>
        [HttpDelete("{locationId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.DungeonMaster)]
        public async Task DeleteLocation([Required] string locationId, bool relocateChildren = true)
        {
            await _locationsService.RemoveLocation(GetCampaignID(), GetUserName(), locationId, relocateChildren, HttpContext.RequestAborted);
        }

        /// <summary>
        /// Updates a given location
        /// </summary>
        /// <param name="locationId">ID of the location to update</param>
        /// <param name="data">Data to use to patch the location</param>
        /// <returns>Result of the operation</returns>
        [HttpPatch("{locationId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.DungeonMaster)]
        public async Task UpdateLocation([Required] string locationId, UpdateLocationData data)
        {
            await _locationsService.UpdateLocation(GetCampaignID(), GetUserName(), locationId, data.Name, data.Description, data.Population, data.Tags, HttpContext.RequestAborted);
        }
    }
}
