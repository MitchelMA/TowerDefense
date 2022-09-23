using Towers;
using UI;
using UnityEngine;

namespace MouseControl
{
    public class TowerSelectable : BaseSelectable
    {
        [SerializeField] private TowerNode towerNode;
        [SerializeField] private TowerStatsUI uiStats;
        public override void Select()
        {
            _selected = true;
            if (towerNode.HasTower)
            {
                uiStats.gameObject.SetActive(true);
            }
            else
            {
                // code for ui for buying tower
            }
            towerNode.PlaceTower(BaseTower.TowerType.Fire);
        }

        public override void Deselect()
        {
            _selected = false;
            uiStats.gameObject.SetActive(false);
        }
    }
}