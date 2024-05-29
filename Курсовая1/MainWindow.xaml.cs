using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Курсовая1
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private string genresFolderPath = @".\music";
		private TrackSearchService trackSearchService;
		public ObservableCollection<Genre> Genres { get; set; }
		private MediaPlayer mediaPlayer;
		public event PropertyChangedEventHandler PropertyChanged;
		private DispatcherTimer timer;
		private bool isPlaying;
		private bool isDragging;
		private string currentTrackPath;
		private string currentTrackName;
		public string CurrentTrackName
		{
			get { return currentTrackName; }
			set
			{
				currentTrackName = value;
				OnPropertyChanged();
			}
		}
		public MainWindow()
		{
			InitializeComponent();
			DataContext = this; // Устанавливаем текущее окно как контекст данных
			LoadGenres(); // Загружаем список жанров
			trackSearchService = new TrackSearchService(genresFolderPath);
			mediaPlayer = new MediaPlayer();
			mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
			mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(1);																			
			timer.Tick += Timer_Tick;
		}
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
		private void MediaPlayer_MediaOpened(object sender, EventArgs e)
		{
			TrackSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
			TotalDurationTextBlock.Text = mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
		}
		private void MediaPlayer_MediaEnded(object sender, EventArgs e)
		{
			PlayPauseButton.Content = "Play";
			isPlaying = false;
			timer.Stop();
			TrackSlider.Value = 0;
			CurrentPositionTextBlock.Text = "00:00";
		}
		private void Timer_Tick(object sender, EventArgs e)
		{
			if (!isDragging)
			{
				TrackSlider.Value = mediaPlayer.Position.TotalSeconds;
				CurrentPositionTextBlock.Text = mediaPlayer.Position.ToString(@"mm\:ss");
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
		private void LoadGenres()
		{
			/*Genres = new ObservableCollection<Genre>(Genre.GetGenres(genresFolderPath));
			foreach (var genre in Genres)
			{
				foreach (var artist in genre.Artists)
				{
					// Создаем путь к файлу фотографии артиста
					string artistFolderPath = System.IO.Path.Combine(artist.ArtistPath, artist.ArtistName + "_logo.jpg");

					// Проверяем, существует ли файл фотографии артиста
					if (File.Exists(artistFolderPath))
					{
						// Если файл существует, устанавливаем его путь как PhotoPath
						artist.PhotoPath = artistFolderPath;
					}
					else
					{
						// Если файл не существует, можно установить путь к фотографии "по умолчанию" или просто оставить PhotoPath пустым
						artist.PhotoPath = string.Empty;
					}
				}
			}
			MainListBox.ItemsSource = Genres; // Устанавливаем список жанров в ListBox
			MainListBox.SelectionChanged += GenresListBox_SelectionChanged; // Обработчик события выбора элемента в ListBox*/
			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string genresFolderPath = System.IO.Path.Combine(baseDirectory, "music");

			Console.WriteLine($"Base directory: {baseDirectory}");
			Console.WriteLine($"Genres folder path: {genresFolderPath}");

			Genres = new ObservableCollection<Genre>(Genre.GetGenres(genresFolderPath));

			foreach (var genre in Genres)
			{
				foreach (var artist in genre.Artists)
				{
					// Создаем путь к файлу фотографии артиста, начиная с папки жанра и папки исполнителя
					string artistFolderPath = System.IO.Path.Combine(genresFolderPath, genre.GenreName, artist.ArtistName);
					string artistPhotoPath = System.IO.Path.Combine(artistFolderPath, artist.ArtistName + "_logo.jpg");

					Console.WriteLine($"Checking path: {artistPhotoPath}");

					// Проверяем, существует ли файл фотографии артиста
					if (File.Exists(artistPhotoPath))
					{
						Console.WriteLine($"File found: {artistPhotoPath}");
						artist.PhotoPath = artistPhotoPath;
					}
					else
					{
						Console.WriteLine($"File not found: {artistPhotoPath}");
						artist.PhotoPath = string.Empty;
					}
				}
			}

			MainListBox.ItemsSource = Genres; // Устанавливаем список жанров в ListBox
			MainListBox.SelectionChanged += GenresListBox_SelectionChanged; // Обработчик события выбора элемента в ListBox
		}
		private void GenresListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Получаем выбранный жанр
			Genre selectedGenre = (sender as ListBox).SelectedItem as Genre;
		}
		private void SearchButton_Click(object sender, RoutedEventArgs e)
		{
			string searchText = SearchTextBox.Text.Trim(); // Получаем текст из текстового поля и удаляем лишние пробелы

			// Получаем выбранный жанр из ComboBox
			Genre selectedGenre = GenreComboBox.SelectedItem as Genre;

			if (!string.IsNullOrEmpty(searchText) && selectedGenre != null)
			{
				// Выполняем поиск треков на основе введенных данных и выбранного жанра
				List<Track> searchResults = trackSearchService.SearchTracks(searchText, selectedGenre);
				// Отображаем результаты поиска в ListBox
				UpdateSearchResults(searchResults);
			}
			else if (!string.IsNullOrEmpty(searchText))
			{
				// Выполняем поиск треков только по введенному тексту
				List<Track> searchResults = trackSearchService.SearchTracks(searchText);
				// Отображаем результаты поиска в ListBox
				UpdateSearchResults(searchResults);
			}
			else if (selectedGenre != null)
			{
				// Если введенный текст пустой, а жанр выбран, отображаем артистов выбранного жанра
				UpdateArtistResults(selectedGenre.Artists);
			}
			else
			{
				// Если ни текст не введен, ни жанр не выбран, очищаем результаты и показываем MainListBox
				ListBox.ItemsSource = null;
				MainListBox.Visibility = Visibility.Visible;
				MessageBox.Show("Введите текст для поиска или выберите жанр.");
			}
		}
		private void UpdateArtistResults(List<Artist> artists)
		{
			// Очищаем предыдущие результаты поиска
			ListBox.ItemsSource = null;

			// Если найдены артисты, отображаем их в ListBox
			if (artists.Count > 0)
			{
				ListBox.Visibility = Visibility.Visible;
				ListBox.ItemsSource = artists;
				MainListBox.Visibility = Visibility.Collapsed;
			}
			else
			{
				// Если артисты не найдены, выводим сообщение об этом и показываем MainListBox
				MessageBox.Show("Артисты не найдены.");
				ListBox.Visibility = Visibility.Collapsed;
				MainListBox.Visibility = Visibility.Visible;
			}
		}
		private void ArtistImage_Click(object sender, MouseButtonEventArgs e)
		{
			Image image = sender as Image;
			if (image != null)
			{
				Artist artist = image.DataContext as Artist;
				if (artist != null)
				{
					ArtistWindow artistWindow = new ArtistWindow(artist);
					artistWindow.Show();
				}
			}
		}
		private void UpdateSearchResults(List<Track> searchResults)
		{
			// Очищаем предыдущие результаты поиска
			ListBox.ItemsSource = null;

			// Если найдены треки, отображаем их в ListBox
			if (searchResults.Count > 0)
			{
				ListBox.Visibility = Visibility.Visible;
				ListBox.ItemsSource = searchResults;
				MainListBox.Visibility = Visibility.Collapsed;
			}
			else
			{
				// Если треки не найдены, выводим сообщение об этом и показываем MainListBox
				MessageBox.Show("Треки не найдены.");
				ListBox.Visibility = Visibility.Collapsed;
				MainListBox.Visibility = Visibility.Visible;
			}
		}
		private void ArtistButton_Click(object sender, RoutedEventArgs e)
		{
			Button artistButton = sender as Button;
			Artist selectedArtist = artistButton.DataContext as Artist;

			if (selectedArtist != null)
			{
				// Открываем новое окно с подробной информацией об артисте
				ArtistWindow artistWindow = new ArtistWindow(selectedArtist);
				artistWindow.Show();
			}
		}
		private void PlayButton_Click(object sender, RoutedEventArgs e)
		{
			Button playButton = sender as Button;
			string trackPath = playButton.Tag as string;

			if (!string.IsNullOrEmpty(trackPath))
			{
				try
				{
					// Преобразование относительного пути в абсолютный
					string absoluteTrackPath = System.IO.Path.GetFullPath(trackPath);

					mediaPlayer.Open(new Uri(absoluteTrackPath));
					mediaPlayer.Play();

					currentTrackPath = absoluteTrackPath;
					CurrentTrackName = System.IO.Path.GetFileNameWithoutExtension(trackPath);
					PlayPauseButton.Content = "Pause";
					BottomPanel.Visibility = Visibility.Visible;
					isPlaying = true;

					mediaPlayer.MediaEnded += (s, args) =>
					{
						PlayPauseButton.Content = "Play";
						isPlaying = false;
					};

					timer.Start();
				}
				catch (Exception ex)
				{
					MessageBox.Show($"An error occurred while trying to play the track: {ex.Message}");
				}
			}
			else
			{
				MessageBox.Show("Track path is empty.");
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
			if (!isDragging && isPlaying)
			{
				mediaPlayer.Position = TimeSpan.FromSeconds(TrackSlider.Value);
			}
		}
		private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (mediaPlayer != null)
			{
				mediaPlayer.Volume = VolumeSlider.Value;
			}
		}
		private void HomeButton_Click(object sender, RoutedEventArgs e)
		{
			ListBox.Visibility = Visibility.Collapsed;
			MainListBox.Visibility = Visibility.Visible;
		}
	}

}

