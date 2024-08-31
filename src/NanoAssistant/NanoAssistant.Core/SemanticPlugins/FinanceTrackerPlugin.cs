using Microsoft.SemanticKernel;
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
        private static int _balance = 0;
        private static int _totalExpense = 0;
        private static int _totalIncome = 0;

        [KernelFunction("get_balance")]
        [Description("Get the balance in cents.")]
        public async Task<int> GetBalance()
        {
            return _balance;
        }

        [KernelFunction("add_expense")]
        [Description("Add the expense in cents to balance and returns the total expense in cents.")]
        public async Task<int> AddExpense(int expense)
        {
            _totalExpense += expense;
            _balance -= expense;
            return _totalExpense;
        }

        [KernelFunction("add_income")]
        [Description("Add the income in cents to balance and returns the total income in cents.")]
        public async Task<int> AddIncome(int income)
        {
            _totalIncome += income;
            _balance += income;
            return _totalIncome;
        }
    }
}
