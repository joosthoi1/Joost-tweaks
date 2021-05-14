﻿using System;
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
        public List<LyricsCommand> ParseLyrics(string[] acceptedCommands)
        {
            string line;
            bool readingEvents = false;
            bool readingSong = false;
            bool readingSyncTrack = false;
            float resolution = 0f;
            float bpm = 0f;

            List<LyricsCommand> commands = new List<LyricsCommand>();

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
                                bool noNewLine = parameter.EndsWith("-");
                                parameter = parameter.TrimEnd('-');

                                LyricsCommand command = new LyricsCommand()
                                {
                                    Tick = tick,
                                    TimeInMs = timeInMs,
                                    Command = commandString,
                                    Parameter = parameter,
                                    NoNewLine = noNewLine,
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
                            bpm = float.Parse(splitted[1])/1000;
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
