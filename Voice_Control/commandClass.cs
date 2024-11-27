using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Voice_Control
{
    internal class Command
    {
        public string Phrase;
        public int Action;
        public string? Argument;

        public Command(string p, int a, string arg)
        {
            Phrase = p;
            Action = a;
            Argument = arg;
        }
    }
}
