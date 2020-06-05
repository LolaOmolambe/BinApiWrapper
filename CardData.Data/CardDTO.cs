using System;
using System.Collections.Generic;
using System.Text;

namespace CardData.Models
{
    public class CardDTO
    {
        public CardNumberDTO Number { get; set; }
        public string Scheme { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public bool Prepaid { get; set; }
        public CardCountryDTO Country { get; set; }
        public CardBankDTO Bank { get; set; }
    }
}
