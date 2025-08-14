using CryptoCore.Services.Interfaces;
using CryptoCore.ViewModels.Requests;
using CryptoCore.ViewModels.Respones;
using CryptoInfrastructure;
using CryptoInfrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CryptoCore.Services.Implements
{
    public class CryptoServices : ICryptoServices
    {
        private readonly CryptoDbcontext _cryptoDbcontext;
        private readonly ILogger<CryptoServices> _logger;
        private readonly IDistributedCache _cache;
        public CryptoServices(CryptoDbcontext cryptoDbcontext, ILogger<CryptoServices> logger, IDistributedCache cache)
        {
            _cryptoDbcontext = cryptoDbcontext;
            _logger = logger;
            _cache = cache;
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

        public async Task<BaseRespone> ExchangeTokenAsync(string adminId, ExchangeTokenRequest exchangeTokenRequest)
        {
            try
            {
                var response = new BaseRespone();
                var userToken = new UserToken();
                var existingUserToken = _cryptoDbcontext.UserTokens
                    .FirstOrDefault(ut => ut.UserId == exchangeTokenRequest.UserId && ut.TokenId == exchangeTokenRequest.TokenId);
                if (exchangeTokenRequest == null || string.IsNullOrEmpty(exchangeTokenRequest.UserId) || exchangeTokenRequest.AmountVnd <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid request data.";
                    return response;
                }
                var userCrytoExchange = new UserCryptoExchange
                {
                    UserId = exchangeTokenRequest.UserId,
                    TokenId = exchangeTokenRequest.TokenId,
                    TokenAmount = exchangeTokenRequest.TokenAmount,
                    AmountVnd = exchangeTokenRequest.AmountVnd,
                    AmountUsdt = exchangeTokenRequest.AmountUsdt,
                    Price = exchangeTokenRequest.AmountVnd / exchangeTokenRequest.TokenAmount,
                    IsBuy = exchangeTokenRequest.IsBuy,
                    CreatedBy = adminId ?? "default", // Default creator if adminId is null
                    ExchangeDate = DateTime.UtcNow
                };
                if (userCrytoExchange.IsBuy)
                {
                    // Ensure the user has enough balance for the purchase
                    var user = await _cryptoDbcontext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == exchangeTokenRequest.UserId);
                    if (user == null || user.Balance < exchangeTokenRequest.AmountVnd)
                    {
                        response.IsSuccess = false;
                        response.Message = "Insufficient balance.";
                        return response;
                    }

                    // Ensure the token exists
                    if (existingUserToken != null)
                    {
                        // Update existing user token
                        existingUserToken.CurrentAmount += exchangeTokenRequest.TokenAmount;
                        _cryptoDbcontext.UserTokens.Update(existingUserToken);
                    }
                    else
                    {
                        // Create new user token
                        userToken.UserId = exchangeTokenRequest.UserId;
                        userToken.TokenId = exchangeTokenRequest.TokenId;
                        userToken.CurrentAmount = exchangeTokenRequest.TokenAmount;
                        await _cryptoDbcontext.UserTokens.AddAsync(userToken);
                    }
                    user.Balance -= exchangeTokenRequest.AmountVnd; // Deduct the amount from user's balance
                    // Update user's balance
                    _cryptoDbcontext.Users.Update(user);
                }
                else if (!userCrytoExchange.IsBuy)
                {
                    var user = await _cryptoDbcontext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == exchangeTokenRequest.UserId);
                    if (user == null)
                    {
                        response.IsSuccess = false;
                        response.Message = "User not found.";
                        return response;
                    }
                    // Ensure the token exists
                    if (existingUserToken == null)
                    {
                        response.IsSuccess = false;
                        response.Message = "Token not found for the user.";
                        return response;
                    }
                    existingUserToken.CurrentAmount -= exchangeTokenRequest.TokenAmount;
                    existingUserToken.LastUpdated = DateTime.UtcNow; // Update last updated time
                    if (existingUserToken.CurrentAmount < 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Insufficient token amount to sell.";
                        return response;
                    }
                    // Update existing user token
                    _cryptoDbcontext.UserTokens.Update(existingUserToken);

                    // Ensure the user has enough tokens to sell
                    var userCrypto = await _cryptoDbcontext.UserCryptoExchanges.AsNoTracking()
                        .Where(uc => uc.UserId == exchangeTokenRequest.UserId && uc.TokenId == exchangeTokenRequest.TokenId).ToListAsync();
                    var currentTokenAmount = userCrypto.Sum(uc => uc.TokenAmount);
                    if (userCrypto == null || currentTokenAmount < exchangeTokenRequest.TokenAmount)
                    {
                        response.IsSuccess = false;
                        response.Message = "Insufficient token amount.";
                        return response;
                    }
                    userCrytoExchange.TokenAmount = exchangeTokenRequest.TokenAmount;
                    user.Balance += exchangeTokenRequest.AmountVnd; // Add the amount to user's balance
                    // Update user's balance
                    _cryptoDbcontext.Users.Update(user);
                }
                await _cryptoDbcontext.UserCryptoExchanges.AddAsync(userCrytoExchange);
                var saveCount = await _cryptoDbcontext.SaveChangesAsync();

                response.IsSuccess = saveCount > 0;
                if (!response.IsSuccess)
                {
                    response.Message = "Failed to exchange token.";
                    return response;
                }
                response.Message = "Token exchanged successfully.";
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error exchanging token for user: {UserId}", exchangeTokenRequest.UserId);
                return new BaseRespone
                {
                    IsSuccess = false,
                    Message = "Error exchanging token: " + e.Message
                };
            }
        }

        public async Task<List<TokenRespone>> GetTokensByUserEmail(string email)
        {
            try
            {
                var response = new List<TokenRespone>();
                var user = await _cryptoDbcontext.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (user == null)
                {
                    throw new Exception("User not found.");
                }
                var userCryptoExchanges = await _cryptoDbcontext.UserCryptoExchanges
                    .Where(x => x.UserId == user.Id)
                    .Include(x => x.CryptoToken)
                    .ToListAsync();
                if (userCryptoExchanges == null || !userCryptoExchanges.Any())
                {
                    return response;
                }

                var groupTokens = userCryptoExchanges
                    .GroupBy(x => x.TokenId)
                    .Select(g => new TokenRespone
                    {
                        TokenId = g.Key,
                        TokenName = g.FirstOrDefault()?.CryptoToken?.Name ?? "Unknown",
                        Symbol = g.FirstOrDefault()?.CryptoToken?.Symbol ?? "UNK",
                        Transactions = g.Select(x => new Transaction
                        {
                            TokenAmount = x.TokenAmount,
                            AmountVnd = x.AmountVnd,
                            AmountUsdt = x.AmountUsdt,
                            Price = x.Price,
                            IsBuy = x.IsBuy,
                            ExchangeDate = x.ExchangeDate,
                            CreatedBy = x.CreatedBy
                        }).ToList(),
                        TotalTokenAmount = g.Where(x => x.IsBuy).Sum(x => x.TokenAmount) - g.Where(x => !x.IsBuy).Sum(x => x.TokenAmount),
                    }).ToList();
                if (groupTokens == null || !groupTokens.Any())
                {
                    throw new Exception("No grouped tokens found for the user.");
                }
                var tokenPriceData = new TokenPriceCaching();
                var tokenPriceDataCaching = await _cache.GetAsync("tokenPriceCaching");
                if (tokenPriceDataCaching != null)
                {
                    tokenPriceData = JsonSerializer.Deserialize<TokenPriceCaching>(tokenPriceDataCaching);
                }

                foreach (var group in groupTokens)
                {
                    if (tokenPriceData == null || tokenPriceData.TokenPrices == null || !tokenPriceData.TokenPrices.Any())
                    {
                        throw new Exception("Token price data is not available.");
                    }
                    var tokenPrice = tokenPriceData.TokenPrices.FirstOrDefault(x => x.Symbol == group.Symbol)?.Price;
                    if (tokenPrice == null)
                    {
                        tokenPrice = 0; // Default to 0 if price is not found
                    }
                    group.CurrentPrice = (float)tokenPrice * 26431; // Assuming a fixed price of 40000 for simplicity
                    group.TotalValue = group.TotalTokenAmount * group.CurrentPrice;
                }
                response = groupTokens;
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
