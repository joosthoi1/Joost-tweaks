using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraLyrics
{
    struct LyricsCommand
    {
        public string Command;
        public int Tick;
        public string Parameter;
        public bool NoNewLine;
        public string OriginalCommand;
        public float TimeInMs;

        public override string ToString()
        {
            return "{\n" +
                $"\tCommand:{this.Command},\n" +
                $"\tTick:{this.Tick},\n" +
                $"\tParameter:{this.Parameter},\n" +
                $"\tNoNewLine:{this.NoNewLine},\n" +
                "}";
        }
    }
}
