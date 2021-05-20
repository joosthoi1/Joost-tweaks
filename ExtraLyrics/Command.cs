using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraLyrics
{
    struct ChartCommand
    {
        public string Command;
        public long Tick;
        public string Parameter;
        public string OriginalCommand;
        public double TimeInMs;

        public override string ToString()
        {
            return "{\n" +
                $"\tCommand:{this.Command},\n" +
                $"\tTick:{this.Tick},\n" +
                $"\tParameter:{this.Parameter},\n" +
                $"\tOriginalCommand:{this.OriginalCommand},\n" +
                "}";
        }
    }
}
