using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DomainModels
{
    public class Track : BaseEntity
    {
        [Required]
        public int TrackNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        public int? DurationSeconds { get; set; }

        public Guid RecordId { get; set; }

        [ValidateNever]
        public virtual Record Record { get; set; }
        public string DurationFormatted
        {
            get
            {
                if (!DurationSeconds.HasValue)
                {
                    return "--:--";
                }
                TimeSpan time = TimeSpan.FromSeconds(DurationSeconds.Value);
                return $"{(int)time.TotalMinutes:00}:{time.Seconds:00}";
            }
        }
    }
}