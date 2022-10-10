using CorporatePrayroll.Services;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CorporatePrayroll.Tests.integration
{
    [TestClass]
    public class PayrollCalculationTests
    {
        static IConfiguration _configuration;
        static PayrollRepository payrollRepo;
        static IDeductionService deductionService;
        static ITaxService taxService;
        static IPayeeService payeeService;
        static ITimecardService timecardService;
        static PayrollCalculator payrollCalculator;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true).Build();

            payrollRepo = new PayrollRepository(_configuration);
            deductionService = new DeductionService(payrollRepo);
            taxService = new TaxService(payrollRepo);
            payeeService = new PayeeService(payrollRepo);
            timecardService = new TimecardService(payrollRepo);

            payrollCalculator = new PayrollCalculator(timecardService, deductionService, taxService, payeeService);

        }

        [TestMethod]
        [TestCategory("integration")]
        [TestCategory("fragile")]
        public async Task CanGetCorrectEffectiveDeduction()
        {
            #region demo
            //var sql = @"insert into dbo.deduction(EmployeeID, DeductionName, DeductionFrequency, Amount, effective_date)
            //    values(189, 'Test Deduction', 2, 123.00, '2022-02-01');";

            //using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("payrolldb")))
            //{
            //    await con.OpenAsync();
            //    await con.ExecuteAsync(sql);
            //}
            #endregion

            //Arrange
            IDeductionService deductionService = new DeductionService(payrollRepo);
            Payee payee = new Payee { ID = 189 };

            //Act
            var deduction = await deductionService.GetPayeeDeductions(payee, new DateTime(2022, 11, 15));

            //Assert
            Assert.AreEqual(2, deduction.Count);
            Assert.AreEqual(new DateTime(2022, 5, 23), deduction[0].effective_date);
            Assert.AreEqual(new DateTime(2022, 5, 23), deduction[1].effective_date);
        }


    }
}
