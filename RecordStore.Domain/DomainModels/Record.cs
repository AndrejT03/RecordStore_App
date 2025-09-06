using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DomainModels
{
    public enum RecordFormat
    {
        Vinyl, CD
    }
    public enum Genre
    {
        Rock, Pop, Jazz, Classical, HipHop, Electronic,
        Blues, Country, Reggae, Metal, Funk, Soul, Other
    }

    public class Record : BaseEntity
    {
        [Required(ErrorMessage = "The Title field is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The title must be between 2 and 100 characters.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Please select an artist.")]
        [Display(Name = "Artist")]
        public Guid? ArtistId { get; set; }
        public Artist? Artist { get; set; }

        [Required(ErrorMessage = "The release year is required.")]
        [Range(1900, 2026, ErrorMessage = "Please enter a valid year.")]
        [Display(Name = "Release Year")]
        public int? ReleaseYear { get; set; }

        [Required(ErrorMessage = "Please select a format.")]
        public RecordFormat? Format { get; set; }

        [Required(ErrorMessage = "Please select a genre.")]
        public Genre? Genre { get; set; }

        [Required(ErrorMessage = "Please select a record label.")]
        [Display(Name = "Record Label")]
        public Guid? RecordLabelId { get; set; }
        public RecordLabel? RecordLabel { get; set; }

        [Display(Name = "Please select if the record is a reissue.")]
        public bool IsReissue { get; set; }

        [Required(ErrorMessage = "The cover URL is required.")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        [Display(Name = "Cover URL")]
        public string? CoverURL { get; set; }

        [Required(ErrorMessage = "The price is required.")]
        [Range(0.01, 10000.00, ErrorMessage = "The price must be greater than 0.")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "The stock quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "The quantity cannot be negative.")]
        [Display(Name = "Stock Quantity")]
        public int? StockQuantity { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Track>? Tracks { get; set; } 
    }
}