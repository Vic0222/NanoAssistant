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
        [Description("Get's the financial summary of the month for the date.")]
        [return:Description("Financial summary of the month. Like balance, total incomes and total expenses.")]
        public Task<FinanceMonthDto> GetFinancialSummary(DateTimeOffset balanceDate)
        {
            return _nanoFinanceTrackerService.GetFinanceMonthStatus(balanceDate, _accessToken);
        }

        [KernelFunction("add_expense")]
        [Description("Add the expense in cents to balance and returns the total expense in cents.")]
        [return: Description("Status of the finance month. Like balance, total incomes and total expenses.")]
        public Task<FinanceMonthDto> AddExpense(DateTimeOffset transactionDate, int expense, string category, string description)
        {
            return _nanoFinanceTrackerService.AddExpense(transactionDate, expense, category, description, _accessToken); ;
        }

        [KernelFunction("add_income")]
        [Description("Add the income in cents to balance and returns the total income in cents.")]
        [return: Description("Status of the finance month. Like balance, total incomes and total expenses.")]
        public Task<FinanceMonthDto> AddIncome(DateTimeOffset transactionDate, int income, string category, string description)
        {
            return _nanoFinanceTrackerService.AddIncome(transactionDate, income, category, description, _accessToken); ;
        }
    }

}
