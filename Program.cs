using System;
using Wrox.ProCSharp;
using Wrox.ProCSharp.JupiterBank;
using Wrox.ProCSharp.VenusBank;

namespace Wrox.ProCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            IBankAccount venusAccount = new SaverAccount();
            ITransferBankAccount jupiterAccount = new CurrentAccount();
            venusAccount.PayIn(200);
            jupiterAccount.PayIn(500);
            jupiterAccount.TransferTo(venusAccount, 100);
            Console.WriteLine(venusAccount.ToString());
            Console.WriteLine(jupiterAccount.ToString());
            Console.ReadLine();

        }
    }
}

namespace Wrox.ProCSharp
{
    public interface IBankAccount
    {
        void PayIn(decimal amount);
        bool Widthdraw(decimal amount);
        decimal Balance { get; }
    }

    public interface ITransferBankAccount : IBankAccount
    {
        bool TransferTo(IBankAccount destination, decimal amount);
    }
}

namespace Wrox.ProCSharp.VenusBank
{
    class SaverAccount : IBankAccount
    {
        private decimal balance;
        public void PayIn(decimal amount)
        {
            balance += amount;
        }

        public bool Widthdraw(decimal amount)
        {
            if (balance >= amount)
            {
                balance -= amount;
                return true;
            }
            Console.WriteLine("Widthdrawl attempt failed.");
            return false;
        }

        public decimal Balance
        {
            get { return balance; }
        }

        public override string ToString()
        {
            return String.Format("Venus Bank Saver: Balance = {0, 6:C}", balance);
        }
    }
}

namespace Wrox.ProCSharp.JupiterBank
{
    public class CurrentAccount : ITransferBankAccount
    {
        private decimal balance;
        public void PayIn(decimal amount)
        {
            balance += amount;
        }

        public bool Widthdraw(decimal amount)
        {
            if (balance >= amount)
            {
                balance -= amount;
                return true;
            }
            Console.WriteLine("Widthdrawl attempt failed.");
            return false;
        }

        public decimal Balance
        {
            get { return balance; }
        }

        public bool TransferTo(IBankAccount destination, decimal amount)
        {
            bool result;
            result = Widthdraw(amount);
            if (result)
            {
                destination.PayIn(amount);
            }
            return result;
        }

        public override string ToString()
        {
            return String.Format("Jupiter Bank Current Account: Balance = {0, 6:C}", balance);
        }     
    }
}
