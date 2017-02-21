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
    [LuisModel("6352092e-aac3-4ba6-b1e5-0a6d4f8925b5", "1b98be32a28e4190af249eb8e1ac73cd")]
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