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
    }
}
