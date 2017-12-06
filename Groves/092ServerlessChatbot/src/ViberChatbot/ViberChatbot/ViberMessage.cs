namespace ViberChatbot
{
    // tag::ViberClasses[]
    public class ViberIncoming
    {
        public string Event { get; set; }
        public long Timestamp { get; set; }
        public ViberSender Sender { get; set; }
        public ViberMessage Message { get; set; }
    }

    public class ViberSender
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class ViberMessage
    {
        public string Text { get; set; }
        public string Type { get; set; }
    }
    // end::ViberClasses[]
}