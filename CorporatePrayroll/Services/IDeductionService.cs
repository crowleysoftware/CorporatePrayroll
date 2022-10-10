namespace CorporatePrayroll.Services
{
    public interface IDeductionService
    {
        Task<List<Deduction>> GetPayeeDeductions(Payee payee, DateTime payrollDate);
        Task<decimal> GetGrossDeductions(Payee payee, DateTime payrollDate);
    }
}