namespace CorporatePrayroll
{
    public class Payee
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Exempt { get; set; }
        public decimal PayRate { get; set; }
        public string StateOfResidence { get; set; }
        public DateTime DateOfHire { get; set; }
    }
}
