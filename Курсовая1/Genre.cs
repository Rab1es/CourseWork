using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсовая1
{
	public class Genre
	{
		public string GenreName { get; private set; }
		public string GenrePath { get; private set; }
		public List<Artist> Artists { get; private set; }

		public Genre(string genrePath)
		{
			GenrePath = genrePath;
			GenreName = Path.GetFileName(genrePath);
			Artists = new List<Artist>();
		}

		public void FindArtists()
		{
			if (!Directory.Exists(GenrePath))
			{
				Console.WriteLine("Ошибка: указанный путь не является директорией с жанром.");
				return;
			}

			string[] artistDirs = Directory.GetDirectories(GenrePath);
			foreach (string artistDir in artistDirs)
			{
				Artist artist = new Artist(artistDir);
				artist.FindAlbums();
				artist.FindArtistPhoto();
				Artists.Add(artist);
			}
		}

		public static List<Genre> GetGenres(string genresFolderPath)
		{
			List<Genre> genres = new List<Genre>();

			string path = Path.GetFullPath(genresFolderPath);
			if (!Directory.Exists(path))
			{
				Console.WriteLine("Ошибка: указанный путь не существует.");
				return genres;
			}

			string[] genreDirs = Directory.GetDirectories(genresFolderPath);
			foreach (string genreDir in genreDirs)
			{
				Genre genre = new Genre(genreDir);
				genre.FindArtists();
				genres.Add(genre);
			}

			return genres;
		}

	}
}
