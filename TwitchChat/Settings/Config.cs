using Common.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace TwitchChat.Settings
{
    [Serializable]
    public class Config : IGUIConfigurable
    {
        public int Version;
        public string TweakVersion;
        public bool SilenceUpdates;

        public float ConfigX;
        public float ConfigY;
        public KeyBind ConfigKeyBind;

        public bool Enabled;
        public KeyBind EnabledKeyBind;

        public int FontSize;
        public string TwitchChannel;

        public float UpdateSpeed;
        public float ChatHeight;
        public float ChatWidth;

        public ColorablePositionableLabel DisplayImage;

        [XmlIgnore]
        public bool LayoutTest;
        [XmlIgnore]
        private bool DraggableLabelsEnabled;
        [XmlIgnore]
        public bool ConfigWindowEnabled;
        [XmlIgnore]
        public bool SeenChangelog;
        [XmlIgnore]
        private bool wasMouseVisible;

        public Config()
        {
            Version = 1;
            TweakVersion = "3.0.0";
            SilenceUpdates = false;

            ConfigX = 100.0f;
            ConfigY = 100.0f;
            ConfigKeyBind = new KeyBind
            {
                Key = KeyCode.F10,
                Ctrl = true,
                Alt = false,
                Shift = true
            };

            Enabled = false;
            EnabledKeyBind = new KeyBind
            {
                Key = KeyCode.F10,
                Ctrl = false,
                Alt = false,
                Shift = false
            };

            FontSize = 15;
            TwitchChannel = "";
            ChatHeight = Screen.height;
            ChatWidth = Screen.width/5;
            UpdateSpeed = 1000.0f;

            DisplayImage = new ColorablePositionableLabel
            {
                Visible = true,
                X = (int)(30.0f * Screen.width / 1440.0f),
                Y = (int)(1415.0f * Screen.height / 1440.0f),
                Size = Screen.height * 50 / 1440,
                Bold = true,
                Italic = false,
                Alignment = TextAnchor.MiddleLeft,
                Color = new ColorARGB(Color.white)
            };
        }

        public static Config LoadConfig()
        {
            var configFilePath = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Tweaks", "TwitchChatConfig.xml"));
            
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
                c.SaveConfig();
                return c;
            }
        }
        public void SaveConfig()
        {
            var configFilePath = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Tweaks", "TwitchChatConfig.xml"));

            if (configFilePath.Exists) {
                configFilePath.Delete();
            }
            var serializer = new XmlSerializer(typeof(Config));
            using (var configOut = configFilePath.OpenWrite())
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
                    if (!wasMouseVisible)
                    {
                        Cursor.visible = false;
                    }
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
                DisplayImage.DrawLabelWindow(187001002);
            }
        }

        public void ConfigureGUI(GUIConfigurationStyles styles)
        {
            GUILayout.Label("Settings", styles.LargeLabel);
            Enabled = GUILayout.Toggle(Enabled, "Enabled", styles.Toggle);

            GUILayout.Label("Font Size", styles.SmallLabel);
            FontSize = (int)GUILayout.HorizontalSlider(FontSize, 0.0f, 100.0f, styles.HorizontalSlider, styles.HorizontalSliderThumb);
            if (int.TryParse(GUILayout.TextField(FontSize.ToString(), styles.TextField), out int fontSize)) FontSize = fontSize;

            GUILayout.Label("Twitch Channel", styles.SmallLabel);
            TwitchChannel = GUILayout.TextField(TwitchChannel, styles.TextField);

            GUILayout.Label("Update Speed (ms)", styles.SmallLabel);
            UpdateSpeed = (int)GUILayout.HorizontalSlider(UpdateSpeed, 0.0f, 10000.0f, styles.HorizontalSlider, styles.HorizontalSliderThumb);
            if (float.TryParse(GUILayout.TextField(UpdateSpeed.ToString(), styles.TextField), out float updateSpeed)) UpdateSpeed = updateSpeed;

            GUILayout.Label("Chat Width", styles.SmallLabel);
            ChatWidth = (int)GUILayout.HorizontalSlider(ChatWidth, 0.0f, Screen.width, styles.HorizontalSlider, styles.HorizontalSliderThumb);
            if (float.TryParse(GUILayout.TextField(ChatWidth.ToString(), styles.TextField), out float chatWidth)) ChatWidth = chatWidth;

            GUILayout.Label("Chat Height", styles.SmallLabel);
            ChatHeight = (int)GUILayout.HorizontalSlider(ChatHeight, 0.0f, Screen.height, styles.HorizontalSlider, styles.HorizontalSliderThumb);
            if (float.TryParse(GUILayout.TextField(ChatHeight.ToString(), styles.TextField), out float chatHeight)) ChatHeight = chatHeight;

            GUILayout.Space(25.0f);
            GUILayout.Label("Enable/Disable Keybind", styles.LargeLabel);
            EnabledKeyBind.ConfigureGUI(styles);

            GUILayout.Space(25.0f);
            GUILayout.Label("Configuration Keybind", styles.LargeLabel);
            ConfigKeyBind.ConfigureGUI(styles);

            GUILayout.Space(25.0f);
            if (GUILayout.Button("Save Config", styles.Button))
            {
                SaveConfig();
            }
        }

    }
}