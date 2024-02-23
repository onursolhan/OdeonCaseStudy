using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InvoiceWorker.Model;
using InvoiceWorker.Database;
using InvoiceWorker.Database.Queries;
using System.Linq;

namespace InvoiceWorker.Helper
{
    internal class ReservationMatcher
    {

        public static MatchingResult MatchInvoicesWithBookings(List<Invoice> invoices)
        {
            MatchingResult matchingResult = new MatchingResult();
            matchingResult.TotalRecords = invoices.Count;

            foreach (Invoice invoice in invoices)
            {
                List<Booking> flightBookings = Reservations.GetMatchingBookings(invoice);
                var matchingBooking = flightBookings.FirstOrDefault(booking => booking.Reservations.Count == invoice.NumberOfSeats && string.IsNullOrEmpty(booking.InvoiceNo));

                if (matchingBooking != null)
                {
                    if(matchingBooking.TotalPrice == invoice.TotalPrice)
                    {
                        try
                        {
                            Reservations.UpdateInvoiceNoByBookingId(matchingBooking.BookingID, invoice.InvoiceNo);
                            matchingResult.SuccesfulRecords++;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Error while updating reservations: {ex.Message}", ex);
                        }
                    }
                    else
                    {
                        matchingResult.InvoicesWithDifferentPrices.Add(invoice);
                    }
                }
                else
                {
                    var hasBookingWithDuplicatedInvoice = flightBookings.Any(booking => booking.Reservations.Count == invoice.NumberOfSeats);
                    if (hasBookingWithDuplicatedInvoice)
                    {
                        matchingResult.DuplicatedInvoices.Add(invoice);
                    }
                    else
                    {
                        matchingResult.UnmatchedInvoices.Add(invoice);
                    }

                }
            }

            return matchingResult;
        }

    }
}
