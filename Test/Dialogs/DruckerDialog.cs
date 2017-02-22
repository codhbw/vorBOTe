using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace Test.Dialogs
{
    enum AnschlussTyp { Lokal, Netzwerk };
    enum Problemtyp { Duplex, Fehlercode};
    enum Anwendung { Office, Windows};

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


        [LuisIntent("PrinterSupport")]
        public async Task PrinterSupport(IDialogContext context, LuisResult result)
        {
            if (result != null) Helper.getEntities(context, result);
            String anschlussTyp;
            String problem;
            String anwendung;
            context.ConversationData.TryGetValue<string>("problemtyp", out problem);
            if (!context.ConversationData.TryGetValue<string>("anschlusstyp", out anschlussTyp))
            {
                var anschlussTypen = (IEnumerable<AnschlussTyp>)Enum.GetValues(typeof(AnschlussTyp));

                PromptDialog.Choice(context,
                                    SelectAnschlussTyp,
                                    anschlussTypen,
                                    "Wie ist der Drucker an den Arbeitsplatz angebunden?");
            }
            //else
            //{
            //    await context.PostAsync($"Druckerproblem ist jetzt behoben.");
            //    context.Wait(MessageReceived);
            //}
            else if (!context.ConversationData.TryGetValue<string>("problemtyp", out problem))
            {
                var problemtypen = (IEnumerable<Problemtyp>)Enum.GetValues(typeof(Problemtyp));

                PromptDialog.Choice(context,
                                    SelectProblemtyp,
                                    problemtypen,
                                    "Welches Problem hast du mit dem Drucker?");
            }
            else if (Problemtyp.Duplex.ToString().ToLower().Equals(problem)
                && !context.ConversationData.TryGetValue<string>("anwendung", out anwendung))
            {
                var anwendungen = (IEnumerable<Anwendung>)Enum.GetValues(typeof(Anwendung));

                PromptDialog.Choice(context,
                                    SelectAnwendung,
                                    anwendungen,
                                    "Aus welcher Anwendung kannst du nicht drucken?");
            }
            //context.Done(true);
        }

        private async Task SelectAnschlussTyp(IDialogContext context, IAwaitable<AnschlussTyp> anschlussTyp)
        {
            String problem;
            String anwendung;
            var message = string.Empty;
            context.ConversationData.TryGetValue<string>("problemtyp", out problem);
            switch (await anschlussTyp)
            {
                case AnschlussTyp.Lokal:
                    context.ConversationData.SetValue<string>("anschlusstyp", "lokal");
                    break;
                case AnschlussTyp.Netzwerk:
                    context.ConversationData.SetValue<string>("anschlusstyp", "netzwerk");
                    message = $"Anschlusstyp ist {anschlussTyp}";
                    break;
                default:
                    message = $"Sorry!! Den Anschlusstyp {anschlussTyp} kenne ich nicht!";
                    break;
            }
            if (!context.ConversationData.TryGetValue<string>("problemtyp", out problem))
            {
                var problemtypen = (IEnumerable<Problemtyp>)Enum.GetValues(typeof(Problemtyp));

                PromptDialog.Choice(context,
                                    SelectProblemtyp,
                                    problemtypen,
                                    "Welches Problem hast du mit dem Drucker?");
            }
            else if (Problemtyp.Duplex.ToString().ToLower().Equals(problem)
                && !context.ConversationData.TryGetValue<string>("anwendung", out anwendung))
            {
                var anwendungen = (IEnumerable<Anwendung>)Enum.GetValues(typeof(Anwendung));

                PromptDialog.Choice(context,
                                    SelectAnwendung,
                                    anwendungen,
                                    "Aus welcher Anwendung kannst du nicht drucken?");
            }
        }

        private async Task SelectProblemtyp(IDialogContext context, IAwaitable<Problemtyp> problemtyp)
        {
            String problem;
            String anwendung;
            var message = string.Empty;
            switch (await problemtyp)
            {
                case Problemtyp.Duplex:
                    context.ConversationData.SetValue<string>("problemtyp", "duplex");
                    break;
                case Problemtyp.Fehlercode:
                    context.ConversationData.SetValue<string>("problemtyp", "fehlercode");
                    break;
                default:
                    message = $"Sorry!! Den Problemtyp {problemtyp} kenne ich nicht!";
                    break;
            }
            context.ConversationData.TryGetValue<string>("problemtyp", out problem);
            if (Problemtyp.Duplex.ToString().ToLower().Equals(problem)
                && !context.ConversationData.TryGetValue<string>("anwendung", out anwendung))
            {
                var anwendungen = (IEnumerable<Anwendung>)Enum.GetValues(typeof(Anwendung));

                PromptDialog.Choice(context,
                                    SelectAnwendung,
                                    anwendungen,
                                    "Aus welcher Anwendung kannst du nicht drucken?");
            }
        }

        private async Task SelectAnwendung(IDialogContext context, IAwaitable<Anwendung> anwendung)
        {
            IMessageActivity msg = context.MakeMessage();
            msg.Type = "message";
            msg.Attachments = new List<Attachment>();
            List<CardImage> cardImages = new List<CardImage>();
            List<CardAction> cardButtons = new List<CardAction>();
            CardAction plButton = null;
            HeroCard plCard = null;
            switch (await anwendung)
            {
                case Anwendung.Office:
                    context.ConversationData.SetValue<string>("anwendung", "office");

                    cardImages.Add(new CardImage(url: "https://uqzqqq-bn1305.files.1drv.com/y3pR8K3P3Qdr7gkcPmrE_rSyVHYuOQ4-Azt_4fEA4fp4K8pSclujdIV4JGrbRtj-zMY9ADPJikimo7xQqam6ZYFQkurm-wVd-eCUUo2-baqNt-WcAu5HXusFrSmUFGCh94wsJbkLNGSHSXmTW2tzS1rIQ/Drucken%20MS%20Office.jpg?psid=1"));

                    plButton = new CardAction()
                    {
                        Value = "https://1drv.ms/b/s!AsBP--kaOJEKgfBlOHEU36x3VwMG8A",
                        Type = "openUrl",
                        Title = "Zur Anleitung"
                    };

                    plCard = new HeroCard()
                    {
                        Title = "Office Beidseitigdrucken",
                        Subtitle = "Beschreibung wie man im Office das Beidseitigdrucken einstellt.",
                        Images = cardImages,
                        Buttons = cardButtons
                    };
                    break;
                case Anwendung.Windows:
                    context.ConversationData.SetValue<string>("anwendung", "windows");

                    cardImages.Add(new CardImage(url: "https://uqzqqq-bn1305.files.1drv.com/y3pR8K3P3Qdr7gkcPmrE_rSyVHYuOQ4-Azt_4fEA4fp4K8pSclujdIV4JGrbRtj-zMY9ADPJikimo7xQqam6ZYFQkurm-wVd-eCUUo2-baqNt-WcAu5HXusFrSmUFGCh94wsJbkLNGSHSXmTW2tzS1rIQ/Drucken%20MS%20Office.jpg?psid=1"));

                    plButton = new CardAction()
                    {
                        Value = "https://1drv.ms/b/s!AsBP--kaOJEKgfBlOHEU36x3VwMG8A",
                        Type = "openUrl",
                        Title = "Zur Anleitung"
                    };

                    plCard = new HeroCard()
                    {
                        Title = "Windows-Anwendungen Beidseitigdrucken",
                        Subtitle = "Beschreibung wie man in Windows-Anwendungen das Beidseitigdrucken einstellt.",
                        Images = cardImages,
                        Buttons = cardButtons
                    };
                    break;
                default:
                    break;
            }
            cardButtons.Add(plButton);

            Attachment plAttachment = plCard.ToAttachment();
            msg.Attachments.Add(plAttachment);

            await context.PostAsync(msg);
            context.Wait(MessageReceived);
        }

    }
}