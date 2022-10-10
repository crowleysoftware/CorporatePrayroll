namespace CorporatePrayroll
{
    public class Deduction
    {
        public int EmployeeID { get; set; }
        public string DeductionName { get; set; }
        public Decimal Amount { get; set; }
        public DeductionFrequency DeductionFrequency { get; set; }
        public DateTime effective_date { get; set; }
    }

    public enum DeductionFrequency
    {
        None, PerPeriod, Yearly
    }
}
