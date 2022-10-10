
namespace CorporatePrayroll.Services
{
    public class TimecardService : ITimecardService
    {
        private readonly IPayrollRepository payrollRepository;

        public TimecardService(IPayrollRepository payrollRepository)
        {
            this.payrollRepository = payrollRepository;
        }

        public async Task<TimeCard> GetTimeCardByEmployeeID(Payee payee, DateTime payrollDate)
        {
            return await payrollRepository.GetTimecardByEmployeeID(payee.ID, payrollDate);
        }
    }
}
