namespace CorporatePrayroll.Services
{
    public interface ITaxService
    {
        Task<decimal> GetGrossTaxAmt(decimal grossPay, string stateCd);
    }
}