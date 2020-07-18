using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HighwayRaise.Settings;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Reflection;

namespace HighwayRaise
{
    class HighwayRaise : MonoBehaviour
    {
        private bool sceneChanged;
        private Quaternion curQuat;
        private float update;
        private int camCount;
        private bool cameraPan = false;
        private float panTime;

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

        private Config config;
        private Rect ChangelogRect;
        private Rect ConfigRect;

        private int configID;
        private int changelogID;

        #region UnityMethods
        void Start()
        {
            config = Config.LoadConfig();

            SceneManager.activeSceneChanged += delegate (Scene _, Scene __) {
                sceneChanged = true;
            };
            sceneChanged = true;

        }
        void LateUpdate()
        {
            if(sceneChanged)
            {
                sceneChanged = false;
                if (SceneManager.GetActiveScene().name == "Gameplay")
                {
                    curQuat = Camera.main.transform.rotation;

                    cameraPan = true;
                    camCount = 0;
                    update = 0f;
                    panTime = config.PanSpeed / 1000f;
                }
            }
            config.HandleInput();

            if (SceneManager.GetActiveScene().name == "Gameplay" && cameraPan && config.Enabled)
            {
                float angle = 7-(1/ panTime *update* 13) + curQuat.eulerAngles.x;
                Quaternion newRotation = Quaternion.AngleAxis(angle, Vector3.left);
                foreach(Camera cam in Camera.allCameras)
                {
                    cam.transform.rotation = Quaternion.Slerp(Camera.current.transform.rotation, newRotation, 1f);
                }

                camCount += 1;

                if (update >= panTime)
                {
                    cameraPan = false;
                }
                else
                {
                    update += Time.deltaTime;
                }
            }
        }
        void Update()
        {

        }
        void OnGUI()
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

                ConfigRect = new Rect(config.ConfigX, config.ConfigY, 320.0f, 500.0f);
                configID = ConfigRect.GetHashCode();
                changelogID = ChangelogRect.GetHashCode();
            }

            if (config.ConfigWindowEnabled)
            {
                config.DrawLabelWindows();
                ConfigRect.x = config.ConfigX;
                ConfigRect.y = config.ConfigY;
                var outputRect = GUILayout.Window(configID, ConfigRect, OnWindow, new GUIContent("Highway Raise Settings"), settingsWindowStyle);
                config.ConfigX = outputRect.x;
                config.ConfigY = outputRect.y;
            }
            if (config.TweakVersion != FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion && !config.SeenChangelog)
            {
                ChangelogRect = GUILayout.Window(changelogID, ChangelogRect, OnChangelogWindow, new GUIContent($"Twitch Chat Changelog"), settingsWindowStyle);
            }
        }
        #endregion

        private void OnWindow(int id)
        {
            var largeLabelStyle = new GUIStyle
            {
                fontSize = 20,
                alignment = TextAnchor.UpperLeft,
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState
                {
                    textColor = Color.white,
                }
            };
            var smallLabelStyle = new GUIStyle
            {
                fontSize = 14,
                alignment = TextAnchor.UpperLeft,
                normal = new GUIStyleState
                {
                    textColor = Color.white,
                }
            };
            settingsScrollPosition = GUILayout.BeginScrollView(settingsScrollPosition);
            config.ConfigureGUI(new Common.Settings.GUIConfigurationStyles
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
            });
            GUILayout.Space(25.0f);

            GUILayout.Label($"Highway Raise v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}");
            GUILayout.Label("Tweak by Joosthoi1");
            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

        private void OnChangelogWindow(int id)
        {
            var largeLabelStyle = new GUIStyle(settingsLabelStyle)
            {
                fontSize = 20,
                alignment = TextAnchor.UpperLeft,
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState
                {
                    textColor = Color.white,
                },
                wordWrap = false
            };
            var smallLabelStyle = new GUIStyle(settingsLabelStyle)
            {
                fontSize = 14,
                alignment = TextAnchor.UpperLeft,
                normal = new GUIStyleState
                {
                    textColor = Color.white,
                },
                wordWrap = true
            };
            GUILayout.Label("Thank you for downloading Highway Raise!", largeLabelStyle);
            GUILayout.Label("To get started, press F11 to enable/disable Highway Raise.", smallLabelStyle);
            GUILayout.Label("Press Ctrl + Shift + F11 to enable/disable the config window.", smallLabelStyle);
            GUILayout.Label("Please make sure to press the \"Save Config\" button at the bottom of the config window so that your settings are saved for the next time you run Clone Hero.", smallLabelStyle);

            GUILayout.Space(15.0f);

            GUILayout.Label("Changelog", largeLabelStyle);
            GUILayout.Label("Added the camera pan for all players", largeLabelStyle);
            GUILayout.Space(15.0f);
            GUILayout.Label($"Highway Raise v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}");

            if (GUILayout.Button("Close this window", settingsButtonStyle))
            {
                config.SeenChangelog = true;
                config.TweakVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
                config.SaveConfig();
            }
            GUI.DragWindow();
        }
    }
}
