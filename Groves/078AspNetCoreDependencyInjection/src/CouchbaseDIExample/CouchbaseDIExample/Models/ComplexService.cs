using System.Diagnostics;
using Couchbase.Core;

namespace CouchbaseDIExample.Models
{
    public interface IComplexService
    {
        void ApproveApplication(string emailAddress);
    }

    // tag::ComplexService[]
    public class ComplexService : IComplexService
    {
        private readonly IBucket _bucket;
        private readonly IEmailService _email;

        public ComplexService(ITravelSampleBucketProvider bucketProvider, IEmailService emailService)
        {
            _bucket = bucketProvider.GetBucket();
            _email = emailService;
        }

        public void ApproveApplication(string emailAddress)
        {
            _bucket.Upsert(emailAddress, new {emailAddress, approved = true});
            _email.SendEmail(emailAddress, "Approved", "Your application has been approved!");
        }
    }
    // end::ComplexService[]

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