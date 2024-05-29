using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Курсовая1
{
	public class Artist
	{
		public string ArtistName { get;  set; }
		public string ArtistPath { get;  set; }
		public string PhotoPath { get;  set; }
		public List<Album> Albums { get;  set; }

		public Artist(string artistPath)
		{
			ArtistPath = artistPath;
			ArtistName = Path.GetFileName(artistPath);
			Albums = new List<Album>();
		}

		public void FindAlbums()
		{
			if (!Directory.Exists(ArtistPath))
			{
				Console.WriteLine("Ошибка: указанный путь не является директорией исполнителя.");
				return;
			}

			string[] albumDirs = Directory.GetDirectories(ArtistPath);
			foreach (string albumDir in albumDirs)
			{
				Album album = new Album(albumDir);
				album.FindTracks();
				album.FindAlbumCover();
				Albums.Add(album);
			}
		}

		public void FindArtistPhoto()
		{
			string[] imageFiles = Directory.GetFiles(ArtistPath, "*.jpg");
			if (imageFiles.Length == 0)
			{
				imageFiles = Directory.GetFiles(ArtistPath, "*.png");
			}
			if (imageFiles.Length == 0)
			{
				imageFiles = Directory.GetFiles(ArtistPath, "*.jpeg");
			}

			if (imageFiles.Length > 0)
			{
				PhotoPath = imageFiles[0];
			}
			else
			{
				Console.WriteLine("Фото исполнителя не найдено.");
			}
		}
	}
}
