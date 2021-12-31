using System;
using System.Security.Cryptography;
using System.Text;

namespace PaymentSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Order newOrder = new Order(1234, 12000);
            System1 system1 = new System1("pay.system1.ru/order?amount=");
            Console.WriteLine(system1.GetPayingLink(newOrder));
            System2 system2 = new System2("order.system2.ru/pay?hash=");
            Console.WriteLine(system2.GetPayingLink(newOrder));
            System3 system3 = new System3("system3.com/pay?amount=");
            Console.WriteLine(system3.GetPayingLink(newOrder));
        }
    }

    public class Order
    {
        public readonly int Id;
        public readonly int Amount;

        public Order(int id, int amount) => (Id, Amount) = (id, amount);
    }

    public abstract class GeneralSystem : IPaymentSystem
    {
        private string _firstAddressPart;

        public GeneralSystem(string firstAddressPart)
        {
            _firstAddressPart = firstAddressPart;
        }

        public string GetPayingLink(Order order)
        {
            string link = _firstAddressPart + GetMiddleAddressPart(order) + Convert.ToHexString(GetHashData(order));
            return link;
        }

        protected abstract string GetMiddleAddressPart(Order order);

        protected abstract byte[] GetHashData(Order order);
    }

    public class System1 : GeneralSystem
    {
        public System1(string firstAddressPart) : base(firstAddressPart)
        {

        }

        protected override byte[] GetHashData(Order order)
        {
            return MD5.HashData(Encoding.UTF8.GetBytes(order.Id.ToString()));
        }

        protected override string GetMiddleAddressPart(Order order)
        {
            return order.Amount + "RUB&hash=";
        }
    }

    public class System2 : GeneralSystem
    {
        public System2(string firstAddressPart) : base(firstAddressPart)
        {

        }

        protected override byte[] GetHashData(Order order)
        {
            return MD5.HashData(Encoding.UTF8.GetBytes(order.Id.ToString() + order.Amount.ToString()));
        }

        protected override string GetMiddleAddressPart(Order order)
        {
            return "";
        }
    }

    public class System3 : GeneralSystem
    {
        public System3(string firstAddressPart) : base(firstAddressPart)
        {

        }

        protected override byte[] GetHashData(Order order)
        {
            Aes aes = Aes.Create();
            byte[] key = aes.Key;
            return SHA1.HashData(Encoding.UTF8.GetBytes(order.Amount.ToString() + order.Id.ToString() + Convert.ToHexString(key)));
        }

        protected override string GetMiddleAddressPart(Order order)
        {
            return order.Amount + "&curency=RUB&hash=";
        }
    }

    public interface IPaymentSystem
    {
        public string GetPayingLink(Order order);
    }
}
