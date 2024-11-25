using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Voice_Control
{
    internal class CommandList
    {
        public string Name { get; set; }
        public Command[]? Commands { get; set; }
        public string Culture { get; set; }

        public CommandList(string n, Command[]? cs, CultureInfo ci) 
        {
            Name = n;
            Commands = cs;
            Culture = ci.Name;
        }

        [JsonConstructor]
        public CommandList(string name, Command[]? commands, string culture)
        {
            Name = name;
            Commands = commands;
            Culture = culture;
        }
    }
}
