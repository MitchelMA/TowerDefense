using MouseControl;
using UI;
using UnityEngine;

namespace Towers
{
    [RequireComponent(typeof(TowerSelectable))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class TowerNode : MonoBehaviour
    {
        private BaseTower _currentTower;
        private TowerFactory _factory;

        public BaseTower CurrentTower => _currentTower;
        
        public bool HasTower => _currentTower;

        private TowerSelectable _selectable;
        public TowerSelectable Selectable => _selectable;

        // Start is called before the first frame update
        private void Start()
        {
            if (GameObject.FindWithTag("TowerFactory")?.TryGetComponent(out _factory) == false)
            {
                Debug.LogError("There wasn't a complete TowerFactory in this scene", this);
            }

            _selectable = GetComponent<TowerSelectable>();
        }

        // Update is called once per frame
        private void Update()
        {
            
        }

        public void PlaceTower(BaseTower.TowerType type)
        {
            Debug.Log("PlaceTower was called");
            if (HasTower)
            {
                // Destroy(_currentTower);
                return;
            }
            
            if (!_factory.CreateTower(type, out GameObject tower))
            {
                return;
            }

            if (!tower.TryGetComponent(out BaseTower towerScript))
            {
                Debug.LogError($"Tower GameObject didn't contain a component of type {typeof(BaseTower)}");
                return;
            }

            _currentTower = Instantiate(towerScript, transform);
        }

        public void PlaceTower(int type)
        {
            PlaceTower((BaseTower.TowerType) type);
        }
    }
}
