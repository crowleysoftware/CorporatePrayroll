using Microsoft.Extensions.Configuration;

namespace CorporatePrayroll.Tests.integration
{
    [TestClass]
    public class PayrollRepositoryTests
    {
        static IConfiguration _configuration;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true).Build();
        }

        [TestMethod]
        [TestCategory("integration")]
        public async Task CanGetAllDeductions()
        {
            //Arrange
            using var repo = new PayrollRepository(_configuration);

            //Act
            var allDeductions = repo.GetDeductions();
            var deductionCount = await allDeductions.CountAsync();

            //Assert
            Assert.IsNotNull(allDeductions);
            Assert.AreEqual(9, deductionCount, "There were zero deductions returned");
        }

        [TestMethod]
        [TestCategory("integration")]
        public async Task CanGetDeductionsForOneEmployee()
        {
            //Arrange
            using var repo = new PayrollRepository(_configuration);

            //Act
            var emp9901Deductions = repo.GetDeductionsByEmployeeID(9901);

            //Assert
            var count = await emp9901Deductions.CountAsync();
            Assert.AreEqual(3, count, $"There were not 3 deductions, there were {count}");
        }

        [TestMethod]
        [TestCategory("integration")]
        public async Task CanGetATaxTable()
        {
            //Arrange
            using var repo = new PayrollRepository(_configuration);

            //Act
            var taxtbls = await repo.GetTaxTables("ME");

            //Assert
            Assert.AreEqual("ME", taxtbls.State);
            Assert.AreEqual(4, taxtbls.TaxRanges.Count);
        }

        [TestMethod]
        [TestCategory("integration")]
        public async Task CanGetPayees()
        {
            //Arrange
            using var repo = new PayrollRepository(_configuration);

            //Act
            var payeesEnumerable = repo.GetPayees();

            //Assert
            Assert.AreEqual(6, await payeesEnumerable.CountAsync());
        }

        [TestMethod]
        [TestCategory("integration")]
        public async Task CanGetEmployeeTimeCards()
        {
            //Arrange
            using var repo = new PayrollRepository(_configuration);

            //Act
            var timeCards = await repo.GetTimecardByEmployeeID(382, new DateTime(2023, 1, 27));

            //Assert
            Assert.AreEqual(5, timeCards.TimeEntries.Count);
        }

        [TestMethod]
        [TestCategory("integration")]
        public async Task Emp9901s401kDedcutionAmountIs88Dollars()
        {
            //Arrange
            using var repo = new PayrollRepository(_configuration);

            //Act
            var emp9901Deduction = await repo.GetDeductionsByEmployeeID(9901).SingleAsync(d => d.DeductionName == "401K");

            //Assert
            Assert.AreEqual(88.0m, (emp9901Deduction.Amount));
        }
    }
}
