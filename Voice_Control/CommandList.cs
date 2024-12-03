using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Voice_Control
{
    public class CommandList
    {
        public string Name { get; set; }
        public Command[]? Commands { get; set; }
        public string Culture { get; set; }
        public bool isNative { get; set; }

        public CommandList(string n, Command[]? cs, CultureInfo ci, bool isN) 
        {
            Name = n;
            Commands = cs;
            Culture = ci.Name;
            isNative = isN;
        }

        [JsonConstructor]
        public CommandList(string Name, Command[]? Commands, string Culture, bool isNative)
        {
            this.Name = Name;
            this.Commands = Commands;
            this.Culture = Culture;
            this.isNative = isNative;
        }
    }
}
