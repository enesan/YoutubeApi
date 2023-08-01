using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTubeAnalytics.v2;

namespace YoutubeApi;

public class UserAuthService
{

    public UserAuthService()
    {
       
    }

    public static async Task<UserCredential> AuthorizeUser(ClientSecrets secrets, string[] scopes, string userName)
    {
        return await GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, scopes, userName, CancellationToken.None);
    }
}