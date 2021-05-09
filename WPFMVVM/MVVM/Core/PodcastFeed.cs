﻿using CodeHollow.FeedReader;
using CodeHollow.FeedReader.Feeds;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WPFMVVM.MVVM.Core
{
    public class PodcastFeed
    {
        public ObservableCollection<BaseFeed> RequestFeeds()
        {
            var feeds = new ObservableCollection<BaseFeed>();

            Parallel.ForEach(GetFeedUrl(), url =>
            {
                Feed feed = Task.Run<Feed>(() =>
                {
                    return FeedReader.ReadAsync(url);
                }).Result;

                var temp = GetPodcastType(feed);
                if (temp != null)
                {
                    feeds.Add(temp);
                    Debug.WriteLine($"Done: {temp.Title} - {temp.Items.Count} episodes loaded.");
                }
            });

            return feeds;
        }

        private BaseFeed GetPodcastType(Feed feed)
        {
            switch (feed.Type)
            {
                case FeedType.Atom:
                    return null;
                case FeedType.Rss_0_91:
                    return null;
                case FeedType.Rss_0_92:
                    return null;
                case FeedType.Rss_1_0:
                    return null;
                case FeedType.Rss_2_0:
                    var rss20 = (Rss20Feed)feed.SpecificFeed;
                    if (rss20.Image.Url == null)
                        rss20.Image.Url = new CodeHollow.FeedReader.Feeds.Itunes.ItunesChannel(rss20.Element).Image.Href;
                    return rss20;
                case FeedType.MediaRss:
                    var mediarss = (MediaRssFeed)feed.SpecificFeed;
                    if (mediarss.Image.Url == null)
                        mediarss.Image.Url = new CodeHollow.FeedReader.Feeds.Itunes.ItunesChannel(mediarss.Element).Image.Href;
                    return mediarss;
                case FeedType.Rss:
                    return null;
                case FeedType.Unknown:
                    Debug.WriteLine("Unknown format: " + feed.Title);
                    return null;
                default:
                    Debug.WriteLine("Skip: Unknown type");
                    return null;
            }
        }

        private List<string> GetFeedUrl()
        {
            return new List<string>()
            {
                @"http://www.superkreuzburg.de/feed/ratsherren/",
                @"https://feeds.lagedernation.org/feeds/ldn-mp3.xml",
                @"https://feeds.simplecast.com/dLRotFGk",
                @"https://www.omnycontent.com/d/playlist/5ac1e950-45c7-4eb7-87c0-aa0f018441b8/008f004b-055e-4e0c-9fd0-abc000ffb04a/3b93bc3f-52cb-4a3b-91ef-abc000ffb053/podcast.rss",
                @"https://feeds.buzzsprout.com/1183862.rss",
                @"http://rechtsbelehrung.com/feed/podcast",
                @"https://feeds.buzzsprout.com/176239.rss",
                @"https://www.hookedmagazin.de/feed/hookedfm/",
                @"https://guildnews.podcaster.de/Guildnews_Podcast.rss",
                @"https://completedeveloperpodcast.com/feed/podcast/",
                @"https://okcool.podigee.io/feed/mp3",
                @"https://80erman.podcaster.de/younginthe80s.rss",
                @"http://feeds.soundcloud.com/users/soundcloud:users:377025653/sounds.rss",
                @"https://k1svup.podcaster.de/edeltalk.rss",
                @"https://www.codingblocks.net/podcast-feed.xml",
                @"http://feeds.feedburner.com/WaveformWithMkbhd",
                @"https://meinscrumistkaputt.de/feed/podcast/",
                @"https://podcastd45a61.podigee.io/feed/mp3",
                @"https://www.hoaxilla.com/podcast/hoaxilla.xml",

                //@"https://steadyhq.com/rss/hooked?auth=72719c2d-0e79-4bcb-a027-f65becedce71",
                //@"https://www.patreon.com/rss/hooked?auth=0Ig5i4ROq4Aqsfkoub6aIhvxQbJJvXo9",
                //@"http://aufeinbier:a3b_dankefueralles@www.gamespodcast.de/feed/premium/",
                //@"https://www.patreon.com/rss/stayforever?auth=35a1206f546de3fc6d2d34b72fbf6db5",
                //@"https://steadyhq.com/rss/stayforever?auth=ac02dce0-92ff-4364-93ed-2466069e22d4"
            };
        }
    }
}