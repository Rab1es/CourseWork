using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Курсовая1
{
	public class MediaStateHelper : IDisposable
	{
		private MediaPlayer _mediaPlayer;
		private bool _isPlaying;

		public bool IsPlaying => _isPlaying;

		public void AttachMediaPlayer(MediaPlayer mediaPlayer)
		{
			if (_mediaPlayer != null)
			{
				_mediaPlayer.MediaOpened -= OnMediaOpened;
				_mediaPlayer.MediaEnded -= OnMediaEnded;
			}

			_mediaPlayer = mediaPlayer;
			_mediaPlayer.MediaOpened += OnMediaOpened;
			_mediaPlayer.MediaEnded += OnMediaEnded;
		}

		private void OnMediaOpened(object sender, EventArgs e)
		{
			_isPlaying = true;
		}

		private void OnMediaEnded(object sender, EventArgs e)
		{
			_isPlaying = false;
		}

		public void Dispose()
		{
			if (_mediaPlayer != null)
			{
				_mediaPlayer.MediaOpened -= OnMediaOpened;
				_mediaPlayer.MediaEnded -= OnMediaEnded;
			}
		}
	}

}
