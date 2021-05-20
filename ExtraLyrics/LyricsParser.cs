using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ExtraLyrics
{
    class LyricsParser
    {
        private string fileName;
        public LyricsParser(String fileName)
        {
            this.fileName = fileName;
        }
        public List<List<ChartCommand>> ParseLyrics()
        {
            List<List<ChartCommand>> commands = new List<List<ChartCommand>>();
            string[] commandNames = new string[] { "lyric", "phrase_start", "phrase_end" };
            commands.Add(this.ParseChart(new string[] { "2lyric", "2phrase_start", "2phrase_end"}, commandNames));
            commands.Add(this.ParseChart(new string[] { "3lyric", "3phrase_start", "3phrase_end" }, commandNames ));
            commands.Add(this.ParseChart(new string[] { "4lyric", "4phrase_start", "4phrase_end" }, commandNames));

            return commands;
        }

        public List<ChartCommand> ParseChart(string[] acceptedCommands)
        {
            string line;
            bool readingEvents = false;
            bool readingSong = false;
            bool readingSyncTrack = false;
            float resolution = 192f;
            float bpm = 120000f;

            List<ChartCommand> commands = new List<ChartCommand>();

            StreamReader file = new StreamReader(this.fileName);

            while ((line = file.ReadLine()) != null)
            {
                
                if (readingEvents)
                {
                    if (line != "{")
                    {
                        if (line == "}")
                        {
                            readingEvents = false;
                        }
                        else
                        {
                            string[] splitted = line.Trim(' ').Split('"');

                            string[] spaceSplitted = splitted[1].Split(' ');
                            string commandString = spaceSplitted[0];
                            if (acceptedCommands.Contains(commandString))
                            {
                                int tick = Int32.Parse(splitted[0].Split(' ')[0]);
                                float timeInMs = tick / resolution * 60.0f / bpm;
                                string parameter = String.Join(" ", spaceSplitted.Skip(1).ToArray());


                                ChartCommand command = new ChartCommand()
                                {
                                    Tick = tick,
                                    TimeInMs = timeInMs,
                                    Command = commandString,
                                    Parameter = parameter,
                                    OriginalCommand = line
                                };
                                commands.Add(command);
                            }
                            
                        }
                    }
                }
                if (readingSong && line != "{")
                {
                    if (line == "}")
                    {
                        readingSong = false;
                    }
                    else
                    {
                        string[] splitted = line.Split('=');
                        if (splitted[0].Trim(' ') == "Resolution")
                        {
                            resolution = float.Parse(splitted[1].Trim(' '));
                        }
                    }
                }
                if (readingSyncTrack && line != "{")
                {
                    if (line == "}")
                    {
                        readingSyncTrack = false;
                    }
                    else
                    {
                        string[] splitted = line.Split('=')[1].Trim(' ').Split(' ');
                        if (splitted[0] == "B")
                        {
                            bpm = float.Parse(splitted[1]);
                        }
                    }
                }

                if (line == "[Events]")
                {
                    readingEvents = true;
                }
                if (line == "[Song]")
                {
                    readingSong = true;
                }
                if (line == "[SyncTrack]")
                {
                    readingSyncTrack = true;
                }
            }

            file.Close();
            return commands;
        }

        public List<ChartCommand> ParseChart(string[] acceptedCommands, string[] commandNames)
        {
            string line;
            bool readingEvents = false;
            bool readingSong = false;
            bool readingSyncTrack = false;
            float resolution = 192f;
            float bpm = 120000f;

            List<ChartCommand> commands = new List<ChartCommand>();

            StreamReader file = new StreamReader(this.fileName);

            while ((line = file.ReadLine()) != null)
            {

                if (readingEvents)
                {
                    if (line != "{")
                    {
                        if (line == "}")
                        {
                            readingEvents = false;
                        }
                        else
                        {
                            string[] splitted = line.Trim(' ').Split('"');

                            string[] spaceSplitted = splitted[1].Split(' ');
                            string commandString = spaceSplitted[0];
                            for (int i = 0; i < acceptedCommands.Length; i++)
                            {
                                if (acceptedCommands[i] == commandString)
                                {
                                    long tick = long.Parse(splitted[0].Split(' ')[0]);
                                    double timeInMs = tick / resolution * 60.0d / bpm * 1000000d;
                                    string parameter = String.Join(" ", spaceSplitted.Skip(1).ToArray());

                                    ChartCommand command = new ChartCommand()
                                    {
                                        Tick = tick,
                                        TimeInMs = timeInMs,
                                        Command = commandNames[i],
                                        Parameter = parameter,
                                        OriginalCommand = line
                                    };
                                    commands.Add(command);
                                    break;
                                }
                            }

                        }
                    }
                }
                if (readingSong && line != "{")
                {
                    if (line == "}")
                    {
                        readingSong = false;
                    }
                    else
                    {
                        string[] splitted = line.Split('=');
                        if (splitted[0].Trim(' ') == "Resolution")
                        {
                            resolution = float.Parse(splitted[1].Trim(' '));
                        }
                    }
                }
                if (readingSyncTrack && line != "{")
                {
                    if (line == "}")
                    {
                        readingSyncTrack = false;
                    }
                    else
                    {
                        string[] splitted = line.Split('=')[1].Trim(' ').Split(' ');
                        if (splitted[0] == "B")
                        {
                            bpm = float.Parse(splitted[1]);
                        }
                    }
                }

                if (line == "[Events]")
                {
                    readingEvents = true;
                }
                if (line == "[Song]")
                {
                    readingSong = true;
                }
                if (line == "[SyncTrack]")
                {
                    readingSyncTrack = true;
                }
            }

            file.Close();
            return commands;
        }
    }
}
