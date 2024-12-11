using Google.Apis.Auth.OAuth2;
using Google.Cloud.Dialogflow.V2;
using Grpc.Auth;
using Microsoft.Extensions.Options;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Settings;

namespace Neighbor.Infrastructure.Services;

public class DialogflowService : IDialogflowService
{
    private readonly SessionsClient _client;
    private readonly string _projectId;
    private readonly DialogflowSetting _dialogflowSetting;

    public DialogflowService(IOptions<DialogflowSetting> dialogflowConfig)
    {
        _dialogflowSetting = dialogflowConfig.Value;

        string json = $@"{{
          ""type"": ""{_dialogflowSetting.Type}"",
          ""project_id"": ""{_dialogflowSetting.ProjectId}"",
          ""private_key_id"": ""{_dialogflowSetting.PrivateKeyId}"",
          ""private_key"": ""{_dialogflowSetting.PrivateKey}"",
          ""client_email"": ""{_dialogflowSetting.ClientEmail}"",
          ""client_id"": ""{_dialogflowSetting.ClientId}"",
          ""auth_uri"": ""{_dialogflowSetting.AuthUri}"",
          ""token_uri"": ""{_dialogflowSetting.TokenUri}"",
          ""auth_provider_x509_cert_url"": ""{_dialogflowSetting.AuthProviderX509CertUrl}"",
          ""client_x509_cert_url"": ""{_dialogflowSetting.ClientX509CertUrl}"",
          ""universe_domain"": ""{_dialogflowSetting.UniverseDomain}""
        }}";

        _projectId = dialogflowConfig.Value.ProjectId;
        
        var credential = GoogleCredential.FromJson(json);
        // Khởi tạo SessionsClient với credential
        _client = new SessionsClientBuilder
        {
            ChannelCredentials = credential.ToChannelCredentials(),
        }.Build();
    }

    public async Task<string> DetectIntentAsync(string sessionId, string text, string languageCode)
    {
        var session = new SessionName(_projectId, sessionId);
        var queryInput = new QueryInput
        {
            Text = new TextInput
            {
                Text = text,
                LanguageCode = languageCode
            }
        };

        var response = await _client.DetectIntentAsync(session, queryInput);
        return response.QueryResult.FulfillmentText;
    }
}

