using System.Web;

internal interface ISecretsProvider
{
    Task<string> GetSecretAsync(string secretName);
}

class SecretsProvider : ISecretsProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _token;

    private readonly string GetSecretsEndpoint = "/secretsmanager/get?secretId=";

    public SecretsProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetSecretAsync(string secretName)
    {
        var httpRequest = new HttpRequestMessage(
            HttpMethod.Get, 
            new Uri($"{GetSecretsEndpoint}{HttpUtility.UrlEncode(secretName)}", UriKind.Relative));
        
        httpRequest.Headers.Add("X-Aws-Parameters-Secrets-Token",           
            Environment.GetEnvironmentVariable("AWS_SESSION_TOKEN")      
        );

        var response = await _httpClient
            .SendAsync(httpRequest)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var responseAsJsonString = await response.Content
            .ReadAsStringAsync()
            .ConfigureAwait(false);

        return responseAsJsonString;
    }
}