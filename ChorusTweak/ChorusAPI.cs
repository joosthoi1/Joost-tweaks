using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Net.Mime;
using UnityEngine;
using Newtonsoft.Json;
using System.IO.Compression;
using Common.Wrappers;
using System.Xml.Linq;

namespace ChorusTweak
{
    public class Songs
    {
        public List<Song> songs { get; set; }
    }
    public class Song
    {
        public int id { get; set; }
        public string name { get; set; }
        public string artist { get; set; }
        public string album { get; set; }
        public string genre { get; set; }
        public string year { get; set; }
        public string charter { get; set; }
        public int length { get; set; }
        public int effectivelength { get; set; }
        public string link { get; set; }
        public Dictionary<String, String> directLinks { get; set; }
    }
    class ChorusAPI
    {
        static readonly char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
        public static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
        public static Songs CSearch(int type=0,string search= null, string name = null, string artist = null, string album = null, string genre = null, string charter = null, string md5 = null)
        {
            name = String.IsNullOrEmpty(name) ? "" : $"name=\"{name}\"+";
            artist = String.IsNullOrEmpty(artist) ? "" : $"+artist=\"{artist}\"+";
            album = String.IsNullOrEmpty(album) ? "" : $"album=\"{album}\"+";
            genre = String.IsNullOrEmpty(genre) ? "" : $"genre=\"{genre}\"+";
            charter = String.IsNullOrEmpty(charter) ? "" : $"charter=\"{charter}\"+";
            md5 = String.IsNullOrEmpty(md5) ? "" : $"md5=\"{md5}\"+";
            string url = "";
            if (type == 0)
            {
                url = $"https://chorus.fightthe.pw/api/search?query={search}{name}{artist}{album}{genre}{charter}{md5}";
                url = url.Remove(url.Length - 1);

            }
            else if (type == 1)
            {
                url = $"https://chorus.fightthe.pw/api/latest";
            }
            else if (type == 2)
            {
                url = $"https://chorus.fightthe.pw/api/random";
            }
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();
            //Debug.Log(jsonResponse);
            Songs s = JsonConvert.DeserializeObject<Songs>(jsonResponse, settings);
            return s;
        }
        public static void DownloadFile(Dictionary<string, string> links, string songName, ChorusTweak x)
        {
            string validFolderName = new string(songName.Where(ch => !invalidFileNameChars.Contains(ch)).ToArray());
            if (!Directory.Exists($"songs/{validFolderName}"))
            {
                Directory.CreateDirectory($"songs/{validFolderName}");
            }
            foreach (string url in links.Values)
            {
                ContentDisposition header;
                byte[] fileContent;
                using (WebClient wc = new WebClient())
                {
                    fileContent = wc.DownloadData(
                        new System.Uri(url)
                    );
                    header = new ContentDisposition(wc.ResponseHeaders["Content-Disposition"]);
                }
                string validFileName = new string(header.FileName.Where(ch => !invalidFileNameChars.Contains(ch)).ToArray());
                using (var fs = new FileStream($"songs/{validFolderName}/{validFileName}", FileMode.Create, FileAccess.Write))
                {
                    fs.Write(fileContent, 0, fileContent.Length);
                }
                string extention = header.FileName.Split('.')[header.FileName.Split('.').Length-1];
                if (extention == "zip" || extention == "rar")
                {
                    x.showArchiveError = true;
                    x.archiveLocation = $"songs/{validFolderName}/{validFileName}";
                    if (extention == "zip")
                    {
                    }
                    else
                    {
                    }
                }
            }
            x.downloadedSong = true;
        }
        /*
        static HttpClient client = new HttpClient();
        public static async Task<HttpResponseMessage> search(string name = null, string artist = null, string album = null, string genre = null, string charter = null, string md5 = null)
        {
            name = String.IsNullOrEmpty(name) ? "" : $"name=\"{name}\"+";
            artist = String.IsNullOrEmpty(artist) ? "" : $"+artist=\"{artist}\"+";
            album = String.IsNullOrEmpty(album) ? "" : $"album=\"{album}\"+";
            genre = String.IsNullOrEmpty(genre) ? "" : $"genre=\"{genre}\"+";
            charter = String.IsNullOrEmpty(charter) ? "" : $"charter=\"{charter}\"+";
            md5 = String.IsNullOrEmpty(md5) ? "" : $"md5=\"{md5}\"+";
            string url = $"https://chorus.fightthe.pw/api/search?query={name}{artist}{album}{genre}{charter}{md5}";
            url = url.Remove(url.Length - 1);
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), url))
            {
                HttpResponseMessage response = await client.SendAsync(request);

                return response;
            }
        }
        public static void DownloadSong(string url, string path)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFileAsync(
                     new System.Uri(url),
                     path
                 );
            }
        }
        */
    }
}
