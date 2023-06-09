using MapMaven.Components.Shared;
using MapMaven.Core.Models;
using MapMaven.Core.Models.Data;
using MapMaven.Models;
using MapMaven.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Map = MapMaven.Models.Map;

namespace MapMaven.Components.Maps
{
    public partial class ImprovementTweaker
    {
        [Inject]
        MapService MapService { get; set; }

        [Inject]
        PlaylistService PlaylistService { get; set; }

        [Inject]
        ISnackbar Snackbar { get; set; }

        [Parameter]
        public EventCallback<MapSelectionConfig> OnMapSelectionChanged { get; set; }

        string PlayedFilter = "Both";
        MapFilter PlayedMapFilter = null;

        string HiddenFilter = "Not hidden";
        MapFilter HiddenMapFilter = null;

        HashSet<Map> SelectedMaps = new();

        bool CreatingPlaylist = false;

        int MapSelectNumber = 10;
        int MapSelectStartFromNumber = 1;

        protected override void OnInitialized()
        {
            SubscribeAndBind(MapService.SelectedMaps, selectedMaps => SelectedMaps = selectedMaps);
            SubscribeAndBind(PlaylistService.CreatingPlaylist, creatingPlaylist => CreatingPlaylist = creatingPlaylist);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            OnHiddenFilterChanged(HiddenFilter);
        }

        void OnPlayedFilterChanged(string value)
        {
            PlayedFilter = value;

            if (PlayedMapFilter != null)
                MapService.RemoveMapFilter(PlayedMapFilter);

            if (PlayedFilter == "Both")
            {
                PlayedMapFilter = null;
                return;
            }

            PlayedMapFilter = new MapFilter
            {
                Type = MapFilterType.Played,
                Name = PlayedFilter,
                Visible = false
            };

            PlayedMapFilter.Filter = PlayedFilter switch
            {
                "Not played" => map => map.PlayerScore == null,
                "Played" => map => map.PlayerScore != null
            };

            MapService.AddMapFilter(PlayedMapFilter);
        }

        void OnHiddenFilterChanged(string value)
        {
            HiddenFilter = value;

            if (HiddenMapFilter != null)
                MapService.RemoveMapFilter(HiddenMapFilter);

            if (HiddenFilter == "Both")
            {
                HiddenMapFilter = null;
                return;
            }

            HiddenMapFilter = new MapFilter
            {
                Type = MapFilterType.Played,
                Name = HiddenFilter,
                Visible = false
            };

            HiddenMapFilter.Filter = HiddenFilter switch
            {
                "Not hidden" => map => !map.Hidden,
                "Hidden" => map => map.Hidden
            };

            MapService.AddMapFilter(HiddenMapFilter);
        }

        async Task CreatePlaylistFromSelectedMaps()
        {
            var subject = new BehaviorSubject<ItemProgress<Map>>(null);

            var cancellationToken = new CancellationTokenSource();

            var snackbar = Snackbar.Add<MapDownloadProgressMessage>(new Dictionary<string, object>
            {
                { nameof(MapDownloadProgressMessage.ProgressReport), subject.Sample(TimeSpan.FromSeconds(0.2)).AsObservable() },
                { nameof(MapDownloadProgressMessage.CreatingPlaylist), true },
                { nameof(MapDownloadProgressMessage.CancellationToken), cancellationToken },
            }, configure: config =>
            {
                config.RequireInteraction = true;
                config.ShowCloseIcon = false;
            });

            var playlistModel = new EditPlaylistModel
            {
                Name = $"Improvement Maps ({DateTime.Now:dd-MM-yyyy HH:mm:ss})",
                FileName = $"Improvement Maps ({DateTime.Now:dd-MM-yyyy_HH-mm-ss})"
            };

            var progress = new Progress<ItemProgress<Map>>(subject.OnNext);

            await PlaylistService.AddPlaylistAndDownloadMaps(playlistModel, SelectedMaps, progress: progress, cancellationToken: cancellationToken.Token);

            MapService.ClearSelectedMaps();

            Snackbar.Remove(snackbar);

            if (!cancellationToken.IsCancellationRequested)
            {
                Snackbar.Add($"Created playlist: {playlistModel.Name}", Severity.Normal, config => config.Icon = Icons.Material.Filled.Check);
            }
            else
            {
                Snackbar.Add($"Cancelled creating playlist.", Severity.Normal, config => config.Icon = Icons.Material.Filled.Cancel);
            }
        }

        void ClearSelection()
        {
            MapService.ClearSelectedMaps();
        }

        async Task ApplyMapSelection()
        {
            await OnMapSelectionChanged.InvokeAsync(new MapSelectionConfig
            {
                MapSelectNumber = MapSelectNumber,
                MapSelectStartFromNumber = MapSelectStartFromNumber
            });
        }
    }
}