using BiendeoCHLib.Settings;
using System;
using System.IO;
using System.Text;
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

        public MoveLyricsLabel Position;

        [XmlIgnore]
        public bool DraggableLabelsEnabled;
        [XmlIgnore]
        public bool ConfigWindowEnabled;
        [XmlIgnore]
        private bool wasMouseVisible;
        [XmlIgnore]
        public int LayoutIndexSelected;

        private static MoveLyricsLabel DefaultPosition
        {
            get
            {
                return new MoveLyricsLabel
                {
                    X = 0,
                    Y = 0
                };
            }
        }
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

            Enabled = true;
            EnabledKeyBind = new KeyBind
            {
                Key = KeyCode.F9,
                Ctrl = false,
                Alt = false,
                Shift = false
            };
        }

        public static Config LoadConfig(string configPath)
        {
            var configFilePath = new FileInfo(configPath);
            if (configFilePath.Exists)
            {
                var configString = File.ReadAllText(configFilePath.FullName);
                var serializer = new XmlSerializer(typeof(Config));
                using (var configIn = new MemoryStream(Encoding.Unicode.GetBytes(configString)))
                {
                    return serializer.Deserialize(configIn) as Config;
                } 
            }
            else
            {
                var c = new Config();
                c.Position = DefaultPosition;
                c.SaveConfig(configPath);
                return c;
            }
        }
        public void SaveConfig(string configPath)
        {
            var configFilePath = new FileInfo(configPath);
            var serializer = new XmlSerializer(typeof(Config));
            using (var configOut = configFilePath.Open(FileMode.Create))
            {
                serializer.Serialize(configOut, this);
            }
        }

        public void HandleInput()
        {
            if (ConfigKeyBind.IsPressed && !ConfigKeyBind.JustSet)
            {
                ConfigWindowEnabled = !ConfigWindowEnabled;
                if (ConfigWindowEnabled)
                {
                    wasMouseVisible = Cursor.visible;
                    Cursor.visible = true;
                }
                else
                {
                    if (!wasMouseVisible) Cursor.visible = false;
                }
            }
            if (EnabledKeyBind.IsPressed && !EnabledKeyBind.JustSet)
            {
                Enabled = !Enabled;
            }
            ConfigKeyBind.JustSet = false;
            EnabledKeyBind.JustSet = false;
        }
        public void DrawLabelWindows()
        {
            if (DraggableLabelsEnabled)
            {
                Position.DrawLabelWindow(Position.WindowId);
            }
        }
    }
}
