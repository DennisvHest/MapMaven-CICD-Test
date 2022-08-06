using BeatSaberTools.Services;
using Microsoft.AspNetCore.Components;

namespace BeatSaberTools.Shared
{
    public partial class SideNav
    {
        [Inject]
        protected PlaylistService PlaylistService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected void OnMapsNavigation()
        {
            PlaylistService.SetSelectedPlaylist(null);
            NavigationManager.NavigateTo("/");
        }
    }
}