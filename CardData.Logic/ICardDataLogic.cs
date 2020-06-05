using CardData.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CardData.Logic
{
     public interface ICardDataLogic
    {
        Task<CardDTO> GetCardInfo(string digits);
    }
}
