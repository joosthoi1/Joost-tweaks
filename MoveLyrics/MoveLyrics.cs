using BepInEx;
using BiendeoCHLib;
using BiendeoCHLib.Patches;
using BiendeoCHLib.Wrappers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MoveLyrics.Settings;
using static UnityEngine.GUI;
using BiendeoCHLib.Settings;

namespace MoveLyrics
{
    [BepInPlugin("com.joosthoi1.movelyrics", "Move Lyrics", "1.0.0")]
    [BepInDependency("com.biendeo.biendeochlib")]
    class MoveLyrics : BaseUnityPlugin
    {
        public static MoveLyrics Instance { get; private set; }

        private GameManagerWrapper gameManager;

        private bool sceneChanged;

        private Transform lyricsTransform;

        private string ConfigPath => Path.Combine(Paths.ConfigPath, Info.Metadata.GUID + ".layout.xml");
        private Config config;

        private GUIStyle settingsWindowStyle;
        private GUIStyle settingsToggleStyle;
        private GUIStyle settingsButtonStyle;
        private GUIStyle settingsTextAreaStyle;
        private GUIStyle settingsTextFieldStyle;
        private GUIStyle settingsLabelStyle;
        private GUIStyle settingsBoxStyle;
        private GUIStyle settingsHorizontalSliderStyle;
        private GUIStyle settingsHorizontalSliderThumbStyle;

        private Vector2 settingsScrollPosition;
        private bool currentlyEditing = false;

        private Harmony Harmony;
        public MoveLyrics()
        {
            Instance = this;
            Harmony = new Harmony("com.joosthoi1.movelyrics");
            PatchBase.InitializePatches(Harmony, Assembly.GetExecutingAssembly(), Logger);
        }

        ~MoveLyrics()
        {
            Harmony.UnpatchAll();
        }

        public void Awake()
        {
        }

        public void Start()
        {
            config = Settings.Config.LoadConfig(ConfigPath);
            SceneManager.activeSceneChanged += delegate (Scene _, Scene __) {
                sceneChanged = true;
            };
        }
        public void LateUpdate()
        {
            string sceneName = SceneManager.GetActiveScene().name;

            if (this.sceneChanged && config.Enabled)
            {
                Logger.LogDebug("Scene changed");
                this.sceneChanged = false;
                if (sceneName == "Gameplay")
                {
                    lyricsTransform = GameObject.Find("Lyrics").transform;

                    lyricsTransform.position = new Vector2(config.Position.X, (float)Screen.height / 1.65f - (float)config.Position.Y);
                }
            }
            if (sceneName == "Gameplay")
            {
            }
            config.HandleInput();
        }
        public void OnGUI()
        {
            if (settingsWindowStyle is null)
            {
                settingsWindowStyle = new GUIStyle(GUI.skin.window);
                settingsToggleStyle = new GUIStyle(GUI.skin.toggle);
                settingsButtonStyle = new GUIStyle(GUI.skin.button);
                settingsTextAreaStyle = new GUIStyle(GUI.skin.textArea);
                settingsTextFieldStyle = new GUIStyle(GUI.skin.textField);
                settingsLabelStyle = new GUIStyle(GUI.skin.label);
                settingsBoxStyle = new GUIStyle(GUI.skin.box);
                settingsHorizontalSliderStyle = new GUIStyle(GUI.skin.horizontalSlider);
                settingsHorizontalSliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb);
            }
            if (config.ConfigWindowEnabled)
            {
                config.DrawLabelWindows();
                var outputRect = GUILayout.Window(187000040, new Rect(config.ConfigX, config.ConfigY, 320.0f, 807.0f), OnWindow, new GUIContent("Extra Song UI Settings"), settingsWindowStyle);
                config.ConfigX = outputRect.x;
                config.ConfigY = outputRect.y;
                if (!(lyricsTransform == null))
                {
                    lyricsTransform.position = new Vector2(config.Position.X, (float)Screen.height/1.65f- (float)config.Position.Y);
                }
            }
        }

		private void OnWindow(int id)
		{
			var smallLabelStyle = new GUIStyle
			{
				fontSize = 14,
				alignment = TextAnchor.UpperLeft,
				normal = new GUIStyleState
				{
					textColor = Color.white,
				}
			};
			var largeLabelStyle = new GUIStyle
			{
				fontSize = 20,
				alignment = TextAnchor.UpperLeft,
				normal = new GUIStyleState
				{
					textColor = Color.white,
				}
			};
			var styles = new GUIConfigurationStyles
			{
				LargeLabel = largeLabelStyle,
				SmallLabel = smallLabelStyle,
				Window = settingsWindowStyle,
				Toggle = settingsToggleStyle,
				Button = settingsButtonStyle,
				TextArea = settingsTextAreaStyle,
				TextField = settingsTextFieldStyle,
				Label = settingsLabelStyle,
				Box = settingsBoxStyle,
				HorizontalSlider = settingsHorizontalSliderStyle,
				HorizontalSliderThumb = settingsHorizontalSliderThumbStyle
			};
            if (!currentlyEditing)
            {
                settingsScrollPosition = GUILayout.BeginScrollView(settingsScrollPosition);
                GUILayout.Label("Settings", largeLabelStyle);

                GUILayout.Space(25.0f);
                GUILayout.Label("Enable/Disable Keybind", largeLabelStyle);
                config.EnabledKeyBind.ConfigureGUI(styles);

                GUILayout.Space(25.0f);
                GUILayout.Label("Configuration Keybind", largeLabelStyle);
                config.ConfigKeyBind.ConfigureGUI(styles);

                GUILayout.Space(25.0f);
                GUILayout.Label("Move lyrics", largeLabelStyle);
                config.DraggableLabelsEnabled = GUILayout.Toggle(config.DraggableLabelsEnabled, "Draggable Label", styles.Toggle);
                GUILayout.Label("Allows you to drag the lyrics with a window", new GUIStyle(styles.SmallLabel)
                {
                    fontStyle = FontStyle.Italic
                });

                config.Position.DraggableWindowsEnabled = config.DraggableLabelsEnabled;
                GUILayout.Space(20.0f);

                currentlyEditing = GUILayout.Button($"Edit position", styles.Button);

                GUILayout.Space(20.0f);
                if (GUILayout.Button("Save Config", settingsButtonStyle))
                {
                    config.SaveConfig(ConfigPath);
                }
                GUILayout.Space(50.0f);

                GUILayout.Label($"Move Lyrics v{typeof(MoveLyrics).Assembly.GetName().Version}");
                GUILayout.Label("Tweak by Joosthoi1");
                GUILayout.Label("Thank you for using this!");
                GUILayout.EndScrollView();
            } else {
                config.Position.ConfigureGUI(styles);
                if (GUILayout.Button("Back", styles.Button))
                {
                    currentlyEditing = false;
                }
            }

            GUI.DragWindow();
		}
	}
}
