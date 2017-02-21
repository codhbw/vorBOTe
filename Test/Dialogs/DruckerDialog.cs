using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;

namespace Test.Dialogs
{
    [LuisModel("2fd2215e-c2c6-4f37-b1ee-a42a00f63f5f", "4f3468354b6246a9a5a8c0d3ace44ff5")]
    [Serializable]
    public class DruckerDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, habe ich nicht verstanden! "
                + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        enum AnschlussTyp { Lokal, Netzwerk };


        [LuisIntent("PrinterSupport")]
        public async Task PrinterSupport(IDialogContext context, LuisResult result)
        {
            EntityRecommendation anschlussTyp;
            if (!result.TryFindEntity("Anschlusstyp", out anschlussTyp))
            {
                var anschlussTypen = (IEnumerable<AnschlussTyp>)Enum.GetValues(typeof(AnschlussTyp));

                PromptDialog.Choice(context,
                                    SelectAnschlussTyp,
                                    anschlussTypen,
                                    "Welcher Anschlusstyp hat der Drucker?");
            }
            else
            {
                await context.PostAsync($"Druckerproblem ist jetzt behoben.");
                context.Wait(MessageReceived);
            }
            context.Done(true);
        }

        private async Task SelectAnschlussTyp(IDialogContext context, IAwaitable<AnschlussTyp> anschlussTyp)
        {
            var message = string.Empty;
            switch (await anschlussTyp)
            {
                case AnschlussTyp.Lokal:
                    message = $"Anschlusstyp {anschlussTyp} is ";
                    break;
                case AnschlussTyp.Netzwerk:
                    message = $"Anschlusstyp {anschlussTyp} is ";
                    break;
                default:
                    message = $"Sorry!! Ich kenne den Anschlusstyp {anschlussTyp} nicht!";
                    break;
            }
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

    }
}