using Google.Apis.YouTube.v3;

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

    public HashSet<string> Tags { get; set; }
    

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
        var response = await request.ExecuteAsync();
        VideosSum += (uint)response.Items.Count;
        
        foreach (var item in response.Items)
        {
            CommentsSum += item.Statistics.CommentCount!.Value;
            LikeSum += item.Statistics.LikeCount!.Value;
            DislikeSum += item.Statistics.DislikeCount!.Value;
            ViewsSum += item.Statistics.ViewCount!.Value;
            Tags = Tags.Concat(item.Snippet.Tags).ToHashSet();
        }
    }

    public async Task GetChannelTheme()
    {
        
    }

    public ulong GetMeanViews() => ViewsSum / VideosSum;
    public ulong GetMeanComments() => CommentsSum / VideosSum;
    public ulong GetMeanLikes() => LikeSum / VideosSum;
    public ulong GetLikeDislikeRel() => LikeSum / DislikeSum;
    public ulong GetCommentsViewsRel() => CommentsSum / ViewsSum;

    public override string ToString()
    {
        return $@"Среднее количество просмотров: {GetMeanViews()}
                  Среднее количество комментов: {GetMeanComments()}
                  Среднее количество лайков: {GetMeanLikes()} 
                  Лайки/дизлайки: {GetLikeDislikeRel()}
                  Комменты/просмотры: {GetCommentsViewsRel()}";
    }

}