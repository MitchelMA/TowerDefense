using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monsters
{
    public class WaveController : MonoBehaviour
    {
        #region Structs

        [Serializable]
        public struct WaveData
        {
            public int easyAmount;
            public int mediumAmount;
            public int hardAmount;
            public Vector2 spawnTimeouts;
        }

        #endregion

        [SerializeField] private WaveData[] waves = new WaveData[1];
        [SerializeField] private SpawnPoint[] spawnPoints = new SpawnPoint[1];
        [SerializeField] private MonsterFactory factory;
        [SerializeField] private Transform monstersParent;

        private float _currentTimeout = 0;
        private int _currentWave = 0;
        private int _currentWaveTotalAmount;
        private Stack<BaseMonster.MonsterType> _typeStack = new Stack<BaseMonster.MonsterType>();

        public int CurrentWaveTotal => _currentWaveTotalAmount;
        public bool waveBusy = true;
        
        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            if (_currentTimeout > 0)
            {
                _currentTimeout -= Time.deltaTime;
                return;
            }

            if (!waveBusy)
                return;

            BaseMonster.MonsterType next = GetNextType();
            SpawnMonster(next);
            
            WaveData curWave = waves[_currentWave];
            System.Random rnd = new System.Random();
            _currentTimeout = (float)(rnd.NextDouble() * (curWave.spawnTimeouts[1] - curWave.spawnTimeouts[0]) + curWave.spawnTimeouts[0]);
        }

        private void SpawnMonster(BaseMonster.MonsterType type)
        {
            if (!factory.CreateTower(type, out GameObject monster))
                return;

            SpawnPoint chosenPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            var clone = Instantiate(monster, monstersParent);
            chosenPoint.SetupMonster(clone);
        }

        private void SetupWave()
        {
            WaveData curWave = waves[-_currentWave];
            int curEasyLeft = curWave.easyAmount;
            int curMediumLeft = curWave.mediumAmount;
            int curHardLeft = curWave.hardAmount;
            _currentWaveTotalAmount = curEasyLeft + curMediumLeft + curHardLeft;

            int index = 0;
            while (index < _currentWaveTotalAmount)
            {
                BaseMonster.MonsterType chosenType = (BaseMonster.MonsterType)UnityEngine.Random.Range(0, 3);
                switch (chosenType)
                {
                    case BaseMonster.MonsterType.Easy:
                    {
                        if (curEasyLeft <= 0)
                            continue;
                        
                        curEasyLeft--;
                        break;
                    }
                    case BaseMonster.MonsterType.Medium:
                    {
                        if (curMediumLeft <= 0)
                            continue;

                        curMediumLeft--;
                        break;
                    }
                    case BaseMonster.MonsterType.Hard:
                    {
                        if(curHardLeft <= 0)
                            continue;

                        curHardLeft--;
                        break;
                    }
                }
                _typeStack.Push(chosenType);
            }
        }

        private BaseMonster.MonsterType GetNextType()
        {
            throw new NotImplementedException();
        }
    }
}
