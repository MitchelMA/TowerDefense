using System;
using Towers;
using UnityEngine;
using UnityEngine.UI;

namespace Currency
{
    public class BuyBtn : MonoBehaviour
    {
        [SerializeField] private BaseTower.TowerType buyType;
        private ulong buyValue;
        private CurrencyController _controller;

        public Button Button { get; private set; }
        public Text Text { get; private set; }

        private void Awake()
        {
            Button = GetComponentInChildren<Button>();
            Text = transform.GetChild(1).GetComponent<Text>();
            _controller = GameObject.FindWithTag("CurrencyController").GetComponent<CurrencyController>();
            buyValue = _controller.GetPriceOfType(buyType);
            Text.text = $"{buyValue} $";
            _controller.MoneyChanged += CheckAvailable;
            CheckAvailable(_controller, _controller.Amount);
        }

        // Start is called before the first frame update
        private void Start()
        {
            
        }

        private void OnEnable()
        {
            // simulate the event
            if(_controller)
                CheckAvailable(_controller, _controller.Amount);
        }

        // Update is called once per frame
        private void Update()
        {
        
        }

        private void CheckAvailable(object sender, ulong newValue)
        {
            Button.interactable = newValue >= buyValue;
        }

        public void BuyTower(TowerNode node, BaseTower.TowerType type)
        {
            if (!_controller.Deplete(buyValue))
                return;
            if(!node.PlaceTower(type))
                _controller.Add(buyValue);
        }
    }
}
