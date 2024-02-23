using System.Collections.Generic;
using System.Linq;

namespace InvoiceWorker.Model
{
    internal class Booking
    {
        public long BookingID { get; set; }
        public string InvoiceNo { get; set; }
        public decimal TotalPrice => Reservations.Sum(reservation => reservation.Price);
        public List<Reservation> Reservations { get; set; }
    }
}
