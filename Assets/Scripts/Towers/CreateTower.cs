using System;
using JetBrains.Annotations;
using Towers.Fire;
using UnityEngine;

namespace Towers
{
    public class CreateTower : MonoBehaviour
    {

        [SerializeField]
        private GameObject factoryGameObj;
        [SerializeField]
        private GameObject towerParent;

        [CanBeNull] private TowerFactory _towerFactory = null;

        public void Start()
        {
            factoryGameObj.TryGetComponent(out _towerFactory);
        }

        public void InstantiateTower(int type)
        {
            if (_towerFactory == null)
                return;

            if (!_towerFactory.CreateTower(type, out var tower))
            {
                return;
            }

            var instantiatedTower = Instantiate(tower, towerParent.transform);
            BaseTower instantiatedTowerBase;
            if (instantiatedTower.TryGetComponent(out instantiatedTowerBase))
            {
                instantiatedTowerBase.Type = (TowerType) type;
            }
            
        }
    }
}
