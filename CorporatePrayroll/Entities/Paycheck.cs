namespace CorporatePrayroll
{
    public class Paycheck
    {
        public int ID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime IssueDate { get; set; }
        public decimal TotalHours { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalTaxes { get; set; }
        public decimal NetPay { get; set; }
    }
}
