using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPlaylist
{
    public class Song
    {
        public string Name { get; set; }
        public Artist Artist { get; set; }
        public int Year { get; set; }
        public TimeSpan Duration { get; set; }
        public string FilePath { get; set; }

        public override string ToString() => $"{Name} - {Artist} ({Year}, {Duration:mm\\mss\\s})";
    }
}
