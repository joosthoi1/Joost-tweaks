using System;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Drawing;

namespace TwitchChat
{
    public class MessageRecievedArgs : EventArgs
    {
        public MessageRecievedArgs(Dictionary<string, string> messageDictionary)
        {
            MessageDictionary = messageDictionary;
        }
        public Dictionary<string, string> MessageDictionary { get; }
    }
    public class TwitchIRC
    {
        private readonly string _server;

        private readonly int _port;

        public string channel = "";
        public string currentChannel = "";


        private readonly int _maxRetries;

        public delegate void MessageRecievedHandler(object sender, MessageRecievedArgs e);
        public event MessageRecievedHandler MessageReceived;

        public static TcpClient irc;
        public static NetworkStream stream;
        public static StreamReader reader;
        public static StreamWriter writer;

        private string text;

        private static string regex = @"^:(\w+)!.*#(\w+)\s+:(.+)$";
        private static Regex rgx = new Regex(regex, RegexOptions.IgnoreCase);
        private static readonly List<string> defaultColors = new List<string>()
        {
            "#FF0000",
            "#0000FF",
            "#00FF00",
            "#B22222",
            "#FF7F50",
            "#9ACD32",
            "#FF4500",
            "#2E8B57",
            "#DAA520",
            "#D2691E",
            "#5F9EA0",
            "#1E90FF",
            "#FF69B4",
            "#8A2BE2",
            "#00FF7F"
        };


        public TwitchIRC(string server, int port, string channel, int maxRetries = 3)
        {
            _server = server;
            _port = port;
            this.channel = channel;
            currentChannel = channel;
            _maxRetries = maxRetries;
        }
        public void Initilize()
        {
            irc = new TcpClient(_server, _port);
            stream = irc.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);

            writer.WriteLine("NICK justinfan01234567891234");
            writer.Flush();
        }
        private void internalUpdate()
        {
            if (this.channel != currentChannel)
            {
                writer.WriteLine("PART #" + currentChannel.ToLower());
                writer.Flush();
                writer.WriteLine("JOIN #" + this.channel.ToLower());
                writer.Flush();

                reader.DiscardBufferedData();
                currentChannel = this.channel;
            }
            string inputLine;
            while (irc.Available > 0)
            {
                inputLine = reader.ReadLine();

                string[] splitInput = inputLine.Split(':')[1].Split(' ');

                if (splitInput[0] == "PING")
                {
                    string PongReply = splitInput[1];
                    writer.WriteLine("PONG " + PongReply);
                    writer.Flush();
                }
                
                switch (splitInput[1])
                {
                    case "001":
                        writer.WriteLine("JOIN #" + this.channel.ToLower());
                        writer.Flush();
                        writer.WriteLine("CAP REQ :twitch.tv/tags ");
                        writer.Flush();
                        currentChannel = this.channel;
                        break;
                    default:
                        break;

                }
                if (inputLine.Contains("PRIVMSG"))
                {
                    text = inputLine;
                    string regex = @"^(.*):(\w+)!.*#(\w+)\s+:(.+)$";
                    Regex rgx = new Regex(regex, RegexOptions.IgnoreCase);
                    string[] reMatches = rgx.Split(text);

                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    foreach (string v in reMatches[1].Split(';'))
                    {
                        string[] splitted = v.Split('=');
                        dict[splitted[0]] = (splitted.Length > 1) ? splitted[1] : null;
                    }
                    dict["message"] = reMatches[4];
                    dict["channel"] = reMatches[3];
                    if (String.IsNullOrEmpty(dict["color"]))
                    {
                        dict["color"] = GetColor(dict["display-name"]);
                    }


                    MessageReceived?.Invoke(this, new MessageRecievedArgs(dict));
                }
            }
        }
        public string GetColor(string name)
        {
            char[] nameChar = name.ToCharArray();
            return defaultColors[(nameChar[0] + nameChar[name.Length-1]) % defaultColors.Count];
        }
        public void Update()
        {
            try
            {
                this.internalUpdate();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        public void StartLoop()
        {
            var retry = false;
            var retryCount = 0;
            do
            {
                try
                {
                    while (true)
                    {
                        this.internalUpdate();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    retry = ++retryCount <= _maxRetries;
                }
            } while (retry);
        }
    }
}
