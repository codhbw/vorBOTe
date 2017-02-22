using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace Test.Dialogs
{
    public enum Anschluss { Lokal, Netzwerk };
    public enum Problem { Duplex, Fehlercode };

    [Serializable]
    public class DruckerForm
    {
        public Anschluss? anschlusstyp;
        public Problem? problem;

        public static IForm<DruckerForm> BuildForm()
        {
            return new FormBuilder<DruckerForm>().Build();
        }
    }
}