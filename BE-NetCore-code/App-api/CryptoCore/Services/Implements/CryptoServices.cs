using CryptoCore.Services.Interfaces;
using CryptoInfrastructure;
using CryptoInfrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCore.Services.Implements
{
    public class CryptoServices : ICryptoServices
    {
        private readonly CryptoDbcontext _cryptoDbcontext;
        public CryptoServices(CryptoDbcontext cryptoDbcontext) {
            _cryptoDbcontext = cryptoDbcontext;
        }
        public async Task<List<CryptoToken>> GetAllCryptoTokenAsync()
        {
            try
            {
                var result = new List<CryptoToken>();
                result = await _cryptoDbcontext.CryptoTokens.AsNoTracking().Where(x => x.IsActive).ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Error fetching crypto tokens: " + e.Message);
            }
        }

        public async Task<bool> GenegrateDefaultDataAsync()
        {
            try
            {
                if (await _cryptoDbcontext.CryptoTokens.AnyAsync())
                {
                    return false; // Data already exists
                }
                var defaultTokenData = new List<CryptoToken>
                {
                    new CryptoToken { Name = "Bitcoin", Symbol = "BTC", CurrentPrice = 0, HighestPrice = 0, IsActive = true },
                    new CryptoToken { Name = "Ethereum", Symbol = "ETH",CurrentPrice = 0, HighestPrice = 0, IsActive = true },
                    new CryptoToken { Name = "Ripple", Symbol = "XRP", CurrentPrice = 0, HighestPrice = 0, IsActive = true },
                    new CryptoToken { Name = "Litecoin", Symbol = "LTC", CurrentPrice = 0, HighestPrice = 0, IsActive = true },
                    new CryptoToken { Name = "Cardano", Symbol = "ADA", CurrentPrice = 0, HighestPrice = 0, IsActive = true },
                    new CryptoToken { Name = "Solana", Symbol = "SOL", CurrentPrice = 0, HighestPrice = 0, IsActive = true },
                };
                await _cryptoDbcontext.CryptoTokens.AddRangeAsync(defaultTokenData);

                var defaultGroup = new Group
                {
                    Id = Constants.Id.DEFAULTGROUP,
                    Name = "Default Group",
                    AdminId = Constants.Id.DEFAULTGROUP
                };
                await _cryptoDbcontext.Groups.AddAsync(defaultGroup);

                var saveSucceed = await _cryptoDbcontext.SaveChangesAsync();
                if (saveSucceed <= 0)
                {
                    throw new Exception("Failed to save default data.");
                }
                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Error generating default data: " + e.Message);
            }
        }
    }
}
