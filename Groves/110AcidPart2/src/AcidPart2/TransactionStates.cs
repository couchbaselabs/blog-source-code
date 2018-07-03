namespace AcidPart2
{
    // tag::TransactionStates[]
    public enum TransactionStates
    {
        Initial = 0,
        Pending = 1,
        Committed = 2,
        Done = 3,
        Cancelling = 4,
        Cancelled = 5
    }
    // end::TransactionStates[]
}