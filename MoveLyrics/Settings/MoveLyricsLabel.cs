using System.Xml.Serialization;

namespace MoveLyrics.Settings
{
    public class MoveLyricsLabel : BarePositionableLabel
    {
        [XmlIgnore]
        public int WindowId = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }
}
