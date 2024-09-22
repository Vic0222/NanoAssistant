using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoAssistant.Core.InternalDtos
{
    public class AddExpenseCommandDto
    {
        public decimal Amount { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public DateTimeOffset TransactionDate { get; set; }
    }
}
