﻿using CodeHollow.FeedReader;
using CodeHollow.FeedReader.Feeds;
using Newtonsoft.Json;
using NoiseCast.Core;

namespace NoiseCast.MVVM.Model
{
    public class EpisodeModel : ObservableObject
    {
        public event EpisodeChangedEventHandler EpisodeChanged;
        public virtual void OnEpisodeChanged(EpisodeChangedEventArgs e) => EpisodeChanged?.Invoke(this, e);

        [JsonProperty("id")] private string _id;
        [JsonProperty("imagePath")] private string _imagePath;
        [JsonProperty("duration")] private double _durationListened;
        private string _mediaPath;
        private FeedItem _episode;

        [JsonIgnore] public string ID => _id;
        [JsonIgnore] public FeedItem Episode => _episode;
        [JsonIgnore] public string ImagePath => _imagePath;
        [JsonIgnore] public string MediaPath => _mediaPath;
        [JsonIgnore] public double DurationListened { get => _durationListened; set => SetProperty(ref _durationListened, value); }
        [JsonIgnore] public bool IsArchived => _durationListened == -1 ? true : false;

        /// <summary>
        /// Constructor for Json-Serialization
        /// </summary>
        [JsonConstructor]
        public EpisodeModel(string id, string imagePath, double duration)
        {
            _id = id;
            _imagePath = imagePath;
            _durationListened = duration;
        }

        /// <summary>
        /// New episode constructor
        /// </summary>
        public EpisodeModel(FeedItem episode, string id, string imagePath)
        {
            _episode = episode;
            _id = id;
            _imagePath = imagePath;
            _durationListened = 0;
            _mediaPath = GetMediaPath();
        }

        /// <summary>
        /// Set <see cref="DurationListened"/> to -1, so that <see cref="IsArchived"/> returns true
        /// </summary>
        public void SetIsArchived() => _durationListened = -1;

        /// <summary>
        /// Set <see cref="FeedItem"/>
        /// </summary>
        /// <param name="episode"></param>
        public void SetEpisodeFeed(FeedItem episode)
        {
            _episode = episode;
            _mediaPath = GetMediaPath();
        }

        /// <summary>
        /// Set Path to the Enclosure URL depending on the FeedType
        /// </summary>
        private string GetMediaPath()
        {
            if (_episode == null) return null;

            string type = _episode.SpecificItem.GetType().Name;

            return type switch
            {
                nameof(Rss20FeedItem) => ((Rss20FeedItem)_episode.SpecificItem).Enclosure.Url,
                nameof(MediaRssFeedItem) => ((MediaRssFeedItem)_episode.SpecificItem).Enclosure.Url,
                _ => null
            };
        }
    }
}