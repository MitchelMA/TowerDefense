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

        public Button Button { get; private set; }
        public Text Text { get; private set; }

        private void Awake()
        {
            var controller = CurrencyController.Instance;
            Button = GetComponentInChildren<Button>();
            Text = transform.GetChild(1).GetComponent<Text>();
            buyValue = controller.GetPriceOfType(buyType);
            Text.text = $"{buyValue} $";
            controller.MoneyChanged += CheckAvailable;
            CheckAvailable(controller, controller.Amount);
        }

        // Start is called before the first frame update
        private void Start()
        {
            
        }

        private void OnEnable()
        {
            var controller = CurrencyController.Instance;
            // simulate the event
            if(controller)
                CheckAvailable(controller, controller.Amount);
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
            if (!CurrencyController.Instance.Deplete(buyValue))
                return;
            if(!node.PlaceTower(type))
                CurrencyController.Instance.Add(buyValue);
        }
    }
}
