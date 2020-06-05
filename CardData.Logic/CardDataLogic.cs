using CardData.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CardData.Logic
{
    public class CardDataLogic : ICardDataLogic
    {
        private readonly ILogger<CardDataLogic> _logger;
        private readonly IConfiguration _configuration;
        public CardDataLogic(ILogger<CardDataLogic> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<CardDTO> GetCardInfo(string digits)
        {
            var cardInfo = new CardDTO();
            try
            {
                if (digits == null)
                {
                    throw new ArgumentException("Expecting a digits of a Card Number");
                }
                if (!digits.Trim().All(c => char.IsNumber(c)) || string.IsNullOrWhiteSpace(digits))
                    throw new ArgumentException("Enter a valid BIN/IIN number.");

                var uri = _configuration["BaseUrl"];
                uri = uri + digits;
                using (var client = new HttpClient())
                {
                    _logger.LogDebug("Calling API with Card digits of {digits} ", digits);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.GetAsync(uri);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Failed in API For Card Digits: {digits}. ResponseCode =  " + $"{response.StatusCode}");
                    }
                    var content = await response.Content.ReadAsStringAsync();
                    cardInfo = JsonConvert.DeserializeObject<CardDTO>(content);
                    if (cardInfo == null)
                    {
                        _logger.LogWarning("No Card information found for  {digits}", digits);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failure in API call", ex);
            }
            return cardInfo;
        }
    }
}
