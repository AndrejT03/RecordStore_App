using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RecordStore.Domain.DTO.Music
{
    public class TheAudioDbArtistResponse
    {
        [JsonPropertyName("artists")]
        public List<TheAudioDbArtist>? Artists { get; set; }
    }

    public class TheAudioDbArtist
    {
        [JsonPropertyName("strArtistThumb")]
        public string? ArtistThumb { get; set; }

        [JsonPropertyName("strGenre")]
        public string? Genre { get; set; }

        [JsonPropertyName("strBiographyEN")]
        public string? Biography { get; set; }
    }
}