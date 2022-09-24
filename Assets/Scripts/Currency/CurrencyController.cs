using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Currency
{
    public class CurrencyController : MonoBehaviour
    {
        [SerializeField] private ulong amount = 500;
        
        private readonly Mutex _amountMutex = new Mutex();
        public event EventHandler<ulong> MoneyChanged;
        public ulong Amount => amount;
        private void Start()
        {
            MoneyChanged?.Invoke(this, amount);
        }

        public bool Deplete(ulong decrease)
        {
            _amountMutex.WaitOne();
            if (decrease > amount)
            {
                _amountMutex.ReleaseMutex();
                return false;
            }
            
            amount -= decrease;
            MoneyChanged?.Invoke(this, amount);
            _amountMutex.ReleaseMutex();
            return true;
        }
        
        public bool Deplete(int decrease)
        {
            return Deplete(Convert.ToUInt64(decrease));
        }

        public void Add(ulong addition)
        {
            _amountMutex.WaitOne();
            amount += addition;
            MoneyChanged?.Invoke(this, amount);
            _amountMutex.ReleaseMutex();
        }

        public void Add(int addition)
        {
            Add(Convert.ToUInt64(addition));
        }
    }
}
