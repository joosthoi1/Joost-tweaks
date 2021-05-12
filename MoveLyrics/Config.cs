using BiendeoCHLib.Settings;
using System;
using System.Xml.Serialization;
using UnityEngine;

namespace MoveLyrics.Settings
{
    [Serializable]
    public class Config
    {
        public int Version;
        public string TweakVersion;

        public float ConfigX;
        public float ConfigY;
        public KeyBind ConfigKeyBind;

        public bool Enabled;
        public KeyBind EnabledKeyBind;

        [XmlIgnore]
        public bool DraggableLabelsEnabled;
        [XmlIgnore]
        public bool ConfigWindowEnabled;
        [XmlIgnore]
        private bool wasMouseVisible;
        [XmlIgnore]
        public int LayoutIndexSelected;
        public Config()
        {
            Version = 1;
            TweakVersion = "0.0.0";

            ConfigX = 400.0f;
            ConfigY = 400.0f;
            ConfigKeyBind = new KeyBind
            {
                Key = KeyCode.F9,
                Ctrl = true,
                Alt = false,
                Shift = true
            };
        }
    }
}
