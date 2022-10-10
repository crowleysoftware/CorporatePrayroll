
using CorporatePrayroll.Services;

namespace CorporatePrayroll.Tests.unit
{
    //TODO: demo other implementations, like no deductions, deduction outside of date range, etc.

    [TestClass]
    public class DeductionTestsByReImplementation
    {
        [TestMethod]
        public async Task CanGetCorrectDeductionWithFutureDatedDeduction()
        {
            //Arrange
            IPayrollRepository payrollRepository = new PayrollRepositoryForDeductionTesting();
            IDeductionService deductionService = new DeductionService(payrollRepository);
            Payee payee = new Payee { ID = 189 };
            DateTime payrollDate = new DateTime(2022, 6, 1);

            //Act
            var deduction = await deductionService.GetPayeeDeductions(payee, payrollDate);

            //Assert
            Assert.AreEqual(2, deduction.Count);
            Assert.AreEqual(1200.00m, deduction.Single(d => d.DeductionName == "Med Flex Spending").Amount);
        }

        [TestMethod]
        public async Task CanGetCorrectDeductionWithPastDatedDeduction()
        {
            //Arrange
            IPayrollRepository payrollRepository = new PayrollRepositoryForDeductionTesting();
            IDeductionService deductionService = new DeductionService(payrollRepository);
            Payee payee = new Payee { ID = 189 };
            DateTime payrollDate = new DateTime(2023, 3, 15);

            //Act
            var deduction = await deductionService.GetPayeeDeductions(payee, payrollDate);

            //Assert
            Assert.AreEqual(2, deduction.Count);
            Assert.AreEqual(1300.00m, deduction.Single(d => d.DeductionName == "Med Flex Spending").Amount);
        }
    }

    class PayrollRepositoryForDeductionTesting : IPayrollRepository
    {

#pragma warning disable CS1998
        public IAsyncEnumerable<Deduction> GetDeductions()
        {
            throw new NotImplementedException();
        }

        public async IAsyncEnumerable<Deduction> GetDeductionsByEmployeeID(int employeeID)
        {
            yield return new Deduction
            {
                EmployeeID = 189,
                DeductionName = "Med Flex Spending",
                DeductionFrequency = DeductionFrequency.Yearly,
                Amount = 1300.00m,
                effective_date = new DateTime(2023, 1, 1)
            };

            yield return new Deduction
            {
                EmployeeID = 189,
                DeductionName = "Med Flex Spending",
                DeductionFrequency = DeductionFrequency.Yearly,
                Amount = 1200.00m,
                effective_date = new DateTime(2022, 5, 23)
            };

            yield return new Deduction
            {
                EmployeeID = 189,
                DeductionName = "401K",
                DeductionFrequency = DeductionFrequency.PerPeriod,
                Amount = 88.00m,
                effective_date = new DateTime(2022, 5, 23)
            };
        }
#pragma warning restore CS1998

        public IAsyncEnumerable<Payee> GetPayees()
        {
            throw new NotImplementedException();
        }

        public Task<TaxTable> GetTaxTables(string stateCode)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<TaxTable> GetTaxTables()
        {
            throw new NotImplementedException();
        }

        public Task<TimeCard> GetTimecardByEmployeeID(int employeeID, DateTime payrollDate)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<TimeCard> GetTimeCards()
        {
            throw new NotImplementedException();
        }
    }
}
