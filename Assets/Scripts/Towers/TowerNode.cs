using MouseControl;
using UnityEngine;

namespace Towers
{
    [RequireComponent(typeof(TowerSelectable))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class TowerNode : MonoBehaviour
    {
        private GameObject _currentTower;
        private TowerFactory _factory;

        public GameObject CurrentTower => _currentTower;

        public bool HasTower => _currentTower;
        // Start is called before the first frame update
        private void Start()
        {
            if (GameObject.FindWithTag("TowerFactory")?.TryGetComponent(out _factory) == false)
            {
                Debug.LogError("There wasn't a complete TowerFactory in this scene", this);
            }
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

            _currentTower = Instantiate(tower, transform);
        }

        public void PlaceTower(int type)
        {
            PlaceTower((BaseTower.TowerType) type);
        }
    }
}
