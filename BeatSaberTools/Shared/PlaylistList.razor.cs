using System;
using System.Collections.Generic;
using BeatSaberTools.Models;
using BeatSaberTools.Services;
using Microsoft.AspNetCore.Components;

namespace BeatSaberTools.Shared
{
    public partial class PlaylistList
    {
        [Inject]
        protected PlaylistService PlaylistService { get; set; }

        [Inject]
        protected BeatSaberDataService BeatSaberDataService { get; set; }

        private IEnumerable<Playlist> Playlists = Array.Empty<Playlist>();
        private bool LoadingPlaylists = false;

        private Playlist SelectedPlaylist;
        private object SelectedPlaylistValue;

        protected override void OnInitialized()
        {
            PlaylistService.Playlists.Subscribe(playlists =>
            {
                Playlists = playlists;
                InvokeAsync(StateHasChanged);
            });

            BeatSaberDataService.LoadingPlaylistInfo.Subscribe(loading =>
            {
                LoadingPlaylists = loading;
                InvokeAsync(StateHasChanged);
            });

            PlaylistService.SelectedPlaylist.Subscribe(playlist =>
            {
                SelectedPlaylist = playlist;
                SelectedPlaylistValue = SelectedPlaylist;
                InvokeAsync(StateHasChanged);
            });
        }

        protected void OnPlaylistSelect(Playlist playlist)
        {
            PlaylistService.SetSelectedPlaylist(playlist);
        }
    }
}