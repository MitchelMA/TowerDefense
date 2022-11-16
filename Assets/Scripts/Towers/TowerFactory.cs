using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Util;

namespace Towers
{
    public class TowerFactory : GenericSingleton<TowerFactory>
    {
        [SerializeField] private SerializableDictionary<GameObject> towers;

        public bool CreateTower(BaseTower.TowerType type, out GameObject tower)
        {
            foreach (var (key, value) in towers)
            {
                if(value == null
                   || !Enum.TryParse(key, out BaseTower.TowerType parseType)
                   || parseType != type)
                    continue;
                
                tower = value;
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