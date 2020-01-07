using Order.Api.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceA.Repository
{
    public interface IBankRepository
    {
        bool AddMoney(decimal money);
    }
    public class BankRepository : IBankRepository
    {
        private static Bank Bank = new Bank { Id = 1, Money = 0 };

        public bool AddMoney(decimal money)
        {
            Bank.Money += money;
            return true;
        }
    }
}
