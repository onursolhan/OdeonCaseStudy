using System.Collections.Generic;

namespace InvoiceWorker.Model
{
    internal class MatchingResult
    {
        public MatchingResult()
        {
            SuccesfulRecords = 0;
            UnmatchedInvoices = new List<Invoice>();
            DuplicatedInvoices = new List<Invoice>();
            InvoicesWithDifferentPrices = new List<Invoice>();
        }
        public int TotalRecords { get; set; }
        public int SuccesfulRecords { get; set; }
        public int InvalidRecords => TotalRecords - SuccesfulRecords;

        public List<Invoice> UnmatchedInvoices { get; set; }
        public List<Invoice> DuplicatedInvoices { get; set; }
        public List<Invoice> InvoicesWithDifferentPrices { get; set; }
    }
}
