using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Towers
{
    public class TowerFactory : MonoBehaviour
    {
        [SerializeField] private TowerType[] _towertypes = new TowerType[3];
        [SerializeField] private GameObject[] _baseTowers = new GameObject[3];

        private bool CreateTower(TowerType type, out GameObject tower)
        {
            int index = Array.IndexOf(_towertypes, type);
            if (index == -1)
            {
                tower = null;
                return false;
            }

            tower = _baseTowers[index];
            return true;
        }

        public bool CreateTower(int type, out GameObject tower)
        {
            return CreateTower((TowerType) type, out tower);
        }
    }
}