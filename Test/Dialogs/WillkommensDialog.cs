using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace Test.Dialogs
{
    [Serializable]
    public class WillkommensDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            List<string> halloStrings = new List<string>() { "hallo", "hallo!", "hello", "hello!", "hi", "hi!" };

            if (halloStrings.Contains(message.Text.ToLower()))
            {
                await context.PostAsync("Hallo! Wie heißt du genau?");
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                context.ConversationData.SetValue<string>("benutzername", message.Text);
                await context.PostAsync($"Willkommen {message.Text}!");

                await context.PostAsync("Was für ein Problem hast du?");
                context.Call<object>(new EinfuehrungsDialog(), DialogDone);
            }
        }

        private async Task DialogDone(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("Auf wiedersehen!");
            return;
        }
    }
}