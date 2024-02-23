using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using InvoiceWorker.Model;

namespace InvoiceWorker.Helper
{
    internal class CsvExporter
    {
        public static MemoryStream ExportToMemoryStream(List<Invoice> invoices)
        {
            var memoryStream = new MemoryStream();

            using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, 1024, true))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<InvoiceMap>();
                csv.WriteRecords(invoices);
                writer.Flush();
            }

            memoryStream.Position = 0;

            return memoryStream;
        }
    }
    public class InvoiceMap : ClassMap<Invoice>
    {
        public InvoiceMap()
        {
            Map(p => p.FlightDateStr).Name("Flagdatum");
            Map(p => p.CarrierAndFlight).Name("FlugNr");
            Map(p => p.NumberOfSeats).Name("Anzahl");
            Map(p => p.PricePerSeat).Name("Einzelpreis");
            Map(p => p.TotalPrice).Name("BetragInEUR");
        }
    }
}
