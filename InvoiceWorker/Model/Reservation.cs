using System;

namespace InvoiceWorker.Model
{
    public class Reservation
    {
        public long BookingID { get; set; }
        public string Customer { get; set; }
        public string CarrierCode { get; set; }
        public int FlightNo { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public decimal Price { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime FlightDate { get; set; }
    }
}
