using Microsoft.AspNetCore.Components;
using MapMaven.Services;
using System.Reactive.Linq;
using Map = MapMaven.Models.Map;
using MudBlazor;
using MapMaven.Models;
using MapMaven.Core.Services;
using MapMaven.Components.Playlists;
using MapMaven.Components.Shared;

namespace MapMaven.Components.Maps
{
    public partial class MapBrowserRow : IDisposable
    {
        [Inject]
        protected BeatSaberDataService BeatSaberDataService { get; set; }
        [Inject]
        protected PlaylistService PlaylistService { get; set; }
        [Inject]
        protected ScoreSaberService ScoreSaberService { get; set; }
        [Inject]
        protected MapService MapService { get; set; }

        [Inject]
        protected IDialogService DialogService { get; set; }

        [Inject]
        ISnackbar Snackbar { get; set; }

        protected Playlist SelectedPlaylist { get; set; }

        protected string CoverImageUrl { get; set; }

        public string? PlayerId { get; set; } = null;

        IDisposable SelectedPlaylistSubscription;

        protected override void OnInitialized()
        {
            SelectedPlaylistSubscription = SubscribeAndBind(PlaylistService.SelectedPlaylist, selectedPlaylist => SelectedPlaylist = selectedPlaylist);

            SubscribeAndBind(ScoreSaberService.PlayerIdObservable, playerId => PlayerId = playerId);
        }

        async Task OpenAddMapToPlaylistDialog(Map map)
        {
            var dialog = await DialogService.ShowAsync<PlaylistSelector>("Add map to playlist", new DialogOptions
            {
                MaxWidth = MaxWidth.ExtraSmall,
                FullWidth = true,
                CloseButton = true
            });

            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                var playlist = (Playlist)result.Data;

                await PlaylistService.AddMapToPlaylist(map, playlist);

                Snackbar.Add($"Added map \"{map.Name}\" to \"{playlist.Title}\"", Severity.Normal, config => config.Icon = Icons.Filled.Check);
            }
        }

        async Task OpenDeleteFromPlaylistDialog(Map map)
        {
            var dialog = DialogService.Show<ConfirmationDialog>(null, new DialogParameters
            {
                { nameof(ConfirmationDialog.DialogText), $"Are you sure you want to remove \"{map.Name}\" from the \"{SelectedPlaylist.Title}\" playlist?" },
                { nameof(ConfirmationDialog.ConfirmText), $"Remove" }
            });

            var result = await dialog.Result;

            if (!result.Cancelled)
                await RemoveFromPlaylist(map);
        }

        async Task RemoveFromPlaylist(Map map)
        {
            await PlaylistService.RemoveMapFromPlaylist(map, SelectedPlaylist);

            Snackbar.Add($"Removed map \"{map.Name}\" from playlist \"{SelectedPlaylist.Title}\"", Severity.Normal, config => config.Icon = Icons.Filled.Check);
        }

        void OpenReplay(Map map)
        {
            var parameters = new DialogParameters
            {
                { nameof(Replay.Map), map }
            };

            DialogService.Show<Replay>(null, parameters, new DialogOptions
            {
                MaxWidth = MaxWidth.ExtraExtraLarge,
                FullWidth = true,
                CloseButton = true
            });
        }

        void SelectSongAuthor(Map map)
        {
            MapService.AddMapFilter(new Core.Models.MapFilter
            {
                Name = map.SongAuthorName,
                Filter = otherMap => map.SongAuthorName == otherMap.SongAuthorName
            });
        }

        public void Dispose()
        {
            SelectedPlaylistSubscription?.Dispose();
        }
    }
}