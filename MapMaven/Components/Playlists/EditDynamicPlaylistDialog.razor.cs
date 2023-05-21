using MapMaven.Core.Models.DynamicPlaylists;
using MapMaven.Core.Services;
using MapMaven.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SortDirection = MapMaven.Core.Models.DynamicPlaylists.SortDirection;

namespace MapMaven.Components.Playlists
{
    public partial class EditDynamicPlaylistDialog
    {
        [Inject]
        protected PlaylistService PlaylistService { get; set; }
        [Inject]
        protected DynamicPlaylistArrangementService DynamicPlaylistArrangementService { get; set; }

        [Inject]
        ISnackbar Snackbar { get; set; }

        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        [Parameter]
        public EditDynamicPlaylistModel SelectedPlaylist { get; set; }

        [Parameter]
        public bool NewPlaylist { get; set; } = true;

        private static List<EditDynamicPlaylistModel> PresetDynamicPlaylists = new List<EditDynamicPlaylistModel>
        {
            new EditDynamicPlaylistModel
            {
                FileName = "RECENTLY_ADDED_MAPS",
                Name = "Recently added maps",
                Description = "The most recently downloaded maps.",
                DynamicPlaylistConfiguration = new DynamicPlaylistConfiguration
                {
                    MapPool = MapPool.Standard,
                    SortOperations = new()
                    {
                        new SortOperation
                        {
                            Field = "AddedDateTime",
                            Direction = SortDirection.Descending
                        }
                    },
                    MapCount = 20
                }
            },
            new EditDynamicPlaylistModel
            {
                FileName = "IMPROVEMENT_MAPS",
                Name = "Improvement maps",
                Description = "Maps recommended to play for maximum pp gain.",
                DynamicPlaylistConfiguration = new DynamicPlaylistConfiguration
                {
                    MapPool = MapPool.Improvement,
                    FilterOperations = new()
                    {
                        new FilterOperation
                        {
                            Field = "Hidden",
                            Operator = Core.Models.DynamicPlaylists.FilterOperator.Equals,
                            Value = false.ToString()
                        }
                    },
                    SortOperations = new()
                    {
                        new SortOperation
                        {
                            Field = "ScoreEstimate.PPIncrease",
                            Direction = SortDirection.Descending
                        }
                    },
                    MapCount = 20
                }
            }
        };

        void ConfigureDynamicPlaylist(EditDynamicPlaylistModel dynamicPlaylist)
        {
            SelectedPlaylist = dynamicPlaylist;
        }

        void ConfigureCustomDynamicPlaylist()
        {
            SelectedPlaylist = new()
            {
                FileName = $"CUSTOM_PLAYLIST_TEST_{Guid.NewGuid()}",
                Name = $"Custom Playlist Test {Guid.NewGuid()}",
                Description = "test",
                DynamicPlaylistConfiguration = new()
                {
                    MapCount = 20
                },
            };
        }

        void AddFilterOperation()
        {
            SelectedPlaylist.DynamicPlaylistConfiguration.FilterOperations.Add(new());
        }

        void RemoveFilterOperation(FilterOperation filterOperation)
        {
            SelectedPlaylist.DynamicPlaylistConfiguration.FilterOperations.Remove(filterOperation);
        }

        async Task OnValidSubmit()
        {
            if (NewPlaylist)
            {
                await PlaylistService.AddDynamicPlaylist(SelectedPlaylist);
                Snackbar.Add($"Added playlist \"{SelectedPlaylist.Name}\"", Severity.Normal, config => config.Icon = Icons.Filled.Check);
            }
            else
            {
                await PlaylistService.EditDynamicPlaylist(SelectedPlaylist);
                Snackbar.Add($"Saved playlist \"{SelectedPlaylist.Name}\"", Severity.Normal, config => config.Icon = Icons.Filled.Check);
            }
            
            Task.Run(DynamicPlaylistArrangementService.ArrangeDynamicPlaylists);

            MudDialog.Close(DialogResult.Ok(SelectedPlaylist));
        }

        void Cancel() => MudDialog.Cancel();
        void CancelConfiguration()
        {
            if (NewPlaylist)
            {
                SelectedPlaylist = null;
            }
            else
            {
                Cancel();
            }
        }
    }
}