using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TagLib;

namespace Курсовая1
{
	/// <summary>
	/// Логика взаимодействия для AlbumWindow.xaml
	/// </summary>
	public partial class AlbumWindow : Window
	{
		private MediaPlayer mediaPlayer;
		private bool isDragging = false;
		private bool isPlaying = false;
		private string currentTrackPath;
		private DispatcherTimer timer;
		private ArtistWindow artistWindow; // Поле для хранения ссылки на ArtistWindow
		public AlbumWindow(string albumName, string albumCoverPath, List<Track> tracks, ArtistWindow artistWindow)
		{
			InitializeComponent();

			AlbumNameTextBlock.Text = albumName;
			if (!string.IsNullOrEmpty(albumCoverPath))
			{
				AlbumCoverImage.Source = new BitmapImage(new Uri(albumCoverPath, UriKind.RelativeOrAbsolute));
			}
			TracksListBox.ItemsSource = tracks;

			mediaPlayer = new MediaPlayer();
			mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
			mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(1);
			timer.Tick += Timer_Tick;

			// Подсчет количества треков и общей длительности альбома
			int trackCount = tracks.Count;
			double totalDuration = 0; // Длительность в секундах

			foreach (var track in tracks)
			{
				var duration = GetTrackDuration(track.FullPath);
				totalDuration += duration.TotalSeconds;
			}

			int totalMinutes = (int)(totalDuration / 60);
			AlbumInfoTextBlock.Text = $"{trackCount} трекiв • {totalMinutes} хвилин";

			BottomPanel.Visibility = Visibility.Collapsed;
			this.artistWindow = artistWindow;

			this.Closing += AlbumWindow_Closing;
		}
		//
		private TimeSpan GetTrackDuration(string trackPath)
		{
			var file = TagLib.File.Create(trackPath);
			return file.Properties.Duration;
		}
		//
		private void PlayButton_Click(object sender, RoutedEventArgs e)
		{
			Button playButton = sender as Button;
			string trackPath = playButton.Tag as string;
			BottomPanel.Visibility = Visibility.Visible;

			if (!string.IsNullOrEmpty(trackPath))
			{
				mediaPlayer.Open(new Uri(trackPath));
				mediaPlayer.Play();

				currentTrackPath = trackPath;
				TrackNameTextBlock.Text = System.IO.Path.GetFileNameWithoutExtension(trackPath);
				PlayPauseButton.Content = "Pause";
				
				isPlaying = true;

				mediaPlayer.MediaEnded += (s, args) =>
				{
					PlayPauseButton.Content = "Play";
					isPlaying = false;
				};

				timer.Start();
			}
			else
			{
				MessageBox.Show("Track path is empty.");
			}
		}

		private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
		{
			if (mediaPlayer.Source != null)
			{
				if (isPlaying)
				{
					mediaPlayer.Pause();
					PlayPauseButton.Content = "Play";
					isPlaying = false;
					timer.Stop();
				}
				else
				{
					mediaPlayer.Play();
					PlayPauseButton.Content = "Pause";
					isPlaying = true;
					timer.Start();
				}
			}
			else
			{
				MessageBox.Show("No track is loaded.");
			}
		}

		private void TrackSlider_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			isDragging = true;
		}

		private void TrackSlider_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			isDragging = false;
			mediaPlayer.Position = TimeSpan.FromSeconds(TrackSlider.Value);
		}

		private void TrackSlider_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (isDragging)
			{
				mediaPlayer.Position = TimeSpan.FromSeconds(TrackSlider.Value);
			}
		}

		private void TrackSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!isDragging)
			{
				mediaPlayer.Position = TimeSpan.FromSeconds(e.NewValue);
			}
		}

		private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			mediaPlayer.Volume = e.NewValue;
		}

		private void MediaPlayer_MediaOpened(object sender, EventArgs e)
		{
			if (mediaPlayer.NaturalDuration.HasTimeSpan)
			{
				TrackSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
				TotalDurationTextBlock.Text = mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
			}
		}

		private void MediaPlayer_MediaEnded(object sender, EventArgs e)
		{
			PlayPauseButton.Content = "Play";
			isPlaying = false;
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			if (!isDragging && mediaPlayer.Source != null)
			{
				TrackSlider.Value = mediaPlayer.Position.TotalSeconds;
				CurrentPositionTextBlock.Text = mediaPlayer.Position.ToString(@"mm\:ss");
			}
		}
		private void AlbumWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (mediaPlayer != null)
			{
				mediaPlayer.Stop();
				mediaPlayer.Close();
			}

			artistWindow.Show();
		}
		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}

