﻿using System;
using System.Collections.Generic;

namespace APIMusicaAuth_SerafinParedesAlejandro.Models
{
    public partial class Genre
    {
        public Genre()
        {
            Tracks = new HashSet<Track>();
        }

        public int GenreId { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }
    }
}
