using CardData.Logic;
using CardData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardData.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class CardController : ControllerBase
    {
        private readonly ICardDataLogic _cardDataLogic;
        private readonly IMemoryCache _cache;

        public CardController(ICardDataLogic cardDataLogic, IMemoryCache cache)
        {
            _cardDataLogic = cardDataLogic;
            _cache = cache;
        }

        [HttpGet("{digits}")]
        public async Task<CardDTO> Get(string digits)
        {
            var cardData = new CardDTO();

            if (!_cache.TryGetValue("CardData", out cardData))
            {
                if(cardData == null)
                {
                    cardData = await GetCardData(digits);
                }

                _cache.Set("CardData", cardData,
                    new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(39))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return cardData;

        }

        private async Task<CardDTO> GetCardData(string digits)
        {
            var cardInfo = await _cardDataLogic.GetCardInfo(digits);
            return cardInfo;

        }
    }
}
