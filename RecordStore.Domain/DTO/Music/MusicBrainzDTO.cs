using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DTO.Music
{
    public class MusicBrainzArtistSearchResponse
    {
        public List<MusicBrainzArtist>? Artists { get; set; }
    }

    public class MusicBrainzArtist
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Country { get; set; } 
        public Area? Area { get; set; }
        public LifeSpan? LifeSpan { get; set; }
    }

    public class Area
    {
        public string? Name { get; set; } 
    }

    public class LifeSpan
    {
        public string? Begin { get; set; }
        public string? End { get; set; }
        public bool Ended { get; set; }
    }
}