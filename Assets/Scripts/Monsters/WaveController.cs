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
        private readonly Stack<BaseMonster.MonsterType> _typeStack = new Stack<BaseMonster.MonsterType>();
        private bool _finishedAllWaves = false;

        public int CurrentWaveTotal => _currentWaveTotalAmount;
        // standard set to false to false so the first wave doesn't automatically start when entered
        private bool _waveBusy = false;
        public bool FinishedAllWaves => _finishedAllWaves;
        
        // Start is called before the first frame update
        private void Start()
        {
            // setup the first wave
            SetupWave(0);
        }

        // Update is called once per frame
        private void Update()
        {
            if (_currentTimeout > 0)
            {
                _currentTimeout -= Time.deltaTime;
                return;
            }
        
            if (!_waveBusy)
                return;
        
            if (!GetNextType(out BaseMonster.MonsterType next))
                return;

            if (!SpawnMonster(next))
            {
                _currentTimeout = 0;
                Debug.LogError($"Couldn't spawn next monster, Monster was of type {next}", this);
                return;
            }
            
            WaveData curWave = waves[_currentWave];
            System.Random rnd = new System.Random();
            _currentTimeout = (float)(rnd.NextDouble() * (curWave.spawnTimeouts[1] - curWave.spawnTimeouts[0]) + curWave.spawnTimeouts[0]);
        }

        private bool SpawnMonster(BaseMonster.MonsterType type)
        {
            if (!factory.CreateMonster(type, out GameObject monster))
                return false;

            SpawnPoint chosenPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            var clone = Instantiate(monster, monstersParent);
            chosenPoint.SetupMonster(clone);
            return true;
        }

        private void SetupWave(int wave)
        {
            Debug.Log($"Setup of wave {wave}");
            WaveData curWave = waves[wave];
            int curEasyLeft = curWave.easyAmount;
            int curMediumLeft = curWave.mediumAmount;
            int curHardLeft = curWave.hardAmount;
            _currentWaveTotalAmount = curEasyLeft + curMediumLeft + curHardLeft;

            _typeStack.Clear();
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
                    default:
                        continue;
                }
                _typeStack.Push(chosenType);
                index++;
            }
        }

        private bool GetNextType(out BaseMonster.MonsterType type)
        {
            if (_typeStack.TryPop(out type)) return true;
            
            // when the stack was empty:
            // set the wave to not being busy
            _waveBusy = false;
            // reset the timeout
            _currentTimeout = 0;
            // prepare the next wave
            if (++_currentWave >= waves.Length)
            {
                WavesEnd();
                return false;
            }
            SetupWave(_currentWave);
            return false;
        }

        // Method is meant to be called by a button-press in the UI
        public void StartWave()
        {
            _waveBusy = true;
        }

        private void WavesEnd()
        {
            _waveBusy = false;
            _finishedAllWaves = true;
            Debug.Log("Finished all waves!!");
        }
    }
}
