using System.Text;
using Google;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace YoutubeApi;

class YoutubeInteraction<T>
{
    private YouTubeService _youTubeService;
    private string _apiKey;
    
    public YoutubeInteraction(YouTubeService service, string apiKey)
    {
        _youTubeService = service;
        _apiKey = apiKey;
    }

   public async Task<string> GetChannelVideos(string channelIdOrName, int maxResults = 50, string parts = "snippet")
    {
        VideoService videoService = new VideoService(_youTubeService, _apiKey);
        int meanViews = 0;
        int meanComments = 0;
        int meanLikes = 0;
        int likeDislike = 0;
        int commentsViews = 0;

        Channel channel = await GetChannelInfo(channelIdOrName);
        
        var request = _youTubeService.Search.List(parts);
        request.Key = _apiKey;
        request.ChannelId = channel.Id;
        request.MaxResults = maxResults;
        request.Order = SearchResource.ListRequest.OrderEnum.Date;

        request.PublishedBefore = await GetLatestOrEarliestVideo(request, 0);

        int count = 0;
        
        SearchListResponse response;
        StringBuilder sb = new StringBuilder();

        while (count != (int) channel.Statistics.VideoCount!)
        {
            do
            {
                response = await request.ExecuteAsync();
        
                foreach (var item in response.Items)
                {
                    sb.Append(item.Id.VideoId).Append(',');
                    count++;
                }
        
                if (response.NextPageToken == null)
                {
                    var earliestVideo = await GetLatestOrEarliestVideo(request, 1);
                    request.PublishedBefore = earliestVideo;
                }

                if (count % 100 == 0)
                {
                    await videoService.GetVideosInfoAsync(sb.ToString());
                }
                request.PageToken = response.NextPageToken;

            } while (response.NextPageToken != null);
            
            
            meanViews += (int)videoService.GetMeanViews();
            meanComments += (int) videoService.GetMeanComments();
            meanLikes += (int) videoService.GetMeanLikes();
            likeDislike += (int) videoService.GetLikeDislikeRel();
            commentsViews += (int) videoService.GetCommentsViewsRel();
            

            
            sb.Clear();
        }

        return $@"Среднее количество просмотров: {meanViews}
                  Среднее количество комментов: {meanComments}
                  Среднее количество лайков: {meanLikes} 
                  Лайки/дизлайки: {likeDislike}
                  Комменты/просмотры: {commentsViews}";;
    }
    
   /// <summary>
   /// 
   /// </summary>
   /// <param name="request"></param>
   /// <param name="regime">
   /// 0 - get the first video
   /// 1 or any - get the last video</param>
   /// <returns></returns>
    async Task<DateTime> GetLatestOrEarliestVideo(SearchResource.ListRequest request, int regime)
    {
        return regime == 0 ? DateTime.Parse((await request.ExecuteAsync()).Items[0].Snippet.PublishedAtRaw)
                : DateTime.Parse((await request.ExecuteAsync()).Items[^1].Snippet.PublishedAtRaw);
    }

    int GetDaysInterval(DateTimeOffset? latestVideoDate, Channel channel)
    {
        int? daysBetween = latestVideoDate?.Subtract(channel.Snippet.PublishedAtDateTimeOffset!.Value).Days;

        return (int) ((ulong?) daysBetween / channel.Statistics.VideoCount)!;
    }
    
    async Task<Channel> GetChannelInfo(string channelIdOrName)
    {
        string parts = "snippet, statistics";

        var request = _youTubeService.Channels.List(parts);
        request.Key = _apiKey;
        request.Id = channelIdOrName;
        
         ChannelListResponse response = await request.ExecuteAsync();

        if (response.Items == null)
        {
            request.Id = null;
            request.ForUsername = channelIdOrName;
            response = await request.ExecuteAsync();

            if (response.Items == null)
            {
                throw new GoogleApiException("GetChannelInfo", "Не найден канал по имени и айди");
            }
      }
 
        return new Channel()
        {
            Id = response.Items[0].Id,
            Snippet = new ChannelSnippet()
            {
                PublishedAtDateTimeOffset = response.Items[0].Snippet.PublishedAtDateTimeOffset
            },
            Statistics = new ChannelStatistics()
            {
                VideoCount = response.Items[0].Statistics.VideoCount,
                SubscriberCount = response.Items[0].Statistics.SubscriberCount
            }
        };
    }
}