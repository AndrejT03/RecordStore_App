using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RecordStore.Domain.DTO.Music
{
    public class WikipediaApiQueryResponse
    {
        [JsonPropertyName("query")]
        public WikipediaQuery? Query { get; set; }
    }

    public class WikipediaQuery
    {
        [JsonPropertyName("pages")]
        public Dictionary<string, WikipediaPage>? Pages { get; set; }
    }

    public class WikipediaPage
    {
        [JsonPropertyName("pageid")]
        public int PageId { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("thumbnail")]
        public WikipediaThumbnail? Thumbnail { get; set; }

        [JsonPropertyName("extract")]
        public string? Extract { get; set; }
    }

    public class WikipediaThumbnail
    {
        [JsonPropertyName("source")]
        public string? Source { get; set; }
    }
}