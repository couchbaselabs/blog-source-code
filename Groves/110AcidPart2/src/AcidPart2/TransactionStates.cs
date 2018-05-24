namespace AcidPart2
{
    public enum TransactionStates
    {
        Initial = 0,
        Pending = 1,
        Committed = 2,
        Done = 3,
        Cancelling,
        Cancelled
    }
}