
namespace CorporatePrayroll.Services
{
    public class PayeeService : IPayeeService
    {
        private readonly IPayrollRepository payrollRepository;

        public PayeeService(IPayrollRepository payrollRepository)
        {
            this.payrollRepository = payrollRepository;
        }

        public IAsyncEnumerable<Payee> GetActivePayees()
        {
            //Realistically payees would have associated data that determined
            //if they are actively employed. To keep it simple we just get everyone.
            return payrollRepository.GetPayees();
        }
    }
}
