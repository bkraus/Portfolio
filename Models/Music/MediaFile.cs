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
        public List<SongInfo> Songs { get; set; }
        public List<GroupList> Artists { get; set; }
        public List<GroupList> Playlists { get; set; }
        public int SongsInLists { get; set; }
        public int RankedSongs { get; set; }
        public int FilteredSongs { get; set; }
        public int TotalSongs { get; set; }
        public int SongsAbove { get; set; }
        public string ArtistFilter { get; set; }
        public MediaFile()
        {
            Songs = new List<SongInfo>();
            Artists = new List<GroupList>();
            Playlists = new List<GroupList>();
            ArtistFilter = "";
            LoadData();
        }
        public MediaFile(string Filter)
        {
            Songs = new List<SongInfo>();
            Artists = new List<GroupList>();
            Playlists = new List<GroupList>();
            ArtistFilter = Filter == null ? "" : Filter;

            LoadData();
        }
        private void LoadData()
        {
            XDocument xdoc = XDocument.Load(Preferences.Retrieve("MusicPath") + "wmpMetadata.xml");
            List<XElement> Mediaitems = xdoc.Root.Elements("MediaItem").ToList();
            SongsInLists = 0;
            TotalSongs = Mediaitems.Count();
            RankedSongs = 0;
            FilteredSongs = 0;
            SongsAbove = 0;
            int rank;
            int id = 0;
            Songs.Clear();
            List<SongInfo> lSongs = new List<SongInfo>();
            foreach (XElement Media in Mediaitems)
            {
                SongInfo song = new SongInfo();
                song.Path = Media.Attribute("SourceUrl").Value;
                song.Title = Media.Element("Title").Value;
                song.Artist = !string.IsNullOrEmpty(Media.Element("WM_AlbumArtist").Value) ? Media.Element("WM_AlbumArtist").Value : Media.Element("Author").Value;
                song.Custom1 = Media.Element("UserCustom1").Value;
                song.ID = id++;
                song.TagFile = TagLib.File.Create(song.Path);
                int.TryParse((string)Media.Element("UserCustom2").Value, out rank);
                song.Custom2 = rank;
                if (rank > 40)
                    SongsAbove++;

                if (song.Custom2 > 0)
                    RankedSongs++;
                if (song.Custom1 != "")
                {
                    AddToPlaylistCnt(song.Custom1);
                    SongsInLists++;
                }
                AddtoArtist(song.Artist);

                if (ArtistFilter == "" || song.Title.ToLower().Contains(ArtistFilter.ToLower()) 
                    || song.Artist.ToLower().Contains(ArtistFilter.ToLower())
                    || song.Custom2.ToString().Contains(ArtistFilter.ToLower()))
                {
                    lSongs.Add(song);
                    FilteredSongs++;
                }
            }
            Songs = lSongs.OrderByDescending(x => x.Custom2).ToList();

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
        public void SaveSong(SongInfo song)
        {
            XDocument xdoc = XDocument.Load(Preferences.Retrieve("MusicPath") + "wmpMetadata.xml");
            List<XElement> Mediaitems = xdoc.Root.Elements("MediaItem").ToList();
            XElement item = Mediaitems.Where(x => x.Attribute("SourceUrl").Value.ToString() == song.Path).FirstOrDefault();
            if (song.Custom1 !=null)
                item.Element("UserCustom1").Value = song.Custom1;
            item.Element("UserCustom2").Value = song.Custom2.ToString();
            xdoc.Save(Preferences.Retrieve("MusicPath") + "wmpMetadata.xml");
        }
    }

}