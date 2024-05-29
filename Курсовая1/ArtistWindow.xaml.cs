using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Курсовая1
{
	/// <summary>
	/// Логика взаимодействия для ArtistWindow.xaml
	/// </summary>
	public partial class ArtistWindow : Window
	{
		private Artist artist;

		public ArtistWindow(Artist selectedArtist)
		{
			InitializeComponent();
			artist = selectedArtist;
			artist.Albums.Clear(); // Очищаем список альбомов, чтобы избежать дублирования
			LoadArtistDetails();
			LoadAlbums();
		}

		private void LoadArtistDetails()
		{
			// Устанавливаем фотографию исполнителя
			if (File.Exists(artist.PhotoPath))
			{
				ArtistPhoto.Source = new BitmapImage(new Uri(artist.PhotoPath));
			}

			// Устанавливаем название исполнителя
			ArtistNameTextBlock.Text = artist.ArtistName;
		}

		private void LoadAlbums()
		{
			foreach (var albumPath in Directory.GetDirectories(artist.ArtistPath))
			{
				var album = new Album(albumPath);
				album.FindTracks();
				album.FindAlbumCover();
				artist.Albums.Add(album);
			}
			AlbumsListBox.ItemsSource = artist.Albums;
		}

		private void AlbumsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (AlbumsListBox.SelectedItem is Album selectedAlbum)
			{
				this.Hide();
				TrackSearchService trackSearchService = new TrackSearchService("path_to_your_root_folder");
				List<Track> tracks = trackSearchService.SearchTracksInAlbum(selectedAlbum.AlbumPath);

				selectedAlbum.FindAlbumCover();  // Убедитесь, что путь к обложке альбома заполнен
				AlbumWindow albumWindow = new AlbumWindow(selectedAlbum.AlbumName, selectedAlbum.CoverPath, tracks, this);
				albumWindow.Show();
				AlbumsListBox.SelectedItem = null;
			}
		}

		private void HomeButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}	
