using System.Runtime.InteropServices.JavaScript;
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

        int monthsShift = 3;

        Channel channel = await GetChannelInfo(channelIdOrName);

        var request = _youTubeService.Search.List(parts);
        request.Key = _apiKey;
        request.ChannelId = channel.Id;
        request.MaxResults = maxResults;
        request.Order = SearchResource.ListRequest.OrderEnum.Date;
        request.PublishedAfter = DateTime.Now.AddMonths(-monthsShift);

        int count = 0;

        SearchListResponse response;
        StringBuilder sb = new StringBuilder();
        
        do
        {
            response = await request.ExecuteAsync();

            for (int i = 0; i < response.Items.Count; i++)
            {
                sb.Append(response.Items[i].Id.VideoId).Append(',');
                if (i == response.Items.Count - 1)
                {
                    await videoService.GetVideosInfoAsync(sb.Remove(sb.Length - 1, 1).ToString());
                    sb.Clear();
                }
            }
            request.PageToken = response.NextPageToken;
        } while (response.NextPageToken != null);
        
        return $@"Среднее количество просмотров за {monthsShift} месяца: {videoService.GetMeanViews()}
                  Среднее количество комментов за {monthsShift} месяца: {videoService.GetMeanComments()}
                  Среднее количество лайков за {monthsShift} месяца: {videoService.GetMeanLikes()} 
                  Средняя частота выпуска видео в месяц за {monthsShift} месяца: {videoService.GetMeanVideoProdFrequency()}
                  Комменты/просмотры за {monthsShift} месяца: {videoService.GetCommentsViewsRel()}";
    }

    // подписчики
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
    
    void SubscribersGained()
    {
    }
    void SubscribersDowned()
    {
        // дёргать каждый месяц
    } 
}