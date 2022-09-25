using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Towers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Currency
{
    public class CurrencyController : MonoBehaviour
    {
        [Serializable]
        private struct TowerPriceCombination
        {
            public ulong price;
            public BaseTower.TowerType type;
        }
        
        [SerializeField] private ulong amount = 500;
        [SerializeField] private TowerPriceCombination[] prices = new TowerPriceCombination[3];

        
        private readonly Mutex _amountMutex = new Mutex();

        private ulong[] _towerPrices;
        private BaseTower.TowerType[] _towerTypes;

        public List<ulong> TowerPrices => new List<ulong>(_towerPrices);
        public List<BaseTower.TowerType> TowerTypes => new List<BaseTower.TowerType>(_towerTypes);
        
        public event EventHandler<ulong> MoneyChanged;
        public ulong Amount => amount;
        private void Start()
        {
            // set of the arrays
            _towerPrices = new ulong[prices.Length];
            _towerTypes = new BaseTower.TowerType[prices.Length];
            for (int i = 0; i < prices.Length; i++)
            {
                _towerPrices[i] = prices[i].price;
                _towerTypes[i] = prices[i].type;
            }
            // call the event
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

        public ulong GetPriceOfType(BaseTower.TowerType type)
        {
            int idx = Array.IndexOf(_towerTypes, type);
            if (idx == -1)
                return ulong.MaxValue;
            return _towerPrices[idx];
        }
    }
}
