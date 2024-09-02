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

        public int Balance { get; set; }

        public int TotalExpense { get; set; }

        public int TotalIncome { get; set; }
    }
}
