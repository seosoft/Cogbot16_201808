using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using System;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;

namespace AnimalQnaRest01
{
    public class AnimalBot : IBot
    {
        private const string EndpointKey = "";
        private const string Host = "";
        private const string KnowledgebaseId = "";

        private const string RequestUri = Host + "/knowledgebases/" + KnowledgebaseId + "/generateAnswer/";

        public async Task OnTurn(ITurnContext context)
        {
            if (context.Activity.Type == ActivityTypes.Message)
            {
                var response = await GetAnswer(context.Activity.Text);
                var qnaResponse = JsonConvert.DeserializeObject<QnaResponse>(response);
                await context.SendActivity($"{qnaResponse.answers[0].answer} ({qnaResponse.answers[0].score})");
            }
        }

        private static async Task<string> GetAnswer(string message)
        {
            var question = $"{{\"question\": \"{message}\"}}";

            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(RequestUri);
                    request.Content = new StringContent(question, Encoding.UTF8, "application/json");
                    request.Headers.Add("Authorization", "EndpointKey " + EndpointKey);

                    var response = await client.SendAsync(request);
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
}
