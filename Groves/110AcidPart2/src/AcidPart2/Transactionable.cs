using System.Collections.Generic;

namespace AcidPart2
{
    public abstract class Transactionable
    {
        public List<string> Transactions { get; set; } = new List<string>();
    }
}