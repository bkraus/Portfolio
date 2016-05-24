using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portfolio.Models.Music
{
    public class SongInfo
    {
        public string Path { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public String Custom1 { get; set; }
        public int Custom2 { get; set; }
        public int ID { get; set; }
        public TagLib.File TagFile { get; set; }
    }
}