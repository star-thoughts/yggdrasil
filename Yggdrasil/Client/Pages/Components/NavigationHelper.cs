using Microsoft.AspNetCore.Components;
using System.Web;

namespace Yggdrasil.Client.Pages.Components
{
    /// <summary>
    /// Helper methods for navigating to pages
    /// </summary>
    static class NavigationHelper
    {
        /// <summary>
        /// Gets the address to use for viewing a location
        /// </summary>
        /// <param name="manager">Manager that may be used to navigate</param>
        /// <param name="locationID">ID of the location to navigate to</param>
        /// <returns>URI of the location</returns>
        public static string GetLocationAddress(this NavigationManager manager, string locationID)
        {
            return $"/location/{HttpUtility.UrlEncodeUnicode(locationID)}";
        }

        /// <summary>
        /// Gets the address to use for viewing the location list
        /// </summary>
        /// <param name="manager">Navigation manager that may e used to navigate</param>
        /// <returns>URI of the list</returns>
        public static string GetLocationsAddress(this NavigationManager manager)
        {
            return $"/locations";
        }

        /// <summary>
        /// Navigates to a location page to view a location
        /// </summary>
        /// <param name="manager">Navigation manager</param>
        /// <param name="locationID">ID of the location to navigate to</param>
        public static void ViewLocation(this NavigationManager manager, string locationID)
        {
            manager.NavigateTo(manager.GetLocationAddress(locationID));
        }
        /// <summary>
        /// Navigates to an edit location page
        /// </summary>
        /// <param name="manager">Navigation manager</param>
        /// <param name="locationID">ID of the location to navigate to</param>
        public static void EditLocation(this NavigationManager manager, string locationID)
        {
            manager.NavigateTo($"/editlocation/{HttpUtility.UrlEncodeUnicode(locationID)}");
        }

        /// <summary>
        /// Navigates to the list of locations
        /// </summary>
        /// <param name="manager">Navigation manager</param>
        public static void ViewLocationList(this NavigationManager manager)
        {
            manager.NavigateTo(manager.GetLocationsAddress());
        }
    }
}
