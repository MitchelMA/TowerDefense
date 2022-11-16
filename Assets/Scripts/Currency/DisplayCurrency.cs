using System;
using Health;
using UnityEngine;
using UnityEngine.UI;

namespace Currency
{
    public class DisplayCurrency : MonoBehaviour
    {
        private Text _text;
        
        // Start is called before the first frame update
        private void Start()
        {
            _text = GetComponent<Text>();
            CurrencyController.Instance.MoneyChanged += UpdateValue;
            // get the value at start
            UpdateValue(CurrencyController.Instance, CurrencyController.Instance.Amount);
        }

        private void UpdateValue(object sender, ulong newValue)
        {
            _text.text = $"{newValue:D5} $";
        }
        
    }
}
