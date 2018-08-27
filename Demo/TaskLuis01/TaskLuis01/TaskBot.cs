using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Ai.LUIS;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;

namespace TaskLuis01
{
    public class TaskBot : IBot
    {
        public async Task OnTurn(ITurnContext context)
        {
            if (context.Activity.Type == ActivityTypes.Message)
            {
                var luisResult = context.Services.Get<RecognizerResult>(LuisRecognizerMiddleware.LuisRecognizerResultKey);

                var (intent, score) = luisResult.GetTopScoringIntent();
                await context.SendActivity($"Intent: {intent}\n\nScore: {score}");
            }
        }
    }
}
