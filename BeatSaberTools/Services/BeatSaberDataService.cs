﻿using BeatSaberTools.Models.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing;
using NVorbis;

namespace BeatSaberTools.Services
{
    public class BeatSaberDataService
    {
        private const string BeatSaberInstallLocation = @"E:/Games/SteamLibrary/steamapps/common/Beat Saber";
        private const string MapsLocation = $"{BeatSaberInstallLocation}/Beat Saber_Data/CustomLevels";

        private readonly Regex _mapIdRegex = new Regex(@"^[0-9A-Fa-f]{4}");

        private readonly BehaviorSubject<Dictionary<string, MapInfo>> _mapInfo = new(new Dictionary<string, MapInfo>());
        private readonly BehaviorSubject<bool> _loadingMapInfo = new(false);

        public IObservable<IEnumerable<MapInfo>> MapInfo => _mapInfo.Select(x => x.Values);
        public IObservable<bool> LoadingMapInfo => _loadingMapInfo;

        public async Task LoadAllMapInfo()
        {
            _loadingMapInfo.OnNext(true);

            try
            {
                var fileReadTasks = Directory.EnumerateDirectories(MapsLocation)
                    .Take(10)
                    .Select(async mapDirectory =>
                    {
                        var infoFilePath = Path.Combine(mapDirectory, "Info.dat");
                        var mapInfoText = await File.ReadAllTextAsync(infoFilePath);

                        var info = JsonSerializer.Deserialize<MapInfo>(mapInfoText);
                        info.DirectoryPath = mapDirectory;

                        var directoryName = Path.GetFileName(Path.GetDirectoryName(info.DirectoryPath + "/"));

                        info.Id = _mapIdRegex.Match(directoryName)?.Value;

                        if (string.IsNullOrEmpty(info.Id))
                            return null;

                        FillSongInfo(info);

                        return info;
                    });

                IEnumerable<MapInfo> mapInfo = await Task.WhenAll(fileReadTasks);

                var mapInfoDictionary = mapInfo
                    .Where(i => i != null)
                    .GroupBy(i => i.Id)
                    .Select(g => g.First())
                    .ToDictionary(i => i.Id);

                _mapInfo.OnNext(mapInfoDictionary);
            }
            finally
            {
                _loadingMapInfo.OnNext(false);
            }
        }

        private void FillSongInfo(MapInfo info)
        {
            var audioFilePath = Path.Combine(info.DirectoryPath, info.SongFileName);

            using (var audioFile = new VorbisReader(audioFilePath))
            {
                info.SongDuration = audioFile.TotalTime;
            }
        }

        public Image GetMapCoverImage(string mapId)
        {
            var mapInfo = _mapInfo.Value[mapId];

            var imageFilePath = Path.Combine(mapInfo.DirectoryPath, mapInfo.CoverImageFilename);

            return Image.FromFile(imageFilePath);
        }

        public string GetMapSongPath(string mapId)
        {
            var mapInfo = _mapInfo.Value[mapId];

            return Path.Combine(mapInfo.DirectoryPath, mapInfo.SongFileName);
        }
    }
}
