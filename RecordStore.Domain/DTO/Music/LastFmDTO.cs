using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RecordStore.Domain.DTO.Music
{
    public class LastFmApiResponse
    {
        [JsonPropertyName("artists")]
        public LastFmTopArtists? TopArtists { get; set; }
    }

    public class LastFmTopArtists
    {
        [JsonPropertyName("artist")]
        public List<LastFmArtist>? Artists { get; set; }
    }

    public class LastFmArtist
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("mbid")]
        public string? MusicBrainzId { get; set; }
    }
}