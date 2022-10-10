using CorporatePrayroll.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CorporatePrayroll.Tests.unit
{
    [TestClass]
    public class CalculatorTests
    {
        [TestMethod]
        public async Task CanCalculatePayrollWithEmptyDeductionList()
        {
            //Arrange
            DateTime monday = new DateTime(2022, 11, 1);
            DateTime tuesday = new DateTime(2022, 11, 2);
            DateTime wednesday = new DateTime(2022, 11, 3);
            DateTime thursday = new DateTime(2022, 11, 4);
            DateTime friday = new DateTime(2022, 11, 5);

            #region Mock Payees
            Payee payee_1 = new Payee { ID = 1, PayRate = 5.0m, StateOfResidence = "ME" };
            Payee payee_2 = new Payee { ID = 2, PayRate = 10.0m, StateOfResidence = "OH" };
            Payee payee_3 = new Payee { ID = 3, PayRate = 65.0m, StateOfResidence = "NY" };

            var mockPayeeSvc = new Mock<IPayeeService>();

            mockPayeeSvc.Setup(m => m.GetActivePayees())
                .Returns((new List<Payee> { payee_1, payee_2, payee_3 })
                .ToAsyncEnumerable());
            #endregion

            #region Mock Timecard Svc
            var mockTimeCardSvc = new Mock<ITimecardService>();
            mockTimeCardSvc.Setup(m => m.GetTimeCardByEmployeeID(payee_1, It.IsAny<DateTime>()))
                .ReturnsAsync((Payee p, DateTime d) => new TimeCard
                {
                    EmployeeID = p.ID,
                    TimeEntries = new List<TimeEntry>() {
                        new TimeEntry { DateOfWork = monday, HoursWorked = 10m },
                        new TimeEntry { DateOfWork = tuesday, HoursWorked = 8m },
                        new TimeEntry { DateOfWork = wednesday, HoursWorked = 8m },
                        new TimeEntry { DateOfWork = thursday, HoursWorked = 7m },
                        new TimeEntry { DateOfWork = friday, HoursWorked = 7.5m }
                      }
                });

            mockTimeCardSvc.Setup(m => m.GetTimeCardByEmployeeID(payee_2, It.IsAny<DateTime>()))
              .ReturnsAsync((Payee p, DateTime d) => new TimeCard
              {
                  EmployeeID = p.ID,
                  TimeEntries = new List<TimeEntry>() {
                        new TimeEntry { DateOfWork = monday, HoursWorked = 8m },
                        new TimeEntry { DateOfWork = tuesday, HoursWorked = 8m },
                        new TimeEntry { DateOfWork = wednesday, HoursWorked = 8m },
                        new TimeEntry { DateOfWork = thursday, HoursWorked = 8m },
                        new TimeEntry { DateOfWork = friday, HoursWorked = 8m }
                    }
              });

            mockTimeCardSvc.Setup(m => m.GetTimeCardByEmployeeID(payee_3, It.IsAny<DateTime>()))
              .ReturnsAsync((Payee p, DateTime d) => new TimeCard
              {
                  EmployeeID = p.ID,
                  TimeEntries = new List<TimeEntry>() {
                        new TimeEntry { DateOfWork = monday, HoursWorked = 5m },
                        new TimeEntry { DateOfWork = tuesday, HoursWorked = 8m },
                        new TimeEntry { DateOfWork = wednesday, HoursWorked = 1m },
                        new TimeEntry { DateOfWork = friday, HoursWorked = 3.5m }
                    }
              });
            #endregion

            #region Mock Deductions
            var mockDeductionSvc = new Mock<IDeductionService>();
            mockDeductionSvc.Setup(m => m.GetGrossDeductions(payee_1, It.IsAny<DateTime>())).ReturnsAsync(50m);
            mockDeductionSvc.Setup(m => m.GetGrossDeductions(payee_2, It.IsAny<DateTime>())).ReturnsAsync(0m);
            mockDeductionSvc.Setup(m => m.GetGrossDeductions(payee_3, It.IsAny<DateTime>())).ReturnsAsync(295m);
            #endregion

            #region Mock Taxes
            //Setup tax amounts
            var mockTaxSvc = new Mock<ITaxService>();

            mockTaxSvc.Setup(m => m.GetGrossTaxAmt(It.IsAny<decimal>(), payee_1.StateOfResidence)).ReturnsAsync(45m);
            mockTaxSvc.Setup(m => m.GetGrossTaxAmt(It.IsAny<decimal>(), payee_2.StateOfResidence)).ReturnsAsync(88m);
            mockTaxSvc.Setup(m => m.GetGrossTaxAmt(It.IsAny<decimal>(), payee_3.StateOfResidence)).ReturnsAsync(310m);
            #endregion

            //Act
            PayrollCalculator calculator = new PayrollCalculator(
                mockTimeCardSvc.Object, mockDeductionSvc.Object, mockTaxSvc.Object, mockPayeeSvc.Object);

            var checks = await calculator.CalculatePayroll(friday);

            //Assert
            Assert.AreEqual(3, checks.Count);

            var checkPayee_1 = checks.Single(c => c.EmployeeID == payee_1.ID);
            var checkPayee_2 = checks.Single(c => c.EmployeeID == payee_2.ID);
            var checkPayee_3 = checks.Single(c => c.EmployeeID == payee_3.ID);

            #region Assert Payee 1
            var totalHrsPayee_1 = 10m + 8m + 8m + 7m + 7.5m;
            var totalDeductionsPayee_1 = 20m + 30m;
            var totalTaxesPayee_1 = 45m;
            var netPayPayee1 = (totalHrsPayee_1 * payee_1.PayRate) - totalDeductionsPayee_1 - totalTaxesPayee_1;

            Assert.AreEqual(netPayPayee1, checkPayee_1.NetPay, $"Net pay assertion for Payee 1");
            Assert.AreEqual(totalHrsPayee_1, checkPayee_1.TotalHours, $"Total hours assertion Payee 1");
            Assert.AreEqual(friday.Date, checkPayee_1.IssueDate.Date, $"Issue date assertion Payee 1");
            Assert.AreEqual(totalDeductionsPayee_1, checkPayee_1.TotalDeductions, "Deduction assertion Payee 1");
            #endregion

            #region Assert Payee 2
            var totalHrsPayee_2 = 8m + 8m + 8m + 8m + 8m;
            var totalDeductionsPayee_2 = 0m;
            var totalTaxesPayee_2 = 88m;
            var netPayPayee2 = (totalHrsPayee_2 * payee_2.PayRate) - totalDeductionsPayee_2 - totalTaxesPayee_2;

            Assert.AreEqual(netPayPayee2, checkPayee_2.NetPay, $"Net pay assertion for Payee 2");
            Assert.AreEqual(totalHrsPayee_2, checkPayee_2.TotalHours, $"Total hours assertion Payee 2");
            Assert.AreEqual(friday.Date, checkPayee_2.IssueDate.Date, $"Issue date assertion Payee 2");
            Assert.AreEqual(totalDeductionsPayee_2, checkPayee_2.TotalDeductions, "Deduction assertion Payee 2");
            #endregion

            #region Assert Payee 3
            var totalHrsPayee_3 = 5m + 8m + 1m + 3.5m;
            var totalDeductionsPayee_3 = 295m;
            var totalTaxesPayee_3 = 310m;
            var netPayPayee3 = (totalHrsPayee_3 * payee_3.PayRate) - totalDeductionsPayee_3 - totalTaxesPayee_3;

            Assert.AreEqual(netPayPayee3, checkPayee_3.NetPay, $"Net pay assertion for Payee 3");
            Assert.AreEqual(totalHrsPayee_3, checkPayee_3.TotalHours, $"Total hours assertion Payee 3");
            Assert.AreEqual(friday.Date, checkPayee_3.IssueDate.Date, $"Issue date assertion Payee 3");
            Assert.AreEqual(totalDeductionsPayee_3, checkPayee_3.TotalDeductions, "Deduction assertion Payee 3");
            #endregion

        }
    }
}
