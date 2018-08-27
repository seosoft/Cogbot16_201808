using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Ai.QnA;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace AnimalQna01
{
    public class AnimalQnaBot : IBot
    {
        private readonly QnAMaker qnaMaker;

        public AnimalQnaBot(IConfiguration configuration)
        {
            var qnaEndpoint = Startup.GetQnAMakerEndpoint(configuration);
            qnaMaker = new QnAMaker(qnaEndpoint);
        }

        public async Task OnTurn(ITurnContext context)
        {
            if (context.Activity.Type == ActivityTypes.Message)
            {
                if (!string.IsNullOrEmpty(context.Activity.Text))
                {
                    await qnaMaker.GetAnswers(context.Activity.Text.Trim()).ConfigureAwait(false);
                    if (!context.Responded)
                    {
                        await context.SendActivity("適切な回答が見つかりませんでした");
                    }
                }
            }
        }
    }
}
