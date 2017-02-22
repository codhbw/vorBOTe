using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using Test.Dialogs;

namespace Test
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new WillkommensDialog());
            }
            else
            {
                //add code to handle errors, or non-messaging activities
                await HandleSystemMessage(activity);
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private async Task HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                var client = new ConnectorClient(new Uri(message.ServiceUrl));
                IConversationUpdateActivity update = message;
                if (update.MembersAdded.Any())
                {
                    var reply = message.CreateReply();
                    var newMembers = update.MembersAdded?.Where(t => t.Id != message.Recipient.Id);
                    foreach (var newMember in newMembers)
                    {
                        reply.Text = "Willkommen beim vorBOTe! Wie heißt Du?";
                        //if (!string.IsNullOrEmpty(newMember.Name))
                        //{
                        //    reply.Text += $" {newMember.Name}";
                        //}
                        //reply.Text += "!";

                        await client.Conversations.ReplyToActivityAsync(reply);
                    }
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return;
        }
    }
}