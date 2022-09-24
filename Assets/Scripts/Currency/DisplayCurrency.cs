using System;
using UnityEngine;
using UnityEngine.UI;

namespace Currency
{
    public class DisplayCurrency : MonoBehaviour
    {
        private CurrencyController _controller;
        private Text _text;
        // Start is called before the first frame update

        private void Start()
        {
            _text = GetComponent<Text>();
            _controller = GameObject.FindWithTag("CurrencyController").GetComponent<CurrencyController>();
            _controller.MoneyChanged += UpdateValue;
            // get the value at start
            UpdateValue(_controller, _controller.Amount);
        }

        // Update is called once per frame
        private void Update()
        {
        
        }

        private void UpdateValue(object sender, ulong newValue)
        {
            _text.text = $"{newValue:D5} $";
        }
        
    }
}
