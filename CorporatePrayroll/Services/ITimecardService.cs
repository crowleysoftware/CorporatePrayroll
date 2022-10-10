namespace CorporatePrayroll.Services
{
    public interface ITimecardService
    {
        Task<TimeCard> GetTimeCardByEmployeeID(Payee payee, DateTime payrollDate);
    }
}