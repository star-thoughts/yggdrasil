using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using Yggdrasil.Auditing;
using Yggdrasil.Identity;
using Yggdrasil.Models;
using Yggdrasil.Server.Configuration;
using Yggdrasil.Server.Hubs;
using Yggdrasil.Server.Identity;
using Yggdrasil.Server.Storage;

namespace Yggdrasil.Server.Controllers
{
    /// <summary>
    /// Controller for managing campaigns
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ResponseCache(CacheProfileName = "Default")]
    public class CampaignsController : ControllerBase
    {
        /// <summary>
        /// Constructs a new <see cref="CampaignsController"/>
        /// </summary>
        /// <param name="logger">Interface used for logging purposes</param>
        /// <param name="storage">Interface to use for communicating with data storage</param>
        public CampaignsController(ILogger<CampaignsController> logger,
                                   UserManager<ApplicationUser> userManager,
                                   RoleManager<ApplicationRole> roleManager,
                                   ICampaignStorage storage,
                                   IOptions<JwtConfiguration> jwtConfig,
                                   IHubContext<ServiceHub> hub)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _storage = storage;
            _hub = hub;
            _jwtConfig = jwtConfig?.Value;
        }

        private readonly ILogger<CampaignsController> _logger;
        private readonly ICampaignStorage _storage;
        private readonly IHubContext<ServiceHub> _hub;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly JwtConfiguration _jwtConfig;

        #region Campaigns
        /// <summary>
        /// Gets a collection of all campaigns in the system
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCampaigns()
        {
            return Ok(new CampaignList()
            {
                Campaigns = (await _storage.GetCampaigns()).ToArray(),
            });
        }

        /// <summary>
        /// Creates a new campaign
        /// </summary>
        /// <param name="name">Name of the campaign to create</param>
        /// <param name="description">Description to use for the campaign</param>
        /// <returns>Result of the operation</returns>
        [HttpPost]
        [Authorize(Roles = Roles.CreateCampaign, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateCampaign([Required] string name, [Required] string description)
        {
            string user = HttpContext.User.Identity.Name;
            string campaignID = await _storage.CreateCampaign(user, name, description, HttpContext.RequestAborted);

            _ = _hub.CampaignAdded(user, new CampaignOverview() { Name = name, ShortDescription = description, DungeonMaster = user, ID = campaignID });

            _ = _storage.AddAuditRecord(new AuditRecord()
            {
                DateTime = DateTime.UtcNow,
                ItemID = campaignID,
                Action = AuditAction.Created,
                RecordType = AuditRecordTypes.Campaign,
                User = user,
                Variables = new Dictionary<string, string>()
                {
                    { AuditKeys.Name, name },
                    { AuditKeys.Description, description },
                },
            }, HttpContext.RequestAborted);

            return CreatedAtAction(nameof(OpenCampaign), new { campaignID }, new CreateCampaignResult() { ID = campaignID });
        }

        /// <summary>
        /// Deletes the given campaign
        /// </summary>
        /// <param name="campaignID">ID of the campaign to delete</param>
        /// <returns>Result of the operation</returns>
        /// <remarks>
        /// This operation is only allowed by someone with the <see cref="Roles.ManageCampaigns"/> role since it can delete any campaign.  For a dungeon
        /// master to delete a campaign, call <see cref="DeleteCurrentCampaign"/> instead.
        /// </remarks>
        [HttpDelete("{campaignID}")]
        [Authorize(Roles = Roles.ManageCampaigns, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteCampaign([Required] string campaignID)
        {
            string user = HttpContext.User.Identity.Name;
            await _storage.DeleteCampaign(campaignID, HttpContext.RequestAborted);

            _ = _hub.CampaignRemoved(user, campaignID);

            _ = _storage.AddAuditRecord(new AuditRecord()
            {
                DateTime = DateTime.UtcNow,
                ItemID = campaignID,
                Action = AuditAction.Removed,
                RecordType = AuditRecordTypes.Campaign,
                User = user,
                Variables = new Dictionary<string, string>(),
            }, HttpContext.RequestAborted);

            return NoContent();
        }


        /// <summary>
        /// Deletes the current campaign
        /// </summary>
        /// <returns>Result of the operation</returns>
        /// <remarks>
        /// This operation is only allowed by someone with the <see cref="Roles.ManageCampaigns"/> or <see cref="Roles.DungeonMaster"/> since it deletes only the
        /// user's current campaign.
        /// </remarks>
        [HttpDelete]
        [Authorize(Roles = Roles.DeleteCampaign, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteCampaign()
        {
            string user = HttpContext.User.Identity.Name;
            string campaignID = GetCampaignID(true);

            await _storage.DeleteCampaign(campaignID, HttpContext.RequestAborted);

            _ = _hub.CampaignRemoved(user, campaignID);

            _ = _storage.AddAuditRecord(new AuditRecord()
            {
                DateTime = DateTime.UtcNow,
                ItemID = campaignID,
                Action = AuditAction.Removed,
                RecordType = AuditRecordTypes.Campaign,
                User = user,
                Variables = new Dictionary<string, string>(),
            }, HttpContext.RequestAborted);

            return NoContent();
        }

        [HttpGet("{campaignID}/token")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> OpenCampaign([Required] string campaignID)
        {
            GetCampaignResult campaign = await _storage.GetCampaign(campaignID, HttpContext.RequestAborted);

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ApplicationRole[] roles = _roleManager.Roles.ToArray();
            string[] campaignRoles = campaign.Users.FirstOrDefault(p => string.Equals(p.UserName, user.UserName, StringComparison.OrdinalIgnoreCase))
                ?.Roles
                .Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

            OpenCampaignResult result = new OpenCampaignResult();
            result.CamapignToken = AuthenticationHelper.GenerateCampaignJwtToken(user, _jwtConfig, campaignID, roles, campaignRoles);

            return Ok(result);
        }
        /// <summary>
        /// Gets the campaign with the given ID, and creates a token for the user with the campaign information forthe user
        /// </summary>
        /// <param name="campaignID">ID of the campaign to get</param>
        /// <returns>Campaign information</returns>
        [HttpGet("overview")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCampaign()
        {
            string campaignID = GetCampaignID();

            GetCampaignResult result = await _storage.GetCampaign(campaignID, HttpContext.RequestAborted);

            return Ok(result);
        }

        /// <summary>
        /// Gets the users for a campaign
        /// </summary>
        /// <returns>users for a campaign</returns>
        [HttpGet("users")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUsersForCampaign()
        {
            string campaignID = GetCampaignID();

            return Ok(await _storage.GetCampaignUsers(campaignID, HttpContext.RequestAborted));
        }
        #endregion
        #region Player Characters
        /// <summary>
        /// Gets the player characters for a campaign
        /// </summary>
        /// <returns></returns>
        [HttpGet("playercharacters")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetPlayerCharacters()
        {
            string campaignID = GetCampaignID();

            return Ok(await _storage.GetPlayerCharacters(campaignID, HttpContext.RequestAborted));
        }

        /// <summary>
        /// Claims a character for the calling user
        /// </summary>
        /// <returns>Result of the operation</returns>
        [HttpPost("playercharacters/claim/{characterID}")]
        [Authorize(Roles = Roles.Player, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ClaimCharacter([Required] string characterID)
        {
            string campaignID = GetCampaignID();

            await _storage.ClaimCharacter(campaignID, characterID, HttpContext.User.Identity.Name, HttpContext.RequestAborted);

            _ = _hub.PlayerCharacterUpdated(campaignID, HttpContext.User.Identity.Name, await _storage.GetPlayerCharacter(campaignID, characterID, HttpContext.RequestAborted));

            return NoContent();
        }

        [HttpPut("playercharacters")]
        [Authorize(Roles = Roles.CampaignRoles, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdatePlayerCharacter([Required] CampaignPlayerCharacter character)
        {
            string campaignID = GetCampaignID();

            await _storage.UpdatePlayerCharacter(campaignID, character, HttpContext.RequestAborted);

            _ = _hub.PlayerCharacterUpdated(campaignID, HttpContext.User.Identity.Name, character);

            return NoContent();
        }

        [HttpPost("playercharacters")]
        [Authorize(Roles = Roles.CampaignRoles, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreatePlayerCharacter([Required] CampaignPlayerCharacter character)
        {
            if (character == null)
                throw new ArgumentNullException(nameof(character));

            string campaignID = GetCampaignID();

            character.ID = await _storage.CreatePlayerCharacter(campaignID, character, HttpContext.RequestAborted);

            _ = _hub.PlayerCharacterAdded(campaignID, HttpContext.User.Identity.Name, character);

            return NoContent();
        }

        [HttpDelete("playercharacters/{id}")]
        [Authorize(Roles = Roles.DungeonMaster, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RemovePlayerCharacter([Required] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            string campaignID = GetCampaignID();

            await _storage.RemoveCharacter(campaignID, id, HttpContext.RequestAborted);

            _ = _hub.PlayerCharacterRemoved(campaignID, HttpContext.User.Identity.Name, id);

            return NoContent();
        }
        #endregion
        #region Helpers
        /// <summary>
        /// Gets the ID of the campaign from the user's claims
        /// </summary>
        /// <returns>ID of the campaign, or null or empty if one is not found</returns>
        string GetCampaignID(bool required = true)
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                string campaignID = identity.Claims.FirstOrDefault(p => string.Equals(p.Type, "campaign", StringComparison.Ordinal))?.Value;
                if (required && string.IsNullOrWhiteSpace(campaignID))
                    throw new ItemNotFoundException(ItemType.Campaign, string.Empty);

                return campaignID;
            }
            return null;
        }
        #endregion
    }
}
