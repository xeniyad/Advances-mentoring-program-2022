using Microsoft.EntityFrameworkCore;
using System;

namespace Carting.DL
{
    [Keyless]
    public class Money
    {
        private const char Delimeter = ';'; 
        public Money() { }
        public Money(double amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public Money(string money)
        {
            var args = money.Split(Delimeter);
            try
            {
                Amount = Convert.ToDouble(args[0]);
                Currency = Enum.Parse<Currency>(args[1]);
            }
            catch (Exception)
            {
                Amount = 0;
                Currency = Currency.USD;
            }
        }

        public double Amount { get; set; }
        public Currency Currency { get; set; }

        public override string ToString()
        {
            return $"{Amount} {Currency}";
        }

        public string ToDbString()
        {
            return $"{Amount}{Delimeter}{Currency}";
        }
    }

    public enum Currency
    {
        USD = 1,
        RUB = 2,
        KZT = 3,
        EUR = 4
    }
}
