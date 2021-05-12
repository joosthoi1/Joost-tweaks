using BiendeoCHLib.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MoveLyrics.Settings
{
    public class MoveLyricsLabel : BarePositionableLabel
    {
        [XmlIgnore]
        public int WindowId = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }
}
