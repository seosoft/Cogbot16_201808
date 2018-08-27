using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Ai.QnA;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace AnimalQna02
{
    public class AnimalQnaBot : IBot
    {
        private readonly QnAMaker qnaMaker;

        public AnimalQnaBot(IConfiguration configuration)
        {
            var qnaEndpoint = Startup.GetQnAMakerEndpoint(configuration);
            var qnaOptions = new QnAMakerOptions
            {
                ScoreThreshold = 0.01f,   // 0.95f, 0.5f, 0.01f
                Top = 2
            };
            qnaMaker = new QnAMaker(qnaEndpoint, qnaOptions);
        }

        public async Task OnTurn(ITurnContext context)
        {
            if (context.Activity.Type == ActivityTypes.Message)
            {
                var results = await qnaMaker.GetAnswers(context.Activity.Text.Trim()).ConfigureAwait(false);
                if (!context.Responded)
                {
                    if (!results.Any())
                    {
                        await context.SendActivity("適切な回答が見つかりませんでした");
                    }
                    else if (results.First().Score < 0.5f)
                    {
                        await context.SendActivity($"{results.First().Answer}");
                        if (results.Count() >= 2)
                        {
                            await context.SendActivity($"自信がないので他の回答もお伝えします\n\n{results[1].Answer}");
                        }
                    }
                }
            }
        }
    }
}
