using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Ai.LUIS;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace TaskLuis02
{
    public class TaskBot : IBot
    {
        private readonly LuisModel _taskModel;

        public TaskBot(IConfiguration configuration)
        {
            _taskModel = Startup.GetLuisModel(configuration, "Task");
        }

        public async Task OnTurn(ITurnContext context)
        {
            if (context.Activity.Type == ActivityTypes.Message)
            {
                var luisResult = context.Services.Get<RecognizerResult>(LuisRecognizerMiddleware.LuisRecognizerResultKey);

                var intent = luisResult.GetTopScoringIntent();
                var entities = luisResult.Entities;
                await DispatchToTopIntent(context, intent.intent, entities);
            }
        }

        private async Task DispatchToTopIntent(ITurnContext context, string intent, Newtonsoft.Json.Linq.JObject entities)
        {
            switch (intent)
            {
                case "Greeting":
                    await context.SendActivity("こんにちは");
                    break;
                case "Task":
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
                    await context.SendActivity(!string.IsNullOrEmpty(day) && task ?
                        $"{day}の予定についてですね" : "すみません、わかりません");
                    break;
                case "None":
                    await context.SendActivity("すみません、わかりません");
                    break;
            }
        }
    }
}
