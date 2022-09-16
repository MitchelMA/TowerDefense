using UnityEngine;
using UnityEngine.UI;

namespace MouseControl
{
    public class EnemySelectable : BaseSelectable
    {
        [SerializeField] private GameObject enemyUi;
        private Text _enemyText;

        protected override void Start()
        {
            if (!enemyUi.TryGetComponent(out _enemyText))
            {
                Debug.LogError($"UI element did not contain component of type {typeof(Text)}", this);
            }
        }

        protected override void Update()
        {
            if (!_selected)
                return;

            Vector3 screenpos = targetCamera.WorldToScreenPoint(transform.position);
            screenpos.y += 40;
            enemyUi.transform.position = screenpos;
        }
        
        public override void Select()
        {
            Debug.Log("EnemySelectable was selected");
            enemyUi.SetActive(true);
            _selected = true;
        }

        public override void Deselect()
        {
            Debug.Log("EnemySelectable was Deselected");
            enemyUi.SetActive(false);
            _selected = false;
        }
    }
}