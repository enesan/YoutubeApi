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
const string CLIENT_ID = "915496023150-1dhiofe00q666gdikpemaofhudbkjeui.apps.googleusercontent.com";
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

await UserAuthService.AuthorizeUser(secrets, scopes, "user");

Console.WriteLine(await yi.GetChannelVideos("tankionline"));

    #region AnalyticsApi

// var aRequest = analyticsService.Reports.Query();
//
// aRequest.Ids = $"channel=={GOOGLE_CHANNEL_ID}";
//
//
// File.Delete("../../../ResponseFiles/analytics.json");
// await using (Stream toWrite = File.Create("../../../ResponseFiles/analytics.json"))
// {
//     
//     await using (Stream resp = await aRequest.ExecuteAsStreamAsync())
//     {
//         await resp.CopyToAsync(toWrite);
//     }
// }

#endregion






async Task GetVideoInfo(string ids)
{
    var request = service.Videos.List("snippet,contentDetails,statistics");
    request.Id = ids;
    var response = await request.ExecuteAsync();

    foreach (var item in response.Items)
    {
        
    }
    
}


void SubscribersGained()
{
    // дёргать каждый месяц
}

void SubscribersDowned()
{
    // дёргать каждый месяц
}





