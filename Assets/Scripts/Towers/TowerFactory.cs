using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Towers
{
    public class TowerFactory : MonoBehaviour
    {
        [Serializable]
        private struct FactoryEntry
        {
            public BaseTower.TowerType type;
            public GameObject tower;
        }

        [SerializeField] private FactoryEntry[] towers = new FactoryEntry[3];

        public bool CreateTower(BaseTower.TowerType type, out GameObject tower)
        {
            foreach (FactoryEntry entry in towers.AsSpan())
            {
                if (!entry.type.Equals(type) || entry.tower is null)
                    continue;
                
                tower = entry.tower;
                return true;
            }
            
            Debug.LogError($"TowerType of Type {type} could not be found", this);
            tower = default;
            return false;
        }

        public bool CreateTower(int type, out GameObject tower)
        {
            return CreateTower((BaseTower.TowerType) type, out tower);
        }
    }
}