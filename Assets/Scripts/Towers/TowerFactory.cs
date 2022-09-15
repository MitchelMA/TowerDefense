using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Towers
{
    public class TowerFactory : MonoBehaviour
    {
        [SerializeField] private BaseTower.TowerType[] _towerTypes = new BaseTower.TowerType[3];
        [SerializeField] private GameObject[] _baseTowers = new GameObject[3];

        private bool CreateTower(BaseTower.TowerType type, out GameObject tower)
        {
            if (_towerTypes.Length != _baseTowers.Length)
            {
                Debug.LogError($"The type array and the object array aren't of the same length:\n_towertypes: {_towerTypes.Length}, _baseTowers: {_baseTowers.Length}", this);
                tower = null;
                return false;
            }
            int index = Array.IndexOf(_towerTypes, type);
            if (index == -1)
            {
                Debug.LogError($"TowerType of Type {type} could not be found", this);
                tower = null;
                return false;
            }

            tower = _baseTowers[index];
            return true;
        }

        public bool CreateTower(int type, out GameObject tower)
        {
            return CreateTower((BaseTower.TowerType) type, out tower);
        }
    }
}