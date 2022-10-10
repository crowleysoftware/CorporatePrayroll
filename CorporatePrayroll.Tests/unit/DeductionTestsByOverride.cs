using CorporatePrayroll.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CorporatePrayroll.Tests.unit
{
    [TestClass]
    public class DeductionTestsByOverride
    {
        [TestMethod]
        public async Task CanGetCorrectDeductionWithFutureDatedDeduction()
        {
            //Arrange

            FakePayrollRepository fakePayrollRepository= new FakePayrollRepository();

            IDeductionService deductionService = new DeductionService(fakePayrollRepository);
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

            FakePayrollRepository fakePayrollRepository = new FakePayrollRepository();

            IDeductionService deductionService = new DeductionService(fakePayrollRepository);
            Payee payee = new Payee { ID = 189 };
            DateTime payrollDate = new DateTime(2023, 3, 15);

            //Act
            var deduction = await deductionService.GetPayeeDeductions(payee, payrollDate);

            //Assert
            Assert.AreEqual(2, deduction.Count);
            Assert.AreEqual(1300.00m, deduction.Single(d => d.DeductionName == "Med Flex Spending").Amount);
        }
    }

    class FakePayrollRepository : PayrollRepository
    {

#pragma warning disable CS1998
        public override async IAsyncEnumerable<Deduction> GetDeductionsByEmployeeID(int employeeID)
        {
            yield return new Deduction { EmployeeID = 189, DeductionName = "Med Flex Spending", DeductionFrequency = DeductionFrequency.Yearly, Amount = 1200.00m, effective_date = new DateTime(2022, 5, 23) };
            yield return new Deduction { EmployeeID = 189, DeductionName = "Med Flex Spending", DeductionFrequency = DeductionFrequency.Yearly, Amount = 1300.00m, effective_date = new DateTime(2023, 1, 1) };
            yield return new Deduction { EmployeeID = 189, DeductionName = "401K", DeductionFrequency = DeductionFrequency.PerPeriod, Amount = 88.00m, effective_date = new DateTime(2022, 5, 23) };
        }
#pragma warning restore CS1998

    }
}
