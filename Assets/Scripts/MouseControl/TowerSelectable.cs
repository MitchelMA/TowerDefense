using Towers;
using UnityEngine;

namespace MouseControl
{
    public class TowerSelectable : BaseSelectable
    {
        [SerializeField] private TowerNode towerNode;
        public override void Select()
        {
            _selected = true;
            towerNode.PlaceTower(BaseTower.TowerType.Fire);
        }

        public override void Deselect()
        {
            _selected = false;
        }
    }
}