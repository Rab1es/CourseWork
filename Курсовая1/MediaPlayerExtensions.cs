using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Курсовая1
{
	public static class MediaPlayerExtensions
	{
		public static bool IsPlaying(this MediaPlayer mediaPlayer)
		{
			// Воспроизведение трека может быть определено через состояние
			// мы проверяем если состояние не является "Paused" и "Stopped"
			var helper = new MediaStateHelper();
			helper.AttachMediaPlayer(mediaPlayer);
			return helper.IsPlaying;
		}
	}
}
