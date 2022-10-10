
namespace CorporatePrayroll.Services
{
    public class DeductionService : IDeductionService
    {
        private readonly IPayrollRepository payrollRepository;

        public DeductionService(IPayrollRepository payrollRepository)
        {
            this.payrollRepository = payrollRepository;
        }


        /// <summary>
        /// The repository does the work of getting the data. This method has the logic
        /// that ultimately is responsible for selecting the correct deductions.
        /// </summary>
        /// <param name="payee"></param>
        /// <param name="payrollDate"></param>
        /// <returns></returns>
        public async Task<List<Deduction>> GetPayeeDeductions(Payee payee, DateTime payrollDate)
        {
            var deductions = await payrollRepository.GetDeductionsByEmployeeID(payee.ID).ToListAsync();
            List<Deduction> effectiveDeductions = new List<Deduction>();

            var grp = deductions.GroupBy(x => x.DeductionName);

            foreach (var deduction in grp)
            {
                var effectiveDeduction = deduction.OrderByDescending(d => d.effective_date).FirstOrDefault(d => d.effective_date <= payrollDate);

                if (effectiveDeduction != null)
                {
                    effectiveDeductions.Add(effectiveDeduction);
                }
            }

            return effectiveDeductions;
        }

        public async Task<decimal> GetGrossDeductions(Payee payee, DateTime payrollDate)
        {
            decimal grossDeductions = 0m;
            var deductions = await GetPayeeDeductions(payee, payrollDate);

            foreach (var d in deductions)
            {
                if (d.DeductionFrequency == DeductionFrequency.PerPeriod)
                {
                    grossDeductions += d.Amount;
                }
                else if (d.DeductionFrequency == DeductionFrequency.Yearly)
                {
                    grossDeductions += d.Amount / 52;

                }
            }
            return deductions.Sum(d => d.Amount);
        }
    }
}
