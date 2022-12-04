
using CorporatePrayroll.Services;

namespace CorporatePrayroll
{
    public class PayrollCalculator
    {
        private readonly ITimecardService timecardService;
        private readonly IDeductionService deductionService;
        private readonly ITaxService taxService;
        private readonly IPayeeService payeeService;

        public PayrollCalculator(
            ITimecardService timecardService,
            IDeductionService deductionService, 
            ITaxService taxService, 
            IPayeeService payeeService)
        {
            this.timecardService = timecardService;
            this.deductionService = deductionService;
            this.taxService = taxService;
            this.payeeService = payeeService;
        }

        //Orchestrator method does no business logic, it just calls on all the 
        //business methods and brings all the data together into a list of paychecks.
        public async Task<List<Paycheck>> CalculatePayroll(DateTime payrollDate)
        {
            List<Paycheck> paychecks = new List<Paycheck>();

            //get all employees
            await foreach (var payee in payeeService.GetActivePayees())
            {
                var timecard = await timecardService.GetTimeCardByEmployeeID(payee, payrollDate);
                var totalDeductions = await deductionService.GetGrossDeductions(payee, payrollDate);
                var grossPay = GetGrossPay(payee, timecard, payrollDate);
                var taxes = await taxService.GetGrossTaxAmt(grossPay, payee.StateOfResidence);
                var netPay = CalculateNetPay(grossPay, taxes, totalDeductions);

                Paycheck payCheck = BuildPaycheck(payee, totalDeductions, timecard, netPay, taxes, payrollDate);

                paychecks.Add(payCheck);
            }

            return paychecks;
        }

        private Paycheck BuildPaycheck(Payee payee, decimal totalDeductions, TimeCard timeCard, decimal netPay, decimal grossTax, DateTime payrollDate)
        {
            Paycheck paycheck = new Paycheck();

            paycheck.EmployeeID = payee.ID;
            paycheck.TotalDeductions = totalDeductions;
            paycheck.NetPay = netPay;
            paycheck.TotalTaxes = grossTax;
            paycheck.TotalHours = timeCard.TimeEntries.Sum(te => te.HoursWorked);
            paycheck.IssueDate = payrollDate;

            return paycheck;
        }

        private decimal GetGrossPay(Payee payee, TimeCard timeCard, DateTime payrollDate)
        {
            //realistically this would have more business logic like checking for overtime
            //and holiday etc.
            var grossPay = timeCard.TimeEntries
                .Where(te => te.DateOfWork >= payrollDate.AddDays(-5) && te.DateOfWork <= payrollDate)
                .Sum(te => te.HoursWorked * payee.PayRate);

            return decimal.Round(grossPay, 2);
        }

        private decimal CalculateNetPay(decimal grossPay, decimal grossTax, decimal totalDeductions)
        {
            return decimal.Round(grossPay - grossTax - totalDeductions, 2);
        }
    }
}
