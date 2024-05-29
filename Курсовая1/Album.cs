using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace Курсовая1
{
	public class Album
	{
		public string AlbumName { get; private set; }
		public string AlbumPath { get; private set; }
		public string CoverPath { get; private set; }
		public List<string> Tracks { get; private set; }

		public Album(string albumPath)
		{
			AlbumPath = albumPath;
			AlbumName = Path.GetFileName(albumPath);
			Tracks = new List<string>();
		}

		public void FindTracks()
		{
			if (!Directory.Exists(AlbumPath))
			{
				Console.WriteLine("Ошибка: указанный путь не является директорией альбома.");
				return;
			}

			string[] trackFiles = Directory.GetFiles(AlbumPath, "*.mp3", SearchOption.AllDirectories);
			foreach (string trackFile in trackFiles)
			{
				Tracks.Add(trackFile);
			}
		}

		public void FindAlbumCover()
		{
			string[] imageFiles = Directory.GetFiles(AlbumPath, "*.jpg", SearchOption.AllDirectories);
			if (imageFiles.Length == 0)
			{
				imageFiles = Directory.GetFiles(AlbumPath, "*.png", SearchOption.AllDirectories);
			}
			if (imageFiles.Length == 0)
			{
				imageFiles = Directory.GetFiles(AlbumPath, "*.jpeg", SearchOption.AllDirectories);
			}

			if (imageFiles.Length > 0)
			{
				CoverPath = imageFiles[0];
			}
			else
			{
				Console.WriteLine("Обложка альбома не найдена.");
			}
		}
	}
}
