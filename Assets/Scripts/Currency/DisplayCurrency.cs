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
            if (GameObject.FindWithTag("CurrencyController")?.TryGetComponent(out _controller) is false)
            {
                Debug.LogError($"Could not get GameObject with component of type {typeof(CurrencyController)}");
                return;
            }

            if (!TryGetComponent(out _text))
            {
                Debug.LogError($"gameObject did not contain a component of type {typeof(Text)}");
                return;
            }

            _controller.MoneyChanged += UpdateValue;
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
