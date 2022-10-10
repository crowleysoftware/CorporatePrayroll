namespace CorporatePrayroll.Services
{
    public interface IPayeeService
    {
        IAsyncEnumerable<Payee> GetActivePayees();
    }
}