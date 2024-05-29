using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсовая1
{
	public class Track
	{
		public string TrackName { get; set; }
		public string FullPath { get; set; }

		public Track(string name, string path)
		{
			TrackName = name;
			FullPath = path;
		}
	}


}
