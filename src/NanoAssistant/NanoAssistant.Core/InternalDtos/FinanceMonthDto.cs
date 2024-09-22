using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoAssistant.Core.InternalDtos
{
    public class FinanceMonthDto
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public decimal Balance { get; set; }

        public decimal TotalExpense { get; set; }

        public decimal TotalIncome { get; set; }
    }
}
