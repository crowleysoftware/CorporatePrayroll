
namespace CorporatePrayroll.Services
{
    public class TaxService : ITaxService
    {
        private readonly PayrollRepository payrollRepository;

        public TaxService(PayrollRepository payrollRepository)
        {
            this.payrollRepository = payrollRepository;
        }

        public async Task<decimal> GetGrossTaxAmt(decimal grossPay, string stateCd)
        {
            var tbl = await payrollRepository.GetTaxTables(stateCd);

            foreach (var range in tbl.TaxRanges)
            {
                if (range.StartRange >= grossPay && range.EndRange <= grossPay)
                {
                    return grossPay * range.TaxRate;
                }
            }

            throw new InvalidOperationException("No appropriate tax range found");
        }
    }
}
