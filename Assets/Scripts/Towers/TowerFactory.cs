using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Towers
{
    public class TowerFactory : MonoBehaviour
    {
        [SerializeField] private BaseTower.TowerType[] towerTypes = new BaseTower.TowerType[3];
        [SerializeField] private GameObject[] baseTowers = new GameObject[3];

        public bool CreateTower(BaseTower.TowerType type, out GameObject tower)
        {
            if (towerTypes.Length != baseTowers.Length)
            {
                Debug.LogError($"The type array and the object array aren't of the same length:\n_towertypes: {towerTypes.Length}, _baseTowers: {baseTowers.Length}", this);
                tower = default;
                return false;
            }
            int index = Array.IndexOf(towerTypes, type);
            if (index == -1)
            {
                Debug.LogError($"TowerType of Type {type} could not be found", this);
                tower = default;
                return false;
            }

            tower = baseTowers[index];
            return true;
        }

        public bool CreateTower(int type, out GameObject tower)
        {
            return CreateTower((BaseTower.TowerType) type, out tower);
        }
    }
}