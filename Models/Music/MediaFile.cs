using Portfolio.Models.Preference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Portfolio.Models.Music
{

    public class MediaFile
    {
        public string Path { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public String Custom1 { get; set; }
        public int Custom2 { get; set; }
        public int sort { get; set; }
        List<MediaFile> Songs { get; set; }
        List<GroupList> Artists { get; set; }
        List<GroupList> Playlists { get; set; }
        public int SongsInLists { get; set; }
        public int RankedSongs { get; set; }
        public int FilteredSongs { get; set; }
        public int TotalSongs { get; set; }
        public int SongsAbove { get; set; }

        public MediaFile()
        {
            Songs = new List<MediaFile>();
            LoadData();
        }
        private void LoadData(bool Update = false)
        {
            XDocument xdoc = XDocument.Load(Preferences.Retrieve("MusicPath"));
            List<XElement> Mediaitems = xdoc.Root.Elements("MediaItem").ToList();
            SongsInLists = 0;
            TotalSongs = Mediaitems.Count();
            RankedSongs = 0;
            FilteredSongs = 0;
            SongsAbove = 0;
            int rank;
            Songs.Clear();
            foreach (XElement Media in Mediaitems)
            {
                MediaFile song = new MediaFile();
                song.Path = Media.Attribute("SourceUrl").Value;
                song.Title = Media.Element("Title").Value;
                song.Artist = !string.IsNullOrEmpty(Media.Element("WM_AlbumArtist").Value) ? Media.Element("WM_AlbumArtist").Value : Media.Element("Author").Value;
                song.Custom1 = Media.Element("UserCustom1").Value;
                int.TryParse((string)Media.Element("UserCustom2").Value, out rank);
                song.Custom2 = rank;
                if (rank > 40)
                    SongsAbove++;
                Songs.Add(song);
                FilteredSongs++;

                if (song.Custom2 > 0)
                    RankedSongs++;
                if (song.Custom1 != "")
                {
                    AddToPlaylistCnt(song.Custom1);
                    SongsInLists++;
                }
                AddtoArtist(song.Artist);

            }


        }
        private void AddtoArtist(string name)
        {
            GroupList p = (from s in Artists where s.Name.ToUpper() == name.ToUpper() select s).FirstOrDefault();
            if (p == null)
            {
                GroupList t = new GroupList();
                t.Name = name;
                t.Count = 1;
                Artists.Add(t);
            }
            else
            {
                p.Count++;
            }
        }
        private void AddToPlaylistCnt(string list)
        {
            string[] slist = list.Split(',');
            foreach (string l in slist)
            {
                GroupList p = (from s in Playlists where s.Name == l.ToUpper() select s).FirstOrDefault();
                if (p == null)
                {
                    GroupList t = new GroupList();
                    t.Name = l.ToUpper();
                    t.Count = 1;
                    Playlists.Add(t);
                }
                else
                {
                    p.Count++;
                }

            }
        }
    }

}