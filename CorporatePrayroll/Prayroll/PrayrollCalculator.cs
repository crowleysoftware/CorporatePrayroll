
namespace CorporatePrayroll
{
    public class PrayrollCalculator
    {
        public async Task<List<Paycheck>> CalculatePayroll(DateTime payrollDate)
        {
            IPayrollRepository prayrollRepository = new PrayrollRepository();
            List<Paycheck> paychecks = new List<Paycheck>();

            //get all employees
            var payees = prayrollRepository.GetPayees();

            //get timecards for each emp
            await foreach (var payee in payees)
            {
                TimeCard timeCard = await prayrollRepository.GetTimecardByEmployeeID(payee.ID, payrollDate);

                var grossPay = timeCard.TimeEntries.Sum(timeEntry => timeEntry.HoursWorked) * payee.PayRate;

                TaxTable tbl = await prayrollRepository.GetTaxTables( payee.StateOfResidence);

                decimal grossTaxAmt = GetTaxRate(grossPay, tbl);

                var deductions = prayrollRepository.GetDeductionsByEmployeeID(payee.ID);

                decimal grossDeductions = 0m;

                await foreach (var d in deductions)
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

                var netPay = grossPay - grossTaxAmt - grossDeductions;

                Paycheck paycheck = new Paycheck();
                paycheck.TotalDeductions = grossDeductions;
                paycheck.IssueDate = payrollDate.Date;
                paycheck.EmployeeID = payee.ID;
                paycheck.TotalHours = timeCard.TimeEntries.Sum(timeEntry => timeEntry.HoursWorked);
                paycheck.TotalTaxes = grossTaxAmt;
                paycheck.NetPay = netPay;
                paychecks.Add(paycheck);
            }

            return paychecks;
        }

        private decimal GetTaxRate(decimal grossPay, TaxTable taxTable)
        {
            foreach (var item in taxTable.TaxRanges)
            {
                if (grossPay >= item.StartRange && grossPay <= item.EndRange)
                {
                    return item.TaxRate;
                }
            }

            throw new InvalidOperationException($"No tax rate identified for {grossPay} in {taxTable.State}");
        }
    }
}
