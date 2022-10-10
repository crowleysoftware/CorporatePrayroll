namespace CorporatePrayroll.Tests
{
    [TestClass]
    public class RepoTests
    {
        [TestMethod]
        public async Task TimeCardTest()
        {
            PrayrollRepository repo = new PrayrollRepository();
            TimeCard tc = await repo.GetTimecardByEmployeeID(382, DateTime.Now);
            Assert.IsNotNull(tc);
        }

        [TestMethod]
        public void PrayrollTest()
        {
            PrayrollCalculator calculator = new PrayrollCalculator();
            var paychecks = calculator.CalculatePayroll(DateTime.Now);

            Assert.IsNotNull(paychecks);
        }
    }
}