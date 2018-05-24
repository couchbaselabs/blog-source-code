namespace AcidPart2
{
    public class TransactionRecord
    {
        public string SourceId { get; set; }
        public Barn Source { get; set; }
        public string DestinationId { get; set; }
        public Barn Destination { get; set; }
        public int Amount { get; set; }
        public TransactionStates State { get; set; }
    }
}