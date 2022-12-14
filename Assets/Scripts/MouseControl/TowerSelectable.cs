using Towers;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MouseControl
{
    public class TowerSelectable : BaseSelectable
    {
        [SerializeField] private TowerNode towerNode;
        [SerializeField] private TowerStatsUI uiStats;
        [SerializeField] private TowerBuyUI uiBuy;
        [SerializeField] private GameObject radiusIndicatorPrefab;

        private GameObject _currentRadiusIndicator;
        
        public override void Select()
        {
            if (towerNode.HasTower)
            {
                uiStats.gameObject.SetActive(true);
                uiBuy.gameObject.SetActive(false);
                // show the radius of the tower
                _currentRadiusIndicator = Instantiate(radiusIndicatorPrefab, transform);
                towerNode.CurrentTower.RadiusIndicator = _currentRadiusIndicator;
                // towerNode.CurrentTower.UpdateStatsDisplay();
                // towerNode.CurrentTower.UpdateStatsBtns();
            }
            else
            {
                // code for ui for buying tower
                uiBuy.gameObject.SetActive(true);
                uiStats.gameObject.SetActive(false);
                SetupBuyUi();
            }
            
            base.Select();
        }

        public override void Deselect()
        {
            uiStats.gameObject.SetActive(false);
            uiBuy.gameObject.SetActive(false);
            if(_currentRadiusIndicator)
                Destroy(_currentRadiusIndicator);
            
            base.Deselect();
        }

        private void SetupBuyUi()
        {
            uiBuy.CloseBtn.onClick.RemoveAllListeners();
            uiBuy.CloseBtn.onClick.AddListener(Deselect);
            
            uiBuy.BuyButtons.fire.Button.onClick.RemoveAllListeners();
            uiBuy.BuyButtons.fire.Button.onClick.AddListener(delegate
            {
                uiBuy.BuyButtons.fire.BuyTower(towerNode, BaseTower.TowerType.Fire);
                Deselect();
                Select();
            });
            
            uiBuy.BuyButtons.ice.Button.onClick.RemoveAllListeners();
            uiBuy.BuyButtons.ice.Button.onClick.AddListener(delegate
            {
                uiBuy.BuyButtons.ice.BuyTower(towerNode, BaseTower.TowerType.Ice);
                Select();
            });
            
            uiBuy.BuyButtons.poison.Button.onClick.RemoveAllListeners();
            uiBuy.BuyButtons.poison.Button.onClick.AddListener(delegate
            {
                uiBuy.BuyButtons.poison.BuyTower(towerNode, BaseTower.TowerType.Poison);
                Select();
            });
        }
    }
}