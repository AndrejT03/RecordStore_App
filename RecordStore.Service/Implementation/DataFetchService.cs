using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RecordStore.Domain.DomainModels;
using RecordStore.Domain.DTO;
using RecordStore.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;
using RecordStore.Domain.DTO.Music;

namespace RecordStore.Service.Implementation
{
    public class DataFetchService : IDataFetchService
    {
        private readonly HttpClient _httpClient;
        private readonly ICountryService _countryService;
        private readonly IArtistService _artistService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DataFetchService> _logger;

        public DataFetchService(HttpClient httpClient, ICountryService countryService, IArtistService artistService, IConfiguration configuration, ILogger<DataFetchService> logger)
        {
            _httpClient = httpClient;
            _countryService = countryService;
            _artistService = artistService;
            _configuration = configuration;
            _logger = logger;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("RecordStoreApp/1.0 (andrejtrajkovski03@gmail.com)");
        }
        

        public async Task<List<Country>> GetAllCountriesAsync()
        {
            return _countryService.GetAll();
        }

        public async Task<List<Country>> ImportTenNewCountriesAsync()
        {
            var apiUrl = "https://restcountries.com/v3.1/all?fields=name,cca2,cca3,capital,region,flags";
            var countriesFromApi = await _httpClient.GetFromJsonAsync<List<CountryApiDTO>>(apiUrl);

            if (countriesFromApi == null || !countriesFromApi.Any())
                return new List<Country>();

            var existingCodes = (_countryService.GetAll())
                .Select(c => c.Code1)
                .ToHashSet();

            var newCountriesDto = countriesFromApi
                .Where(dto => !string.IsNullOrEmpty(dto.cca2) && !existingCodes.Contains(dto.cca2))
                .Take(10)
                .ToList();

            if (!newCountriesDto.Any())
                return new List<Country>();

            var countriesToInsert = newCountriesDto.Select(x => new Country
            {
                Id = Guid.NewGuid(),
                Name = x.name?.common ?? "N/A",
                Code1 = x.cca2,
                Code2 = x.cca3,
                Capital = x.capital?.FirstOrDefault() ?? "N/A",
                Region = x.region,
                Flag = x.flags.png ?? ""
            }).ToList();

            _countryService.InsertMany(countriesToInsert);
            return countriesToInsert;
        }

        public async Task<List<Artist>> ImportArtistsAsync()
        {
            _logger.LogInformation("Attempting to import exactly 10 new popular artists with full details.");

            var countriesInDb = (_countryService.GetAll())
                .ToDictionary(c => c.Name, c => c, StringComparer.OrdinalIgnoreCase);

            var existingArtistIds = _artistService.GetAll().Select(a => a.Id).ToHashSet();

            var lastFmApiKey = _configuration["ApiKeys:LastFm"];
            if (string.IsNullOrEmpty(lastFmApiKey))
            {
                _logger.LogError("API key for Last.fm is not configured.");
                return new List<Artist>();
            }

            var newArtistsToInsert = new List<Artist>();
            int currentPage = 1; 

            while (newArtistsToInsert.Count < 10)
            {
                _logger.LogInformation("Fetching page {PageNumber} from Last.fm to find more artists.", currentPage);

                var lastFmApiUrl = $"http://ws.audioscrobbler.com/2.0/?method=chart.gettopartists&api_key={lastFmApiKey}&page={currentPage}&format=json";
                var lastFmResponse = await _httpClient.GetFromJsonAsync<LastFmApiResponse>(lastFmApiUrl);

                if (lastFmResponse?.TopArtists?.Artists == null || !lastFmResponse.TopArtists.Artists.Any())
                {
                    _logger.LogWarning("No more artists found on Last.fm. Stopping search.");
                    break; 
                }

                foreach (var lfmArtist in lastFmResponse.TopArtists.Artists)
                {
                    if (newArtistsToInsert.Count >= 10) break;

                    if (string.IsNullOrEmpty(lfmArtist.Name) || !Guid.TryParse(lfmArtist.MusicBrainzId, out var artistId) || existingArtistIds.Contains(artistId))
                    {
                        continue;
                    }

                    var (pictureUrl, bio) = await GetArtistDetailsFromWikipediaAsync(lfmArtist.Name);

                    if (string.IsNullOrWhiteSpace(pictureUrl) || string.IsNullOrWhiteSpace(bio))
                    {
                        _logger.LogInformation("Skipping '{ArtistName}' due to missing picture or bio from Wikipedia.", lfmArtist.Name);
                        continue;
                    }

                    var countryName = await GetArtistCountryFromMusicBrainzAsync(artistId);
                    var normalizedCountryName = NormalizeCountryName(countryName);

                    if (string.IsNullOrEmpty(normalizedCountryName) || !countriesInDb.TryGetValue(normalizedCountryName, out var foundCountry))
                    {
                        _logger.LogWarning("Skipping '{ArtistName}' because their country '{Country}' was not found in local DB.", lfmArtist.Name, countryName ?? "N/A");
                        continue;
                    }

                    var artist = new Artist
                    {
                        Id = artistId,
                        Name = lfmArtist.Name,
                        Picture = pictureUrl,
                        Bio = bio.Trim(), 
                        CountryId = foundCountry.Id
                    };

                    newArtistsToInsert.Add(artist);
                    existingArtistIds.Add(artist.Id);
                    _logger.LogInformation("Successfully validated artist '{ArtistName}'. Found {Count}/10.", artist.Name, newArtistsToInsert.Count);
                }

                currentPage++;
            }

            if (newArtistsToInsert.Any())
            {
                _logger.LogInformation("Importing {Count} new popular artists to the database.", newArtistsToInsert.Count);
                _artistService.InsertMany(newArtistsToInsert);
            }
            else
            {
                _logger.LogWarning("No new popular artists meeting all criteria could be found.");
            }

            return newArtistsToInsert;
        }


        private async Task<(string? PictureUrl, string? Bio)> GetArtistDetailsFromWikipediaAsync(string artistName)
        {
            try
            {
                var sanitizedArtistName = artistName.Replace(",", "");
                var artistNameForUrl = Uri.EscapeDataString(sanitizedArtistName);
                var url = $"https://en.wikipedia.org/w/api.php?action=query&prop=pageimages|extracts&format=json&exintro&explaintext&pithumbsize=500&titles={artistNameForUrl}";

                var response = await _httpClient.GetFromJsonAsync<WikipediaApiQueryResponse>(url);
                var page = response?.Query?.Pages?.Values.FirstOrDefault();

                if (page != null && page.PageId > 0 && !string.IsNullOrWhiteSpace(page.Extract))
                {
                    _logger.LogInformation("Successfully found details on Wikipedia for: {ArtistName}", artistName);

                    string? firstParagraph = page.Extract
                        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(firstParagraph))
                    {
                        return (page.Thumbnail?.Source, null); 
                    }

                    string cleanedBio = Regex.Replace(firstParagraph, @"\[\d+\]", "");
                    cleanedBio = Regex.Replace(cleanedBio, @"\s*\([^)]*\b(IPA|/|EYE-lish)\b[^)]*\)", "", RegexOptions.IgnoreCase);
                    cleanedBio = cleanedBio.Trim();

                    return (page.Thumbnail?.Source, cleanedBio);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching details from Wikipedia for: {ArtistName}", artistName);
            }

            _logger.LogWarning("Could not find details on Wikipedia for: {ArtistName}", artistName);
            return (null, null);
        }

        private async Task<string?> GetArtistCountryFromMusicBrainzAsync(Guid artistId)
        {
            try
            {
                var url = $"https://musicbrainz.org/ws/2/artist/{artistId}?inc=area-rels&fmt=json";
                var response = await _httpClient.GetFromJsonAsync<MusicBrainzArtist>(url);
                return response?.Area?.Name;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch country for artist ID {ArtistId} from MusicBrainz.", artistId);
                return null;
            }
        }

        private string? NormalizeCountryName(string? countryName)
        {
            if (string.IsNullOrEmpty(countryName)) return null;
            var countryMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "United Kingdom of Great Britain and Northern Ireland", "United Kingdom" },
                { "England", "United Kingdom" },
                { "Scotland", "United Kingdom" },
                { "Wales", "United Kingdom" },
                { "Northern Ireland", "United Kingdom" },
                { "USA", "United States" },
                { "United States of America", "United States" }
            };
            return countryMap.TryGetValue(countryName, out var mappedName) ? mappedName : countryName;
        }
    }
}