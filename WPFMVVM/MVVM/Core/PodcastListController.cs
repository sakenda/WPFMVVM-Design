﻿using CodeHollow.FeedReader;
using NoiseCast.Core;
using NoiseCast.MVVM.Model;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace NoiseCast.MVVM.ViewModel.Controller
{
    public static class PodcastListController
    {
        /// <summary>
        /// Add a feed based on an url string.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="url"></param>
        /// <returns>Returns <see cref="true"/> if adding was successful, <see cref="false"/> when podast is already added to list</returns>
        public static bool AddFeed(this ObservableCollection<PodcastModel> list, string url)
        {
            Feed feed = null;

            try
            {
                feed = Task.Run(() => FeedReader.ReadAsync(url)).Result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Could not load Feed from:" + url, ex);
            }

            if (feed == null) return false;

            var podcastModel = new PodcastModel(feed, url);
            list.Add(podcastModel);

            return true;
        }

        /// <summary>
        /// Removes Podcast if it exist. Removes saved files from subscribed podcast
        /// </summary>
        /// <param name="podcast"></param>
        /// <returns>Returns <see cref="true"/> if Podcast was Removed, <see cref="false"/> if it wasn't.</returns>
        public static bool RemoveFeed(this PodcastModel podcast)
        {
            if (podcast == null) return false;
            if (!MainViewModel.PodcastsList.Contains(podcast)) return false;

            if (podcast.IsSubscribed)
            {
                string[] filePaths = new string[]
                {
                    podcast.ImagePath,
                    podcast.FilePath
                };

                FileController.DeleteFile(filePaths);
            }

            MainViewModel.PodcastsList.Remove(podcast);

            return true;
        }

        /// <summary>
        /// Serializes podcast locally
        /// </summary>
        /// <param name="podcast"></param>
        public static void SaveFeed(this PodcastModel podcast) => FeedSerialization.Serialize(podcast);

        /// <summary>
        /// Load the given RSS-Link and check if <see cref="Feed.LastUpdatedDate"/> is newer than the current
        /// <see cref="PodcastModel.LastUpdatedDate"/>.
        /// </summary>
        public static async void UpdateFeed(this PodcastModel podcast)
        {
            Feed feed = await FeedReader.ReadAsync(podcast.RSSLink);
            podcast.Update(feed);
        }
    }
}