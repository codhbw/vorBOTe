using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using System.Globalization;

namespace Test.Dialogs
{
    public enum Anschlusstyp { Lokal, Netzwerk };
    public enum Name { KABuero01, KABuero02 };
    public enum Problem { Duplex, Fehlercode };

    [Serializable]
    public class DruckerForm
    {
        [Prompt("Wie ist der Drucker an den Arbeitsplatz angebunden? {||}")]
        public Anschlusstyp? anschlusstyp;
        [Prompt("Wie heißt der Drucker? {||}")]
        public Name? name;
        [Prompt("Was für ein Problem hast du mit dem Drucker? {||}")]
        public Problem? problem;

        public static IForm<DruckerForm> BuildForm()
        {
            OnCompletionAsyncDelegate<DruckerForm> druckerHelp = async (context, state) =>
            {
                await context.PostAsync("We are currently trying to find out how to help you.");
                var myform = new FormDialog<AnwendugForm>(new AnwendugForm(), AnwendugForm.BuildForm, FormOptions.PromptInStart, null, new CultureInfo("de-DE"));
                context.Call<AnwendugForm>(myform, DruckerDialogDone);
            };
            return new FormBuilder<DruckerForm>().OnCompletion(druckerHelp).Build();
        }

        private static async Task OnFormCompletion(IDialogContext context, DruckerForm state)
        {
            if (Problem.Duplex.Equals(state.problem))
            {
                //await context.PostAsync("Aus welcher Anwendung kannst du nicht drucken?");
                //context.Call<object>(new DruckerDialog(), DruckerDialogDone);
                var myform = new FormDialog<AnwendugForm>(new AnwendugForm(), AnwendugForm.BuildForm, FormOptions.PromptInStart, null, new CultureInfo("de-DE"));
                context.Call<AnwendugForm>(myform, DruckerDialogDone);
            }
        }

        private static async Task DruckerDialogDone(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("Wir hoffen, die Hilfeseiten waren hilfreich.");
        }
    }
}