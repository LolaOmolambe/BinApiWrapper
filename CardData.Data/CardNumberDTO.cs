using System;
using System.Collections.Generic;
using System.Text;

namespace CardData.Models
{
    public class CardNumberDTO
    {
        public int Length { get; set; }
        public bool Luhn { get; set; }
    }
}
