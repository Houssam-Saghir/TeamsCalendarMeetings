using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string clientId = configuration["AzureAd:ClientId"] ?? throw new InvalidOperationException("ClientId not found in configuration");
string tenantId = configuration["AzureAd:TenantId"] ?? throw new InvalidOperationException("TenantId not found in configuration");
string clientSecret = configuration["AzureAd:ClientSecret"] ?? throw new InvalidOperationException("ClientSecret not found in configuration");
string userEmail = configuration["UserEmail"] ?? throw new InvalidOperationException("UserEmail not found in configuration");



string accessToken = await GetAccessTokenAsync(clientId, tenantId, clientSecret);
string responseBody = await CreateTeamsMeetingAsync(accessToken, userEmail);
ParseAndDisplayMeeting(responseBody);



static async Task<string> GetAccessTokenAsync(string clientId, string tenantId, string clientSecret)
{
    IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
        .Create(clientId)
        .WithClientSecret(clientSecret)
        .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
        .Build();

    string[] scopes = ["https://graph.microsoft.com/.default"];

    AuthenticationResult result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
    return result.AccessToken;
}

static async Task<string> CreateTeamsMeetingAsync(string accessToken, string userEmail)
{
    using var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", accessToken);

    // Meeting starts 1 hour from now, ends 2 hours from now
    var start = DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ss");
    var end = DateTime.UtcNow.AddHours(2).ToString("yyyy-MM-ddTHH:mm:ss");

    var eventJson = $$"""
    {
        "subject": "Test Teams Meeting from .NET",
        "body": {
            "contentType": "HTML",
            "content": "This meeting was created from a .NET Console Application using Microsoft Graph API."
        },
        "start": {
            "dateTime": "{{start}}",
            "timeZone": "UTC"
        },
        "end": {
            "dateTime": "{{end}}",
            "timeZone": "UTC"
        },
        "isOnlineMeeting": true,
        "onlineMeetingProvider": "teamsForBusiness",
        "attendees": [
            {
                "emailAddress": {
                    "address": "{{userEmail}}",
                    "name": "Meeting Organizer"
                },
                "type": "required"
            }
        ]
    }
    """;

    var content = new StringContent(eventJson, Encoding.UTF8, "application/json");

    var response = await httpClient.PostAsync(
        $"https://graph.microsoft.com/v1.0/users/{userEmail}/events", content);

    string body = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine($"Error: {response.StatusCode}");
        Environment.Exit(1);
    }

    return body;
}

static void ParseAndDisplayMeeting(string json)
{
    using var doc = JsonDocument.Parse(json);
    var root = doc.RootElement;

    string subject = root.GetProperty("subject").GetString() ?? "-";
    string webLink = root.GetProperty("webLink").GetString() ?? "-";
    string joinUrl = root
                          .GetProperty("onlineMeeting")
                          .GetProperty("joinUrl")
                          .GetString() ?? "-";
    string startTime = root.GetProperty("start").GetProperty("dateTime").GetString() ?? "-";
    string endTime = root.GetProperty("end").GetProperty("dateTime").GetString() ?? "-";

}