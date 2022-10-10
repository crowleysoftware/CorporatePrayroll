
namespace CorporatePrayroll.Tests.integration
{
    [TestClass]
    public class PrayrollCalculationTests
    {
        [TestMethod]
        [TestCategory("integration")]
        public async Task CalculatePrayroll()
        {
            var calculator = new PrayrollCalculator();
            List<Paycheck> checks = await calculator.CalculatePayroll(new DateTime(2022, 10, 23));
            var emp382 = checks.Single(c => c.EmployeeID == 382);
            Assert.AreEqual(694.6929, emp382.NetPay);
        }
    }
}
