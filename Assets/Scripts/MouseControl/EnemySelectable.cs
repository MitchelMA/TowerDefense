using UnityEngine;

namespace MouseControl
{
    public class EnemySelectable : BaseSelectable
    {
        public override void Select()
        {
            Debug.Log("EnemySelectable was selected");
            _selected = true;
        }

        public override void Deselect()
        {
            Debug.Log("EnemySelectable was Deselected");
            _selected = false;
        }
    }
}