using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Dialogs
{
    public enum Anwendungsart { Office, BAP, Windows };

    [Serializable]
    public class AnwendugForm
    {
        public Anwendungsart? anwendung;

        public static IForm<AnwendugForm> BuildForm()
        {
            OnCompletionAsyncDelegate<AnwendugForm> druckerHelp = async (context, state) =>
            {
                if (Anwendungsart.Windows.Equals(state.anwendung))
                {
                    await context.PostAsync("Hier ist eine Anleitung.");
                }
            };
            return new FormBuilder<AnwendugForm>().OnCompletion(druckerHelp).Build();
        }
    }
}