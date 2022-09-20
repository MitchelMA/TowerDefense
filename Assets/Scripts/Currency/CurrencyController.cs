using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Currency
{
    public class CurrencyController : MonoBehaviour
    {
        [SerializeField] private ulong amount = 500;
        [SerializeField] private Text currencyDisplay;

        private readonly Mutex _amountMutex = new Mutex();
        private void Start()
        {
            UpdateDisplay();
        }

        public bool Deplete(ulong decrease)
        {
            _amountMutex.WaitOne();
            if (amount - decrease <= 0)
            {
                _amountMutex.ReleaseMutex();
                return false;
            }
            
            amount -= decrease;
            UpdateDisplay();
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
            UpdateDisplay();
            _amountMutex.ReleaseMutex();
        }

        public void Add(int addition)
        {
            Add(Convert.ToUInt64(addition));
        }

        private void UpdateDisplay()
        {
            currencyDisplay.text = $"{amount:D6} $";
        }
    }
}
