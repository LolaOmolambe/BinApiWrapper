using System;
using System.Collections.Generic;
using System.Text;

namespace CardData.Models
{
    public class CardCountryDTO
    {
        public string Numeric { get; set; }
        public string Alpha2 { get; set; }
        public string Name { get; set; }
        public string Emoji { get; set; }
        public string Currency { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
