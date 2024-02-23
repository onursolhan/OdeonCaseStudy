using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceWorker.Model
{
    public class Invoice
    {
        public DateTime FlightDate { get; set; }
        public string CarrierCode { get; set; }
        public string FlightNo { get; set; }
        public int NumberOfSeats { get; set; }
        public decimal PricePerSeat { get; set; }
        public decimal TotalPrice { get; set; }
        public string InvoiceNo { get; set; }
        public string CarrierAndFlight => $"{CarrierCode} {FlightNo}";
        public string FlightDateStr => FlightDate.ToShortDateString();
    }
}
