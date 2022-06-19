using Microsoft.EntityFrameworkCore;

namespace Carting.DL
{
    [Keyless]
    public class Money
    {
        public Money() { }
        public Money(double amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public double Amount { get; set; }
        public Currency Currency { get; set; }
    }

    public enum Currency
    {
        USD = 1,
        RUB = 2,
        KZT = 3,
        EUR = 4
    }
}
