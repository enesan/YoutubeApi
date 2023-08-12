using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.YouTubeAnalytics.v2;
using Google.Apis.YouTubeReporting.v1;
using YoutubeApi;

const string SECRET = "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";
const string CLIENT_ID = "915496023150-p3r9stpk2ehmi474lp4orcurg67q3tct.apps.googleusercontent.com";
const string API_KEY = "AIzaSyA_HckYImRn8zMOGks4tLVH2zsM1FTVWMA";
const string GOOGLE_CHANNEL_ID = "UC_x5XG1OV2P6uZZ5FSM9Ttw"; // 5700 видео
const string CHANNEL_NAME = "tankionline"; // 701 видео
const string TEST_VIDEO_ID = "kfd-oLypqFI";

string[] scopes = 
{
    YouTubeService.ScopeConstants.Youtube,
    YouTubeService.ScopeConstants.YoutubeChannelMembershipsCreator,
    YouTubeService.ScopeConstants.YoutubeForceSsl,
    YouTubeService.ScopeConstants.Youtubepartner,
    YouTubeAnalyticsService.ScopeConstants.YtAnalyticsMonetaryReadonly,
    YouTubeAnalyticsService.ScopeConstants.YtAnalyticsReadonly,
};

ClientSecrets secrets = new ClientSecrets()
{
    ClientId = CLIENT_ID,
    ClientSecret = SECRET
};

using YouTubeService service = new YouTubeService();

//using YouTubeAnalyticsService analyticsService = new YouTubeAnalyticsService();
//using YouTubeReportingService reportingService = new YouTubeReportingService();

VideoService videoService = new VideoService(service, API_KEY);
YoutubeInteraction<int> yi = new YoutubeInteraction<int>(service,API_KEY);

await UserAuthService.AuthorizeUser(secrets, scopes, "asd");

//Console.WriteLine(await yi.GetChannelVideos("tankionline"));

var a = await GetMyChannelInfo();

Console.WriteLine(a.Id);
int aa = 12;

async Task<Channel> GetMyChannelInfo()
{
    string parts = "snippet, statistics";

    var request = service.Channels.List(parts);
    request.Key = API_KEY;
    //request.ForUsername = "tankionline";
    request.Mine = true;
        
    ChannelListResponse response = await request.ExecuteAsync();

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



// 100 - заглушка
ulong SubscribersGained()
{
    // что-то с бд
    return GetMyChannelInfo().Result.Statistics.SubscriberCount!.Value - 100;
}

ulong SubscribersDowned()
{
    // что-то с бд
    return GetMyChannelInfo().Result.Statistics.SubscriberCount!.Value - 100;
}





