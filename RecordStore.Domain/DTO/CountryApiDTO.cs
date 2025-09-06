using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Domain.DTO
{
    public class CountryApiDTO
    {
        public NameProperty name { get; set; }
        public string cca2 { get; set; }
        public string cca3 { get; set; }
        public List<string> capital { get; set; }
        public string region { get; set; }
        public FlagsProperty flags { get; set; }
    }

    public class NameProperty
    {
        public string common { get; set; }
    }
    public class FlagsProperty
    {
        public string png { get; set; }
    }
}