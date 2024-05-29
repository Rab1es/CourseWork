using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Курсовая1
{
	public class TrackSearchService
	{
		private string rootFolderPath; // Корневая папка с жанрами

		public TrackSearchService(string rootFolderPath)
		{
			this.rootFolderPath = rootFolderPath;
		}

		public List<Track> SearchTracks(string searchText, Genre selectedGenre = null)
		{
			var searchResults = new HashSet<string>();

			// Если жанр выбран, ищем треки только в папках этого жанра
			string[] searchFolders;
			if (selectedGenre != null)
			{
				searchFolders = Directory.GetDirectories(selectedGenre.GenrePath, "*", SearchOption.AllDirectories);
			}
			else
			{
				searchFolders = Directory.GetDirectories(rootFolderPath, "*", SearchOption.AllDirectories);
			}

			foreach (string folder in searchFolders)
			{
				string[] trackFiles = Directory.GetFiles(folder, "*.mp3", SearchOption.AllDirectories);

				foreach (string trackFile in trackFiles)
				{
					string trackName = Path.GetFileNameWithoutExtension(trackFile);

					if (string.IsNullOrEmpty(searchText) || trackName.ToLower().Contains(searchText.ToLower()))
					{
						searchResults.Add(trackFile);
					}
				}
			}

			return searchResults.Select(path => new Track(
				Path.GetFileNameWithoutExtension(path),
				path
			)).ToList();
		}
		public List<Track> SearchTracksInAlbum(string albumPath)
		{
			var searchResults = new List<Track>();

			if (Directory.Exists(albumPath))
			{
				string[] trackFiles = Directory.GetFiles(albumPath, "*.mp3", SearchOption.AllDirectories);
				foreach (string trackFile in trackFiles)
				{
					string trackName = Path.GetFileNameWithoutExtension(trackFile);
					searchResults.Add(new Track(trackName, trackFile));
				}
			}

			return searchResults;
		}
	}
}
