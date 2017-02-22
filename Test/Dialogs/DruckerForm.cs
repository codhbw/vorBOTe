using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace Test.Dialogs
{
    public enum Anschlusstyp { Lokal, Netzwerk };
    public enum Problem { Duplex, Fehlercode };

    [Serializable]
    public class DruckerForm
    {
        [Prompt("Welchen Anschlusstypen hat der Drucker? {||}")]
        public Anschlusstyp? anschlusstyp;
        [Prompt("Was für ein Problem hast du mit dem Drucker? {||}")]
        public Problem? problem;

        public static IForm<DruckerForm> BuildForm()
        {
            return new FormBuilder<DruckerForm>().Build();
        }
    }
}