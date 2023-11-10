using System.Text;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

//Enables the Lambda function's JSON input to be converted into a .NET class
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook;

public class Function
{
    public string FunctionHandler(object input, ILambdaContext context)
    {
        //GitHub issue arrives and details are converted to string
        dynamic json = JsonConvert.DeserializeObject<dynamic>(input.ToString());
        
        //Formats issue into a message for Slack
        string payload = $"{{'text':'Issue Created: {json.issue.html_url}'}}";
        
        //Makes a POST to Slack with the message (SLACK_URL is an Environment Variable from Lambda)
        var client = new HttpClient();
        var webRequest = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("SLACK_URL"))
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        //POST is sent
        var response = client.Send(webRequest);
        using var reader = new StreamReader(response.Content.ReadAsStream());
            
        return reader.ReadToEnd();
    }
}