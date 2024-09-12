using Microsoft.SemanticKernel;
using NanoAssistant.Core.InternalDtos;
using NanoAssistant.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoAssistant.Core.SemanticPlugins
{
    public class FinanceTrackerPlugin
    {
        private readonly INanoFinanceTrackerService _nanoFinanceTrackerService;
        private string _accessToken = string.Empty;

        public FinanceTrackerPlugin(INanoFinanceTrackerService nanoFinanceTrackerService)
        {
            _nanoFinanceTrackerService = nanoFinanceTrackerService;
        }

        /// <summary>
        /// A work around so this class
        /// </summary>
        /// <param name="accessToken"></param>
        public void SetAccessToken(string accessToken)
        {
            _accessToken = accessToken;
        }

        [KernelFunction("get_finance_summary")]
        [Description("Get's the financial summary of the month for the date in cents.")]
        [return:Description("Financial summary of the month. Like balance, total incomes and total expenses in cents.")]
        public Task<FinanceMonthDto> GetFinancialSummary([Description("Required")] string account, DateTimeOffset balanceDate)
        {
            return _nanoFinanceTrackerService.GetFinanceMonthStatus(account, balanceDate, _accessToken);
        }

        [KernelFunction("add_expense")]
        [Description("Add the expense in cents.")]
        [return: Description("Financial summary in cents.")]
        public Task<FinanceMonthDto> AddExpense([Description("Required")] string account, DateTimeOffset transactionDate, [Description("The expense in cents.")] int expense, string category, string description)
        {
            return _nanoFinanceTrackerService.AddExpense(account, transactionDate, expense, category, description, _accessToken); ;
        }

        [KernelFunction("add_income")]
        [Description("Add the income in cents.")]
        [return: Description("Financial summary in cents.")]
        public Task<FinanceMonthDto> AddIncome([Description("Required")] string account, DateTimeOffset transactionDate, [Description("The income in cents.")] int income, string category, string description)
        {
            return _nanoFinanceTrackerService.AddIncome(account, transactionDate, income, category, description, _accessToken); ;
        }
    }

}
