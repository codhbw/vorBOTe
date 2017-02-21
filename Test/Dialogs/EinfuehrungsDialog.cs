using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;
using System.Threading;

namespace Test.Dialogs
{
    [LuisModel("2fd2215e-c2c6-4f37-b1ee-a42a00f63f5f", "4f3468354b6246a9a5a8c0d3ace44ff5")]
    [Serializable]
    public class EinfuehrungsDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, habe ich nicht verstanden! "
                + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Einstieg")]
        public async Task Einstieg(IDialogContext context, LuisResult result)
        {
            EinfuehrungsDialog.getEntities(context, result);
            String frageart = "Frageart";
            String objekt = "Objekt";
            EntityRecommendation frageartEntity;
            EntityRecommendation objektEntity;
            if (!result.TryFindEntity(frageart, out frageartEntity))
            {
                var fragearten = (IEnumerable<Frageart>)Enum.GetValues(typeof(Frageart));

                PromptDialog.Choice(context,
                                    SelectFrageart,
                                    fragearten,
                                    "Was ist dein Anliegen?");
                context.Wait(MessageReceived);
            }
            if (!result.TryFindEntity(objekt, out objektEntity))
            {
                var objekte = (IEnumerable<Objekt>)Enum.GetValues(typeof(Objekt));

                PromptDialog.Choice(context,
                                    SelectObjekt,
                                    objekte,
                                    "Was ist das Objekt, um das es dir geht?");
                //context.Wait(MessageReceived);
            }
            //context.Call<object>(new DruckerDialog(), DruckerDialogDone);
        }

        public static void getEntities(IDialogContext context, LuisResult result)
        {
            foreach (EntityRecommendation entity in result.Entities)
            {
                context.ConversationData.SetValue<string>(entity.Type, entity.Entity);
            }
        }

        enum Frageart { ServiceRequest, Incident };
        enum Objekt { Drucker, Browser, BAP};

        private async Task SelectObjekt(IDialogContext context, IAwaitable<Objekt> objekt)
        {
            var message = string.Empty;
            switch (await objekt)
            {
                case Objekt.Drucker:
                case Objekt.Browser:
                case Objekt.BAP:
                    message = $"Du bist hier wegen: {objekt}";
                    break;
                default:
                    message = "Hm...das kenne ich nicht.";
                    break;
            }
            //await context.PostAsync(message);
            //context.Wait(MessageReceived);
        }

        private Task DruckerDialogDone(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
            return null;
        }

        private async Task SelectFrageart(IDialogContext context, IAwaitable<Frageart> frageart)
        {
            var message = string.Empty;
            switch (await frageart)
            {
                case Frageart.Incident:
                case Frageart.ServiceRequest:
                    message = $"Du bist hier wegen: {frageart}";
                    break;
                default:
                    message = $"Hm...mit diesem Problem kann ich dir leider nicht behilflich sein.";
                    break;
            }
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
    }
}