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
using NewMidiParser;

namespace ExtraLyrics
{
    [BepInPlugin("com.joosthoi1.extralyrics", "Extra Lyrics", "1.0.0")]
    [BepInDependency("com.biendeo.biendeochlib")]
    class ExtraLyrics : BaseUnityPlugin
    {
        public static ExtraLyrics Instance { get; private set; }

        private GameManagerWrapper gameManager;
        private Transform lyricsTransform;
        private Font uiFont;

        private TextMeshProUGUI altLyrics0;
        List<ChartCommand> commands0;
        private int index0;
        private string altLyricText0;
        private List<int> indexes0;
        private int colorIndex0;
        
        private TextMeshProUGUI altLyrics1;
        List<ChartCommand> commands1;
        private int index1;
        private string altLyricText1;
        private List<int> indexes1;
        private int colorIndex1;
        
        private TextMeshProUGUI altLyrics2;
        List<ChartCommand> commands2;
        private int index2;
        private string altLyricText2;
        private List<int> indexes2;
        private int colorIndex2;

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
                    if (gameManager.GlobalVariables.SongEntry.SongEntry.lyrics && gameManager.GlobalVariables.SongEntry.SongEntry.chartPath.EndsWith("notes.mid"))
                    {

                        string file = gameManager.GlobalVariables.SongEntry.SongEntry.chartPath;
                        MidiParser parser = new MidiParser(file);

                        List<List<ChartCommand>> coms = parser.ParseMidi();
                        commands0 = coms[0];
                        commands1 = coms[1];
                        commands2 = coms[2];

                        Logger.LogDebug(commands2.Count);
                        foreach (ChartCommand com in commands2)
                        {
                            Logger.LogDebug($"{com.TimeInMs} {com.Command} {com.Parameter}");
                        }
                        index0 = 0;
                        index1 = 0;
                        index2 = 0;

                        //Logger.LogDebug(gameManager.GlobalVariables.SongEntry.SongEntry.chartPath);

                        lyricsTransform = GameObject.Find("Lyrics").transform;

                        altLyrics0 = CreateTmpText(lyricsTransform, "AltLyrics1", 3);
                        altLyrics1 = CreateTmpText(lyricsTransform, "AltLyrics2", 2);
                        altLyrics2 = CreateTmpText(lyricsTransform, "AltLyrics3", 1);

                        altLyrics0.fontSize = 44f;
                        altLyrics1.fontSize = 44f;
                        altLyrics2.fontSize = 44f;

                        altLyrics0.transform.localPosition = new Vector2(0, 460);
                        altLyrics1.transform.localPosition = new Vector2(0, 300);
                        altLyrics2.transform.localPosition = new Vector2(0, 240);
                        altLyrics0.horizontalAlignment = HorizontalAlignmentOptions.Center;
                        altLyrics1.horizontalAlignment = HorizontalAlignmentOptions.Center;
                        altLyrics2.horizontalAlignment = HorizontalAlignmentOptions.Center;


                        altLyrics0.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2, altLyrics0.preferredHeight);
                        altLyrics1.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2, altLyrics1.preferredHeight);
                        altLyrics2.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2, altLyrics2.preferredHeight);
                        indexes0 = new List<int>();
                        indexes1 = new List<int>();
                        indexes2 = new List<int>();
                        ClearLyric0();
                        ClearLyric1();
                        ClearLyric2();
                    }
                    else if (gameManager.GlobalVariables.SongEntry.SongEntry.lyrics && gameManager.GlobalVariables.SongEntry.SongEntry.chartPath.EndsWith("notes.chart"))
                    {
                        string file = gameManager.GlobalVariables.SongEntry.SongEntry.chartPath;
                        LyricsParser parser = new LyricsParser(file);

                        commands0 = parser.ParseLyrics()[0];
                        index0 = 0;

                        Logger.LogDebug(gameManager.GlobalVariables.SongEntry.SongEntry.chartPath);

                        lyricsTransform = GameObject.Find("Lyrics").transform;

                        altLyrics0 = CreateTmpText(lyricsTransform, "AltLyrics1", 1);
                        altLyrics0.fontSize = 44f;

                        altLyrics0.transform.localPosition = new Vector2(0, 460);
                        altLyrics0.horizontalAlignment = HorizontalAlignmentOptions.Center;

                        altLyrics0.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2, altLyrics0.preferredHeight);
                        indexes0 = new List<int>();

                        ClearLyric0();
                        ClearLyric1();
                        ClearLyric2();

                    }
                    else
                    {
                        commands0 = new List<ChartCommand>();
                    }
                    //UpdateTextButton(altLyrics, "TEST", 40, Color.white);
                }
            }
            if (SceneManager.GetActiveScene().name.Equals("Gameplay"))
            {
                
                if (index0 < commands0.Count)
                {
                    if (TimeSpan.FromSeconds(gameManager.SongTime).TotalMilliseconds >= commands0[index0].TimeInMs)
                    {
                        ChartCommand curCommand = commands0[index0++];

                        if (curCommand.Command == "phrase_start")
                        {
                            ClearLyric0();
                            int i = 0;
                            ChartCommand x;
                            while ((index0 + i) < commands0.Count && (x = commands0[index0 + i++]).Command == "lyric")
                            {
                                altLyricText0 += x.Parameter;

                                if (altLyricText0.EndsWith("="))
                                {
                                    altLyricText0 = altLyricText0.TrimEnd('=') + "-";
                                }
                                else if (altLyricText0.EndsWith("$") || altLyricText0.EndsWith("#"))
                                {
                                    altLyricText0 = altLyricText0.TrimEnd(new char[] { '$', '#' });
                                }
                                else if (altLyricText0.EndsWith("-"))
                                {
                                    altLyricText0 = altLyricText0.TrimEnd('-');
                                }
                                else
                                {
                                    altLyricText0 += " ";
                                }
                                indexes0.Add(altLyricText0.Length);
                            }
                            altLyrics0.text = altLyricText0;
                        }
                        else if (curCommand.Command == "phrase_end")
                        {
                            ClearLyric0();
                        }
                        else
                        {
                            altLyrics0.text = "<color=#47a9ea>" + altLyricText0.Substring(0, indexes0[colorIndex0]) + "</color>" + altLyricText0.Substring(indexes0[colorIndex0++]);
                        }
                        //altLyrics.text = $"{curCommand.Command} {curCommand.Parameter}";
                    }
                }

                if (index1 < commands1.Count)
                {
                    if (TimeSpan.FromSeconds(gameManager.SongTime).TotalMilliseconds >= commands1[index1].TimeInMs)
                    {
                        ChartCommand curCommand = commands1[index1++];

                        if (curCommand.Command == "phrase_start")
                        {
                            ClearLyric1();
                            int i = 0;
                            ChartCommand x;
                            while ((index1 + i) < commands1.Count && (x = commands1[index1 + i++]).Command == "lyric")
                            {
                                altLyricText1 += x.Parameter;

                                if (altLyricText1.EndsWith("="))
                                {
                                    altLyricText1 = altLyricText1.TrimEnd('=') + "-";
                                }
                                else if (altLyricText1.EndsWith("$") || altLyricText1.EndsWith("#"))
                                {
                                    altLyricText1 = altLyricText1.TrimEnd(new char[] { '$', '#' });
                                }
                                else if (altLyricText1.EndsWith("-"))
                                {
                                    altLyricText1 = altLyricText1.TrimEnd('-');
                                }
                                else
                                {
                                    altLyricText1 += " ";
                                }
                                indexes1.Add(altLyricText1.Length);
                            }
                            altLyrics1.text = altLyricText1;
                        }
                        else if (curCommand.Command == "phrase_end")
                        {
                            ClearLyric1();
                        }
                        else
                        {
                            altLyrics1.text = "<color=#e59b2b>" + altLyricText1.Substring(0, indexes1[colorIndex1]) + "</color>" + altLyricText1.Substring(indexes1[colorIndex1++]);
                        }
                        //altLyrics.text = $"{curCommand.Command} {curCommand.Parameter}";
                    }
                }
                if (index2 < commands2.Count)
                {
                    if (TimeSpan.FromSeconds(gameManager.SongTime).TotalMilliseconds >= commands2[index2].TimeInMs)
                    {
                        Logger.LogDebug("TEST0");
                        ChartCommand curCommand = commands2[index2++];

                        Logger.LogDebug("TEST1");
                        if (curCommand.Command == "phrase_start")
                        {
                            ClearLyric2();
                            int i = 0;
                            ChartCommand x;
                            while ((index2 + i) < commands2.Count && (x = commands2[index2 + i++]).Command == "lyric")
                            {
                                Logger.LogDebug("TEST2");

                                altLyricText2 += x.Parameter;

                                if (altLyricText2.EndsWith("="))
                                {
                                    altLyricText2 = altLyricText2.TrimEnd('=') + "-";
                                }
                                else if (altLyricText2.EndsWith("$") || altLyricText2.EndsWith("#"))
                                {
                                    altLyricText2 = altLyricText2.TrimEnd(new char[] { '$', '#' });
                                }
                                else if (altLyricText2.EndsWith("-"))
                                {
                                    altLyricText2 = altLyricText2.TrimEnd('-');
                                }
                                else
                                {
                                    altLyricText2 += " ";
                                }
                                indexes2.Add(altLyricText2.Length);
                            }
                            altLyrics2.text = altLyricText2;
                        }
                        else if (curCommand.Command == "phrase_end")
                        {
                            ClearLyric2();
                        }
                        else
                        {
                            altLyrics2.text = "<color=#af3e1c>" + altLyricText2.Substring(0, indexes2[colorIndex2]) + "</color>" + altLyricText2.Substring(indexes2[colorIndex2++]);
                        }
                        //altLyrics.text = $"{curCommand.Command} {curCommand.Parameter}";
                    }
                }

            }
        }
        private void ClearLyric0()
        {
            altLyrics0.text = "";

            altLyricText0 = "";

            indexes0 = new List<int>();

            colorIndex0 = 0;

        }
        private void ClearLyric1()
        {
            altLyrics1.text = "";

            altLyricText1 = "";

            indexes1 = new List<int>();

            colorIndex1 = 0;

        }
        private void ClearLyric2()
        {
            altLyrics2.text = "";

            altLyricText2 = "";

            indexes2 = new List<int>();

            colorIndex2 = 0;

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
