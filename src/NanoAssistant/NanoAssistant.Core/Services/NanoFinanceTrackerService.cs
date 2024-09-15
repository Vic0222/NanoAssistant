using Microsoft.Extensions.Logging;
using NanoAssistant.Core.InternalDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace NanoAssistant.Core.Services
{
    public interface INanoFinanceTrackerService
    {
        Task<FinanceMonthDto> AddExpense(string account, DateTimeOffset transactionDate, int expense, string category, string description, string accessToken);
        Task<FinanceMonthDto> AddIncome(string account, DateTimeOffset transactionDate, int income, string category, string description, string accessToken);
        Task<List<FinancialTransactionDto>> GetBreakdown(string account, DateTimeOffset date, string accessToken);
        Task<FinanceMonthDto> GetFinanceMonthStatus(string account, DateTimeOffset dateTime, string accessToken);
    }

    public class NanoFinanceTrackerService : INanoFinanceTrackerService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NanoFinanceTrackerService> _logger;

        public NanoFinanceTrackerService(HttpClient httpClient, ILogger<NanoFinanceTrackerService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<FinanceMonthDto> AddIncome(string account, DateTimeOffset transactionDate, int income, string category, string description, string accessToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await _httpClient.PostAsJsonAsync($"api/FinanceMonth/{account}/{transactionDate.Year}/{transactionDate.Month}/incomes", new AddIncomeCommandDto()
                {
                    Amount = income,
                    Category = category,
                    Description = description,
                    TransactionDate = transactionDate
                });
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<FinanceMonthDto>() ?? new FinanceMonthDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting finance month , {TransactionDate}", transactionDate);
                throw;
            }
        }

        public async Task<FinanceMonthDto> AddExpense(string account, DateTimeOffset transactionDate, int expense, string category, string description, string accessToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await _httpClient.PostAsJsonAsync($"api/FinanceMonth/{account}/{transactionDate.Year}/{transactionDate.Month}/expenses", new AddExpenseCommandDto()
                {
                    Amount = expense,
                    Category = category,
                    Description = description,
                    TransactionDate = transactionDate
                });
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<FinanceMonthDto>() ?? new FinanceMonthDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting finance month , {TransactionDate}", transactionDate);
                throw;
            }
        }

        public async Task<FinanceMonthDto> GetFinanceMonthStatus(string account, DateTimeOffset dateTime, string accessToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await _httpClient.GetAsync($"api/FinanceMonth/{account}/{dateTime.Year}/{dateTime.Month}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<FinanceMonthDto>() ?? new FinanceMonthDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting finance month , {DateTime}", dateTime);
                throw;
            }
            
        }

        public async Task<List<FinancialTransactionDto>> GetBreakdown(string account, DateTimeOffset date, string accessToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await _httpClient.GetAsync($"api/FinanceMonth/{account}/{date.Year}/{date.Month}/transactions");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<FinancialTransactionDto>>() ?? new List<FinancialTransactionDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting finance month , {DateTime}", date);
                throw;
            }
        }
    }
}
