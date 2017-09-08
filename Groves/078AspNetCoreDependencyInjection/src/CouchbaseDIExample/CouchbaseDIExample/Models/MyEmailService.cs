using System.Diagnostics;

namespace CouchbaseDIExample.Models
{
    public interface IEmailService
    {
        void SendEmail(string to, string subject, string body);
    }

    public class MyEmailService : IEmailService
    {
        public void SendEmail(string to, string subject, string body)
        {
            // this isn't going to actually send an email
            // instead just write out to debug
            Debug.Write($"Email to {to}, Subject: {subject}, Body: {body}");
        }
    }
}