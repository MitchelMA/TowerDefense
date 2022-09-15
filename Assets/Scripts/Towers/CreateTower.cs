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

        private TowerFactory _towerFactory;

        public void Start()
        {
            if (!factoryGameObj.TryGetComponent(out _towerFactory))
            {
                Debug.LogError($"The Factory Game Object didn't contain a Component of Type {typeof(TowerFactory)}", this);
            }
        }

        public void InstantiateTower(int type)
        {
            if (!_towerFactory.CreateTower(type, out GameObject tower))
            {
                return;
            }

            Instantiate(tower, towerParent.transform);
        }
    }
}
