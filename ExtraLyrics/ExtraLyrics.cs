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
using static UnityEngine.GUI;

namespace ExtraLyrics
{
    [BepInPlugin("com.joosthoi1.extralyrics", "Extra Lyrics", "1.0.0")]
    [BepInDependency("com.biendeo.biendeochlib")]
    class ExtraLyrics : BaseUnityPlugin
    {
        public static ExtraLyrics Instance { get; private set; }

        private GameManagerWrapper gameManager;
        private Transform lyricsTransform;
        private TextMeshProUGUI altLyrics;
        private Font uiFont;

        List<LyricsCommand> commands;
        private int index;
        private string altLyricText;
        private List<int> indexes;
        private int colorIndex;

        private bool sceneChanged;

        private Harmony Harmony;

        public ExtraLyrics()
        {
            Instance = this;
            Harmony = new Harmony("com.joosthoi1.bepinexdebug");
            PatchBase.InitializePatches(Harmony, Assembly.GetExecutingAssembly(), Logger);
        }

        ~ExtraLyrics()
        {
            Harmony.UnpatchAll();
        }

        public void Start()
        {
            SceneManager.activeSceneChanged += delegate (Scene _, Scene __) {
                sceneChanged = true;
            };
        }
        public void LateUpdate()
        {
            if (uiFont is null && SceneManager.GetActiveScene().name.Equals("Main Menu"))
            {
                uiFont = GameObject.Find("Profile Title").GetComponent<Text>().font;
            }
            if (sceneChanged)
            {
                sceneChanged = false;
                if (SceneManager.GetActiveScene().name.Equals("Gameplay"))
                {
                    var gameManagerObject = GameObject.Find("Game Manager");
                    gameManager = GameManagerWrapper.Wrap(GameObject.Find("Game Manager")?.GetComponent<GameManager>());
                    if (gameManager.GlobalVariables.SongEntry.SongEntry.lyrics && gameManager.GlobalVariables.SongEntry.SongEntry.chartPath.EndsWith("notes.chart"))
                    {

                        string file = gameManager.GlobalVariables.SongEntry.SongEntry.chartPath;
                        LyricsParser parser = new LyricsParser(file);

                        commands = parser.ParseLyrics(new string[] { "2lyric", "2phrase_start", "2phrase_end" });
                        index = 0;

                        

                        Logger.LogDebug(gameManager.GlobalVariables.SongEntry.SongEntry.chartPath);

                        lyricsTransform = GameObject.Find("Lyrics").transform;

                        altLyrics = CreateTmpText(lyricsTransform, "AltLyrics", 1);
                        altLyrics.fontSize = 44f;
                        Logger.LogDebug(altLyrics.fontSize);

                        altLyrics.transform.localPosition = new Vector2(0, 460);
                        altLyrics.horizontalAlignment = HorizontalAlignmentOptions.Center;

                        altLyrics.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2, altLyrics.preferredHeight);
                    }
                    else
                    {
                        commands = new List<LyricsCommand>();
                    }
                    //UpdateTextButton(altLyrics, "TEST", 40, Color.white);
                }
            }
            if (SceneManager.GetActiveScene().name.Equals("Gameplay"))
            {
                if (index < commands.Count)
                {
                    Logger.LogDebug(commands[index].TimeInMs);
                    if (TimeSpan.FromSeconds(gameManager.SongTime).TotalMilliseconds >= commands[index].TimeInMs*1000)
                    {
                        LyricsCommand curCommand = commands[index++];


                        if (curCommand.Command == "2phrase_start")
                        {
                            ClearLyric();
                            int i = 0;
                            LyricsCommand x;
                            while ((index + i) < commands.Count && (x = commands[index + i++]).Command == "2lyric")
                            {
                                altLyricText += x.Parameter;
                                altLyricText += x.NoNewLine ? "" : " ";
                                indexes.Add(altLyricText.Length);
                            }
                            altLyrics.text = altLyricText;
                        }
                        else if (curCommand.Command == "2phrase_end")
                        {
                            ClearLyric();
                        }
                        else
                        {
                            altLyrics.text = "<color=#0000ff>" + altLyricText.Substring(0, indexes[colorIndex]) + "</color>" + altLyricText.Substring(indexes[colorIndex++]);
                        }
                        //altLyrics.text = $"{curCommand.Command} {curCommand.Parameter}";
                    }
                }
            }
        }
        private void ClearLyric()
        {
            altLyrics.text = "";
            altLyricText = "";
            indexes = new List<int>();
            colorIndex = 0;
        }
        public void Update()
        {
            
        }
        private TextMeshProUGUI CreateTmpText(Transform parent, string labelName, int height)
        {
            var o = new GameObject(labelName, new Type[] {
                        typeof(TextMeshProUGUI)
                    });
            o.transform.SetParent(parent);
            o.transform.SetSiblingIndex(height);
            o.transform.localPosition = new Vector3(0, 0);

            TextMeshProUGUI text = o.GetComponent<TextMeshProUGUI>();
            text.font = TMP_FontAsset.CreateFontAsset(uiFont);
            text.alignment = TextAlignmentOptions.MidlineJustified;
            text.color = Color.white;

            return text;
        }
    }
}
