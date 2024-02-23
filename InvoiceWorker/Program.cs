using InvoiceWorker.Helper;
using System;
using System.IO;

namespace InvoiceWorker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string fileName = "Invoice_10407.PDF";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "_Document", fileName);

            var invoices = PdfParser.ParsePdf(filePath);
            Console.WriteLine($"Found {invoices.Count} invoices...");

            var result = ReservationMatcher.MatchInvoicesWithBookings(invoices);
            Console.WriteLine($"Processed {result.TotalRecords} invoices. {result.SuccesfulRecords} successful...");

            Console.WriteLine("Will try to send mail...");
            EmailSender.SendEmailWithCsvAttachment(result, fileName);
            Console.WriteLine("Mail sent!");

            Console.ReadLine();
        }
    }
}
