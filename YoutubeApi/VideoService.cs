using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace YoutubeApi;

public class VideoService
{
    private YouTubeService _service;
    private string _apiKey;
    private ulong CommentsSum { get; set; }
    private ulong VideosSum { get; set; }
    private ulong LikeSum { get; set; }
    private ulong DislikeSum { get; set; }
    private ulong ViewsSum { get; set; }
    private int VideoProdFrequencySum { get; set; }

    public HashSet<string> Tags { get; set; } = new();


    public VideoService(YouTubeService service, string apiKey)
    {
        _service = service;
        _apiKey = apiKey;
    }

    public async Task GetVideosInfoAsync(string ids)
    {
        var request = _service.Videos.List("snippet,contentDetails,statistics");
        request.Id = ids;
        request.Key = _apiKey;
        request.MaxResults = 50;
        var response = await request.ExecuteAsync();
        VideosSum += (uint) response.Items.Count;

        for (var index = 0; index < response.Items.Count; index++)
        {
            var item = response.Items[index];
            if (item != response.Items.First())
            {
                VideoProdFrequencySum += GetVideoProdFrequency(item, response.Items[index - 1]);
            }

            if (item.Statistics.CommentCount != null)
            {
                CommentsSum += item.Statistics.CommentCount.Value;
            }

            if (item.Statistics.LikeCount != null)
            {
                LikeSum += item.Statistics.LikeCount.Value;
            }

            if (item.Statistics.ViewCount != null)
            {
                ViewsSum += item.Statistics.ViewCount.Value;
            }

            if (item.Snippet.Tags != null)
            {
                Tags.UnionWith(item.Snippet.Tags);
            }
        }
    }

     int GetVideoProdFrequency(Video current, Video prev)
    {
        return current.Snippet.PublishedAtDateTimeOffset!.Value
            .Subtract(prev.Snippet.PublishedAtDateTimeOffset!.Value)
            .Hours;
    }

    public async Task GetChannelTheme()
    {
    }

    public ulong GetMeanViews() => ViewsSum / VideosSum;
    public ulong GetMeanComments() => CommentsSum / VideosSum;

    public ulong GetMeanLikes() => LikeSum / VideosSum;

    //   public ulong GetLikeDislikeRel() => LikeSum / DislikeSum;
    public ulong GetCommentsViewsRel() => CommentsSum / ViewsSum;
    public int GetMeanVideoProdFrequency() => VideoProdFrequencySum / (int)VideosSum;

    public override string ToString()
    {
        return $@"Среднее количество просмотров: {GetMeanViews()}
                  Среднее количество комментов: {GetMeanComments()}
                  Среднее количество лайков: {GetMeanLikes()} 
                  Комменты/просмотры: {GetCommentsViewsRel()}
                  Средняя частота выпуска видео: {GetMeanVideoProdFrequency()}";
    }

    public void ClearStats()
    {
        CommentsSum = 0;
        VideosSum = 0;
        LikeSum = 0;
        DislikeSum = 0;
        ViewsSum = 0;
        VideoProdFrequencySum = 0;
    }
}