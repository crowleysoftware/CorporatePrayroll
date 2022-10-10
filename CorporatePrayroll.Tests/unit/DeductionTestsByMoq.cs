using CorporatePrayroll.Services;
using Moq;

namespace CorporatePrayroll.Tests.unit
{
    [TestClass]
    public class DeductionTestsByMoq
    {
        [TestMethod]
        public async Task CanGetCorrectDeductionWithFutureDatedDeduction()
        {
            //Arrange

            var mockData = new[] {
                new Deduction { EmployeeID = 189, DeductionName = "Med Flex Spending", DeductionFrequency = DeductionFrequency.Yearly, Amount = 1300.00m, effective_date = new DateTime(2023, 1, 1) },
                new Deduction { EmployeeID = 189, DeductionName = "Med Flex Spending", DeductionFrequency = DeductionFrequency.Yearly, Amount = 1200.00m, effective_date = new DateTime(2022, 5, 23) },
                new Deduction { EmployeeID = 189, DeductionName = "401K", DeductionFrequency = DeductionFrequency.PerPeriod, Amount = 88.00m, effective_date = new DateTime(2022, 5, 23) }
            };

            var mockRepo = new Mock<IPayrollRepository>();

            mockRepo.Setup(m => m.GetDeductionsByEmployeeID(It.IsAny<int>()))
                    .Returns(mockData.ToAsyncEnumerable());

            IDeductionService deductionService = new DeductionService(mockRepo.Object);

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
            var mockData = new[] {
                new Deduction { EmployeeID = 189, DeductionName = "Med Flex Spending", DeductionFrequency = DeductionFrequency.Yearly, Amount = 1300.00m, effective_date = new DateTime(2023, 1, 1) },
                new Deduction { EmployeeID = 189, DeductionName = "Med Flex Spending", DeductionFrequency = DeductionFrequency.Yearly, Amount = 1200.00m, effective_date = new DateTime(2022, 5, 23) },
                new Deduction { EmployeeID = 189, DeductionName = "401K", DeductionFrequency = DeductionFrequency.PerPeriod, Amount = 88.00m, effective_date = new DateTime(2022, 5, 23) }
            };

            var mockRepo = new Mock<IPayrollRepository>();

            mockRepo.Setup(m => m.GetDeductionsByEmployeeID(It.IsAny<int>()))
                    .Returns(mockData.ToAsyncEnumerable());

            IDeductionService deductionService = new DeductionService(mockRepo.Object);
            Payee payee = new Payee { ID = 189 };
            DateTime payrollDate = new DateTime(2023, 3, 15);

            //Act
            var deduction = await deductionService.GetPayeeDeductions(payee, payrollDate);

            //Assert
            Assert.AreEqual(2, deduction.Count);
            Assert.AreEqual(1300.00m, deduction.Single(d => d.DeductionName == "Med Flex Spending").Amount);
        }
    }
}
