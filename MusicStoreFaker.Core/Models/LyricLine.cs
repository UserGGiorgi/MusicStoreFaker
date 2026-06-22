using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStoreFaker.Core.Models
{
    public class LyricLine
    {
        public int TimeMs { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
