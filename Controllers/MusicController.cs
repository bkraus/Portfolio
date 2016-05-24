using Portfolio.Models.Music;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TagLib;

namespace Portfolio.Controllers
{
    public class MusicController : Controller
    {
        //
        // GET: /Music/

        public ActionResult Index()
        {
            MediaFile Media = new MediaFile();
            return View(Media);
        }

        public ActionResult Update(int id)
        {
            MediaEdit mediaEdit = new MediaEdit();
            MediaFile Media = new MediaFile();
            SongInfo song = Media.Songs.Where(z=>z.ID ==id).FirstOrDefault();
            mediaEdit.ID  = id;
            mediaEdit.Path = song.Path;
            TagLib.File TagFile = TagLib.File.Create(song.Path);
            mediaEdit.Title = TagFile.Tag.Title;
            mediaEdit.FirstAlbumArtist = TagFile.Tag.FirstAlbumArtist;
            mediaEdit.Album = TagFile.Tag.Album;
            mediaEdit.Disc =(int) TagFile.Tag.Disc;
            mediaEdit.FirstPerformer = TagFile.Tag.FirstPerformer;
            mediaEdit.FirstComposer = TagFile.Tag.FirstComposer;
            mediaEdit.Custom1 = song.Custom1;
            mediaEdit.Custom2 = song.Custom2;
            mediaEdit.ArtistFilter = Request.QueryString["Filter"]==null ? "" : Request.QueryString["Filter"].ToString();
            return View(mediaEdit);
        }
        public ActionResult Save(MediaEdit mediaEdit)
        {
            using (TagLib.File tagFile = TagLib.File.Create(mediaEdit.Path))
            {
                if ( HasChanged(tagFile,mediaEdit))
                {
                    tagFile.Tag.AlbumArtists = null;
                    tagFile.Tag.AlbumArtists = new string[] { mediaEdit.FirstAlbumArtist };
                    tagFile.Tag.Composers = null;
                    tagFile.Tag.Composers = new string[] { mediaEdit.FirstComposer };
                    tagFile.Tag.Performers = null;
                    tagFile.Tag.Performers = new string[] { mediaEdit.FirstPerformer };
                    tagFile.Tag.Title = mediaEdit.Title;
                    tagFile.Tag.Album = mediaEdit.Album;
                    tagFile.Tag.Disc = (uint)mediaEdit.Disc;
                    tagFile.Save();
                }
                MediaFile Media = new MediaFile();
                SongInfo song = Media.Songs.Where(x => x.Path == mediaEdit.Path).FirstOrDefault();
                song.Custom1 = mediaEdit.Custom1;
                song.Custom2 = mediaEdit.Custom2;
                Media.SaveSong(song);
            }

            return Filter(mediaEdit.ArtistFilter);
        }
        public ActionResult UpdateIndex(int ID)
        {
            MediaFile Media = new MediaFile();
            return View("Index",Media);
        }
        public ActionResult Filter(string ArtistFilter)
        {
            ViewBag.Filter = ArtistFilter;
            MediaFile Media = new MediaFile(ArtistFilter);
            return View("Index", Media);
        }

        private bool HasChanged(TagLib.File tagFile, MediaEdit mediaEdit)
        {
            if (tagFile.Tag.AlbumArtists == null || tagFile.Tag.AlbumArtists.Length == 0)
            {
                if (!string.IsNullOrEmpty(mediaEdit.FirstAlbumArtist))
                    return true;
            }
            else if (tagFile.Tag.AlbumArtists[0] != mediaEdit.FirstAlbumArtist)
                return true;
            if (tagFile.Tag.Composers == null || tagFile.Tag.Composers.Length == 0)
            {
                if (!string.IsNullOrEmpty(mediaEdit.FirstComposer))
                    return true;
            }
            else if (tagFile.Tag.Composers[0] != mediaEdit.FirstComposer)
                return true;
            if (tagFile.Tag.Performers == null || tagFile.Tag.Performers.Length == 0)
            {
                if (!string.IsNullOrEmpty(mediaEdit.FirstPerformer))
                    return true;
            }
            else if (tagFile.Tag.Performers[0] != mediaEdit.FirstPerformer)
                return true;

            
            if (tagFile.Tag.Title != mediaEdit.Title ||
                    tagFile.Tag.Album != mediaEdit.Album ||
                    tagFile.Tag.Disc != mediaEdit.Disc)
                return true;
            return false;
            }
     }
}
