﻿using BeatSaberTools.Core.ApiClients;
using BeatSaberTools.Core.Models.Data.ScoreSaber;
using BeatSaberTools.Core.Utilities.Scoresaber;

namespace BeatSaberTools.Models
{
    public class Map
    {
        public string Id { get; set; }
        public string Hash { get; set; }
        public string Name { get; set; }
        public string SongAuthorName { get; set; }
        public string MapAuthorName { get; set; }
        public DateTime AddedDateTime { get; set; }
        public TimeSpan SongDuration { get; set; }
        public TimeSpan PreviewStartTime { get; set; }
        public TimeSpan PreviewDuration { get; set; }
        public TimeSpan PreviewEndTime => PreviewStartTime + PreviewDuration;

        public PlayerScore? PlayerScore { get; set; }
        public RankedMap? RankedMap { get; set; }
        public IEnumerable<ScoreEstimate> ScoreEstimate { get; set; }
    }
}
