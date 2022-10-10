using Moq;

namespace CorporatePrayroll.Tests.unit
{
    public static class PayrollServiceMock
    {
        public static Mock<IPayrollRepository> GetDeductionsForEmplyee189 (this Mock<IPayrollRepository> mock, Exception ex = null)
        {
            if (ex != null)
            {
                mock.Setup(m => m.GetDeductionsByEmployeeID(It.IsAny<int>())).Throws(ex);
                return mock;
            }

            var mockData = new[] {
                new Deduction { EmployeeID = 189, DeductionName = "Med Flex Spending", DeductionFrequency = DeductionFrequency.Yearly, Amount = 1300.00m, effective_date = new DateTime(2023, 1, 1) },
                new Deduction { EmployeeID = 189, DeductionName = "Med Flex Spending", DeductionFrequency = DeductionFrequency.Yearly, Amount = 1200.00m, effective_date = new DateTime(2022, 5, 23) },
                new Deduction { EmployeeID = 189, DeductionName = "401K", DeductionFrequency = DeductionFrequency.PerPeriod, Amount = 88.00m, effective_date = new DateTime(2022, 5, 23) }
            };

            mock.Setup(m => m.GetDeductionsByEmployeeID(It.IsAny<int>()))
                    .Returns(mockData.ToAsyncEnumerable());
            return mock;
        }

        public static Mock<IPayrollRepository> GetDeductionsByEmployeeID(this Mock<IPayrollRepository> mock, Deduction[] deductionsToReturn, Exception ex = null)
        {
            if (ex != null)
            {
                mock.Setup(m => m.GetDeductionsByEmployeeID(It.IsAny<int>())).Throws(ex);
                return mock;
            }
            
            mock.Setup(m => m.GetDeductionsByEmployeeID(It.IsAny<int>()))
                    .Returns(deductionsToReturn.ToAsyncEnumerable());
            return mock;
        }
    }
}
