using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test
{
    public class Helper
    {
        public static void getEntities(IDialogContext context, LuisResult result)
        {
            foreach (EntityRecommendation entity in result.Entities)
            {
                context.ConversationData.SetValue<string>(entity.Type.ToLower(), entity.Entity.ToLower());
            }
        }
    }
}