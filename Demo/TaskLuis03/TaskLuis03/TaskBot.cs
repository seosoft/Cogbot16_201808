using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Ai.LUIS;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace TaskLuis03
{
    public class TaskBot : IBot
    {
        private readonly Dictionary<string, Func<double, Newtonsoft.Json.Linq.JObject, string>> _functions;

        private readonly LuisModel _taskModel;

        public TaskBot(IConfiguration configuration)
        {
            _taskModel = Startup.GetLuisModel(configuration, "Task");

            _functions = new Dictionary<string, Func<double, JObject, string>>
            {
                ["Greeting"] = GetGreetingMessage,
                ["Task"] = GetTaskMessage,
                ["None"] = GetNoneMessage
            };
        }

        public async Task OnTurn(ITurnContext context)
        {
            if (context.Activity.Type == ActivityTypes.Message)
            {
                var luisResult = context.Services.Get<RecognizerResult>(LuisRecognizerMiddleware.LuisRecognizerResultKey);

                var intent = luisResult.GetTopScoringIntent();
                var entities = luisResult.Entities;
                await context.SendActivity(_functions[intent.intent](intent.score, entities));
            }
        }

        private string GetGreetingMessage(double score, JObject entities) => "こんにちは";

        private string GetNoneMessage(double score, JObject entities) => "ごめんなさい、わかりません";

        private string GetTaskMessage(double score, JObject entities)
        {
            var day = string.Empty;
            var task = false;
            foreach (var entity in entities)
            {
                switch (entity.Key.ToString())
                {
                    case "Day":
                        day = entity.Value.First.ToString();
                        break;
                    case "Task":
                        task = true;
                        break;
                }
            }

            return !string.IsNullOrEmpty(day) && task ?
                $"{day}の予定についてですね" : "ごめんなさい、わかりません";
        }
    }
}
