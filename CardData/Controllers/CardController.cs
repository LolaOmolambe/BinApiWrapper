using CardData.Logic;
using CardData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public CardController(ICardDataLogic cardDataLogic)
        {
            _cardDataLogic = cardDataLogic;
        }

        [HttpGet()]
        public object Get()
        {

            return new { Moniker = "fhjdf" };

        }

        [HttpGet("{digits}")]
        public async Task<CardDTO> Get(string digits)
        {
            var cardInfo = await _cardDataLogic.GetCardInfo(digits);
            return cardInfo;

        }
    }
}
