using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using InvoiceWorker.Model;

namespace InvoiceWorker.Helper
{
    internal class PdfParser
    {
        public static List<Invoice> ParsePdf(string fileName)
        {
            using (var reader = new PdfReader(fileName))
            {
                return ParsePdf(reader);
            }
        }

        public static List<Invoice> ParsePdf(Stream fileStream)
        {
            using (var reader = new PdfReader(fileStream))
            {
                return ParsePdf(reader);
            }
        }

        private static List<Invoice> ParsePdf(PdfReader pdfReader)
        {
            List<Invoice> invoices = new List<Invoice>();

            for (int i = 1; i <= pdfReader.NumberOfPages; i++)
            {
                string pageText = PdfTextExtractor.GetTextFromPage(pdfReader, i);
                var invoiceInPage = GetInvoicesFromPage(pageText);
                invoices.AddRange(invoiceInPage);
            }

            return invoices;
        }

        private static List<Invoice> GetInvoicesFromPage(string pdfText)
        {
            List<Invoice> invoices = new List<Invoice>();

            string[] lines = pdfText.Split('\n');

            string invoiceNumber = null;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (invoiceNumber == null)
                {
                    if (line.Contains("Nummer Seite Datum"))
                    {
                        invoiceNumber = lines[i + 1].Split(' ')[0].Trim();
                    }
                }
                else
                {
                    Invoice invoice = ParseInvoiceFromLine(line, invoiceNumber);
                    if (invoice != null)
                    {
                        invoices.Add(invoice);
                    }
                }
            }

            return invoices;
        }

        private static Invoice ParseInvoiceFromLine(string line, string invoiceNumber)
        {
            string pattern = @"(?<FlightDate>\d{2}\.\d{2}\.\d{4})\s+" +
                             @"(?<FlightNumber>[A-Z]{2} \d{3})\s+" +
                             @"(?<OriginDestination>[A-Z]{3} [A-Z]{3})\s+" +
                             @"(?<NumberOfSeats>\d+)\s+" +
                             @"(?<PricePerSeat>\d+,\d{2})\s+" +
                             @"(?<TotalPrice>\d+,\d{2})";

            Match match = Regex.Match(line, pattern);

            if (match.Success)
            {
                var FlightNumber = match.Groups["FlightNumber"].Value.Split(' ');
                return new Invoice
                {
                    FlightDate = Convert.ToDateTime(match.Groups["FlightDate"].Value),
                    CarrierCode = FlightNumber[0],
                    FlightNo = FlightNumber[1],
                    NumberOfSeats = int.Parse(match.Groups["NumberOfSeats"].Value),
                    PricePerSeat = decimal.Parse(match.Groups["PricePerSeat"].Value),
                    TotalPrice = decimal.Parse(match.Groups["TotalPrice"].Value),
                    InvoiceNo = invoiceNumber
                };
            }

            return null;
        }

    }
}
