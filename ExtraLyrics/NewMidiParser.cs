using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtraLyrics;

namespace NewMidiParser
{
    class MidiParser
    {
        private string path;

        public MidiParser(string path)
        {
            this.path = path;
        }
        public List<List<ChartCommand>> ParseMidi()
        {
            List<List<ChartCommand>> commands = new List<List<ChartCommand>>();
            commands.Add(new List<ChartCommand>());
            commands.Add(new List<ChartCommand>());
            commands.Add(new List<ChartCommand>());
            int commandIndex = 0;

            MidiFile midiFile = MidiFile.Read(this.path);

            TempoMap tempoMap = midiFile.GetTempoMap();

            foreach (var x in midiFile.GetTrackChunks())
            {
                string trackName = "";

                using (TimedEventsManager timedEventsManager = x.ManageTimedEvents()) {
                    foreach (var e in timedEventsManager.Events)
                    {
                        if (e.Event is SequenceTrackNameEvent)
                        {
                            //Console.WriteLine(((SequenceTrackNameEvent)e.Event).Text);
                            trackName = ((SequenceTrackNameEvent)e.Event).Text;
                            switch (trackName)
                            {
                                case ("PART VOCALS"):
                                    commandIndex = -1;
                                    break;
                                case ("HARM1"):
                                    commandIndex = 0;
                                    break;
                                case ("HARM2"):
                                    commandIndex = 1;
                                    break;

                                case ("HARM3"):
                                    commandIndex = 2;
                                    break;

                            }
                        }
                        if (commandIndex != -1)
                        {
                            if (e.Event is NoteOnEvent)
                            {
                                if (((NoteOnEvent)e.Event).NoteNumber == 105)
                                {
                                    ChartCommand noteOnCommand =
                                            new ChartCommand()
                                            {
                                                Command = "phrase_start",
                                                Parameter = "",
                                                Tick = (long)e.Time,
                                                OriginalCommand = "",
                                                TimeInMs = e.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000d
                                            };
                                    commands[commandIndex].Add(noteOnCommand);
                                    if (commandIndex == 1)
                                    {
                                        commands[2].Add(noteOnCommand);
                                    }
                                }
                            }
                            if (e.Event is NoteOffEvent)
                            {
                                if (((NoteOffEvent)e.Event).NoteNumber == 105)
                                {
                                    ChartCommand noteOffCommand =
                                            new ChartCommand()
                                            {
                                                Command = "phrase_end",
                                                Parameter = "",
                                                Tick = (long)e.Time,
                                                OriginalCommand = "",
                                                TimeInMs = e.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000d
                                            };
                                    commands[commandIndex].Add(noteOffCommand);
                                    if (commandIndex == 1)
                                    {
                                        commands[2].Add(noteOffCommand);
                                    }
                                }
                            }
                        }

                        if (e.Event is Melanchall.DryWetMidi.Core.TextEvent)
                        {
                            if (trackName.StartsWith("HARM"))
                            {
                                string line = ((Melanchall.DryWetMidi.Core.TextEvent)e.Event).Text;
                                if (!line.StartsWith("[") && !line.EndsWith("]"))
                                {
                                    if (line != "+")
                                    {
                                        MetricTimeSpan metricTime = e.TimeAs<MetricTimeSpan>(tempoMap);
                                        double timeInMs = metricTime.TotalMicroseconds / 1000d;
                                        var curCommand = new ChartCommand()
                                        {
                                            Tick = (int)e.Time,
                                            TimeInMs = timeInMs,
                                            Command = "lyric",
                                            Parameter = line,
                                            OriginalCommand = ""
                                        };
                                        commands[commandIndex].Add(curCommand);

                                    }
                                }
                            }
                        }
                        /*
                        if (e.EventType == MidiEventType.Text)
                        {
                            Console.WriteLine(((TextEvent)e).Text);
                        }
                        */
                    }
                }

                
            }
            commands[0] = commands[0].OrderBy(o => o.TimeInMs).ToList();
            commands[1] = commands[1].OrderBy(o => o.TimeInMs).ToList();
            commands[2] = commands[2].OrderBy(o => o.TimeInMs).ToList();
            return commands;
        }
    }
}
