using System;
using JetBrains.Annotations;
using Towers.Fire;
using UnityEngine;

namespace Towers
{
    public class CreateTower : MonoBehaviour
    {

        [SerializeField]
        private TowerFactory towerFactory;
        [SerializeField]
        private Transform towerParent;

        public void InstantiateTower(int type)
        {
            if (!towerFactory.CreateTower(type, out GameObject tower))
            {
                return;
            }

            Instantiate(tower, towerParent);
        }
    }
}
