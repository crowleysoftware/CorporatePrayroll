using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CorporatePrayroll
{
    public class PayrollRepository : IPayrollRepository, IDisposable
    {
        private readonly IConfiguration config;
        private readonly SqlConnection sqlConnection;
        private bool disposedValue;

        public PayrollRepository()
        {

        }

        public PayrollRepository(IConfiguration config)
        {
            this.config = config;

            sqlConnection = new SqlConnection(config.GetConnectionString("payrolldb"));
            sqlConnection.Open();
        }

        public virtual async IAsyncEnumerable<Deduction> GetDeductions()
        {
            var rdr = await sqlConnection.ExecuteReaderAsync("SELECT ID, EmployeeID, DeductionName, DeductionFrequency, Amount FROM dbo.deduction;");
            var rowParser = rdr.GetRowParser<Deduction>();

            while (await rdr.ReadAsync())
            {
                yield return rowParser(rdr);
            }
        }

        public virtual async IAsyncEnumerable<Deduction> GetDeductionsByEmployeeID(int employeeID)
        {
            string sql = "SELECT ID, EmployeeID, DeductionName, DeductionFrequency, Amount, effective_date " +
                "FROM dbo.deduction" +
                " WHERE EmployeeID = @empid;";

            var rdr = await sqlConnection.ExecuteReaderAsync(sql, new { empid = employeeID });
            var rowParser = rdr.GetRowParser<Deduction>();

            while (await rdr.ReadAsync())
            {
                yield return rowParser(rdr);
            }
        }

        public virtual async IAsyncEnumerable<Payee> GetPayees()
        {
            string sql = "SELECT ID, FirstName, LastName, DateOfHire, Exempt, PayRate, StateOfResidence FROM dbo.payee";
            var rdr = await sqlConnection.ExecuteReaderAsync(sql);
            var rowParser = rdr.GetRowParser<Payee>();

            while (await rdr.ReadAsync())
            {
                yield return rowParser(rdr);
            }
        }

        public virtual async Task<TaxTable> GetTaxTables(string stateCode)
        {
            string sql = "SELECT ID, State, StartRange, EndRange, TaxRate " +
                "FROM dbo.tax_table " +
                "WHERE State = @statecd;";

            var rdr = await sqlConnection.ExecuteReaderAsync(sql, new { statecd = stateCode });
            var rowParser = rdr.GetRowParser<TaxRange>();

            TaxTable taxTable = new TaxTable { State = stateCode, TaxRanges = new List<TaxRange>() };

            while (await rdr.ReadAsync())
            {
                var ranges = rowParser(rdr);
                taxTable.TaxRanges.Add(new TaxRange { StartRange = ranges.StartRange, EndRange = ranges.EndRange, TaxRate = ranges.TaxRate });
            }

            return taxTable;
        }

        public virtual async Task<TimeCard> GetTimecardByEmployeeID(int employeeID, DateTime payrollDate)
        {
            string sql = "SELECT ID, EmployeeID, DateOfWork, HoursWorked " +
                "FROM dbo.timecard " +
                "WHERE EmployeeID = @payeeId AND DateOfWork > @payPeriodStartDt";

            var timecards = await sqlConnection.QueryAsync<TimeEntry>(sql, new { payeeId = employeeID, payPeriodStartDt = payrollDate.AddDays(-5) });

            return new TimeCard { EmployeeID = employeeID, TimeEntries = timecards.ToList() };
        }

        public virtual IAsyncEnumerable<TimeCard> GetTimeCards()
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                sqlConnection.Dispose();
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~PayrollRepository()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public IAsyncEnumerable<TaxTable> GetTaxTables()
        {
            throw new NotImplementedException();
        }
    }
}
