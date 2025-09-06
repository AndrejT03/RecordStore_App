using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RecordStore.Domain.DTO
{
    public class ArtistApiResponse
    {
        [JsonPropertyName("artists")]
        public List<ArtistApiDto>? Artists { get; set; }
    }
    public class ArtistApiDto
    {
        [JsonPropertyName("strArtist")]
        public string? Name { get; set; }

        [JsonPropertyName("strArtistThumb")]
        public string? Picture { get; set; }

        [JsonPropertyName("strBiographyEN")]
        public string? Bio { get; set; }

        [JsonPropertyName("strCountry")]
        public string? Country { get; set; }
    }
}