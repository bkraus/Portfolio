using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portfolio.Models.Music
{
    public class MediaEdit
    {
        public int ID { get; set; }
        public string Path { get; set; }
        public string FirstAlbumArtist { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public int Disc { get; set; }
        public string FirstPerformer { get; set; }
        public string FirstComposer { get; set; }
        public string Custom1 { get; set; }
        public int Custom2 { get; set; }
        public string ArtistFilter { get; set; }

    }
}