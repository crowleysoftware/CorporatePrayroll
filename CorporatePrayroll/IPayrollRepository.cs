namespace CorporatePrayroll
{
    public interface IPayrollRepository
    {
        IAsyncEnumerable<Deduction> GetDeductions();
        IAsyncEnumerable<Deduction> GetDeductionsByEmployeeID(int employeeID);
        IAsyncEnumerable<Payee> GetPayees();
        Task<TaxTable> GetTaxTables(string stateCode);
        IAsyncEnumerable<TaxTable> GetTaxTables();
        Task<TimeCard> GetTimecardByEmployeeID(int employeeID, DateTime payrollDate);
        IAsyncEnumerable<TimeCard> GetTimeCards();
    }
}