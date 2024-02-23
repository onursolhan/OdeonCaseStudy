using InvoiceWorker.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace InvoiceWorker.Helper
{
    internal class EmailSender
    {
        private static readonly string RecipientEmail = "onursolhan@hotmail.com,onursolhan@gmail.com";
        private static readonly string SmtpServer = "smtp.office365.com";
        private static readonly int Port = 587;
        private static readonly string Username = "onurtestacc@outlook.com";
        private static readonly string Password = "very_hard_password";

        public static void SendEmailWithCsvAttachment(MatchingResult result, string fileName)
        {
            using (SmtpClient smtp = new SmtpClient(SmtpServer))
            {
                smtp.Port = Port;
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(Username, Password);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(Username);
                    mail.To.Add(RecipientEmail);
                    mail.Subject = fileName + " imported";
                    mail.Body = CreateMailBody(result);

                    AttachCsvIfNotEmpty(mail.Attachments, result.UnmatchedInvoices, "UnmatchedInvoices.csv");
                    AttachCsvIfNotEmpty(mail.Attachments, result.DuplicatedInvoices, "DuplicatedInvoices.csv");
                    AttachCsvIfNotEmpty(mail.Attachments, result.InvoicesWithDifferentPrices, "InvoicesWithDifferentPrices.csv");

                    
                    try
                    {
                        smtp.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error sending email: {ex.Message}");
                    }
                }
            }
        }
        private static string CreateMailBody(MatchingResult result)
        {
            string body = string.Format("We received an invoice and imported it to our booking system.\nTotal processed records: {0}\nSuccessful records: {1}\nTotal invalid records: {2}", 
                result.TotalRecords, result.SuccesfulRecords, result.InvalidRecords);
            if (result.InvalidRecords > 0)
            {
                if (result.UnmatchedInvoices.Any())
                {
                    body += string.Format("\nInvoices are not found in booking: {0} / Total Price: {1}", 
                        result.UnmatchedInvoices.Count, result.UnmatchedInvoices.Sum(invoice => invoice.TotalPrice));
                }
                if (result.DuplicatedInvoices.Any())
                {
                    body += string.Format("\nWe already got a booking’s invoice : {0} / Total Price: {1}", 
                        result.DuplicatedInvoices.Count, result.DuplicatedInvoices.Sum(invoice => invoice.TotalPrice));
                }
                if (result.InvoicesWithDifferentPrices.Any())
                {
                    body += string.Format("\nInvoice and booking have different prices: {0} / Total Price: {1}", 
                        result.InvoicesWithDifferentPrices.Count, result.InvoicesWithDifferentPrices.Sum(invoice => invoice.TotalPrice));
                }

                body += "\nLists of invalid invoices are included in the attachments ";
            }
            return body;
        }

        private static void AttachCsvIfNotEmpty(AttachmentCollection attachments, List<Invoice> invoices, string fileName)
        {
            if (invoices.Any())
            {
                attachments.Add(CreateCsvAttachment(invoices, fileName));
            }
        }
        private static Attachment CreateCsvAttachment(List<Invoice> invoices, string fileName)
        {
            MemoryStream csvMemoryStream = CsvExporter.ExportToMemoryStream(invoices);
            return new Attachment(csvMemoryStream, fileName, "text/csv");
        }
    }
}
