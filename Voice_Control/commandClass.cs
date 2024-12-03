using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Voice_Control
{
    internal class Command
    {
        public string Phrase { get; set; }
        public int Action {  get; set; }
        public string? Argument {  get; set; }

        [JsonConstructor]
        public Command(string Phrase, int Action, string? Argument)
        {
            this.Phrase = Phrase;
            this.Action = Action;
            this.Argument = Argument;
        }
    }
}
