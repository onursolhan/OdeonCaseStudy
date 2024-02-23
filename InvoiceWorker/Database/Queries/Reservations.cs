using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using InvoiceWorker.Model;

namespace InvoiceWorker.Database.Queries
{
    internal class Reservations
    {
        public static List<Reservation> GetMatchingReservations(Invoice invoice)
        {
            var reservations = new List<Reservation>();
            using (var connection = ConnectionHelper.CreateSqlConnection())
            using (var command = new SqlCommand("SELECT * FROM Bookings WHERE FlightDate = @FlightDate AND FlightNo = @FlightNo AND CarrierCode = @CarrierCode", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@FlightDate", invoice.FlightDate);
                command.Parameters.AddWithValue("@FlightNo", invoice.FlightNo);
                command.Parameters.AddWithValue("@CarrierCode", invoice.CarrierCode);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reservations.Add(new Reservation
                        {
                            BookingID = (long)reader["BookingID"],
                            Customer = (string)reader["Customer"],
                            CarrierCode = (string)reader["CarrierCode"],
                            Destination = (string)reader["Destination"],
                            FlightDate = (DateTime)reader["FlightDate"],
                            InvoiceNo = reader["InvoiceNo"] == DBNull.Value ? "" : (string)reader["InvoiceNo"],
                            FlightNo = (int)reader["FlightNo"],
                            Origin = (string)reader["Origin"],
                            Price = (int)reader["Price"]
                        });
                    }
                }
            }

            return reservations;
        }

        public static List<Booking> GetMatchingBookings(Invoice invoice)
        {
            var bookings = new List<Booking>();
            using (var connection = ConnectionHelper.CreateSqlConnection())
            using (var command = new SqlCommand("SELECT * FROM Bookings WHERE FlightDate = @FlightDate AND FlightNo = @FlightNo AND CarrierCode = @CarrierCode", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@FlightDate", invoice.FlightDate);
                command.Parameters.AddWithValue("@FlightNo", invoice.FlightNo);
                command.Parameters.AddWithValue("@CarrierCode", invoice.CarrierCode);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var reservation = new Reservation
                        {
                            BookingID = (long)reader["BookingID"],
                            Customer = (string)reader["Customer"],
                            CarrierCode = (string)reader["CarrierCode"],
                            Destination = (string)reader["Destination"],
                            FlightDate = (DateTime)reader["FlightDate"],
                            InvoiceNo = reader["InvoiceNo"] == DBNull.Value ? "" : (string)reader["InvoiceNo"],
                            FlightNo = (int)reader["FlightNo"],
                            Origin = (string)reader["Origin"],
                            Price = (decimal)reader["Price"]
                        };

                        var booking = bookings.FirstOrDefault(b => b.BookingID == reservation.BookingID);
                        if (booking != null)
                        {
                            booking.Reservations.Add(reservation);
                        }
                        else
                        {
                            bookings.Add(new Booking
                            {
                                BookingID = reservation.BookingID,
                                InvoiceNo = reservation.InvoiceNo,
                                Reservations = new List<Reservation> { reservation }
                            });
                        }
                    }
                }
            }

            return bookings;
        }

        public static void UpdateInvoiceNoByBookingId(long bookingID, string invoiceNo)
        {
            using (var connection = ConnectionHelper.CreateSqlConnection())
            using (var command = new SqlCommand("UPDATE Bookings SET InvoiceNo = @InvoiceNo WHERE BookingID = @BookingID AND 1 = 0", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@InvoiceNo", invoiceNo);
                command.Parameters.AddWithValue("@BookingID", bookingID);
                command.ExecuteNonQuery();
            }
        }
    }
}
