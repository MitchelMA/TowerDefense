using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Health;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
            public float difficultyMultiplier;
        }

        [Serializable]
        private struct UIElements
        {
            public Button nextBtn;
            public Text waveCountText;
            public Text enemyLeftText;
        }
        
        #endregion

        [SerializeField] private WaveData[] waves = new WaveData[1];
        [SerializeField] private SpawnPoint[] spawnPoints = new SpawnPoint[1];
        [SerializeField] private MonsterFactory factory;
        [SerializeField] private Transform monstersParent;
        [SerializeField] private UIElements ui;

        private float _currentTimeout = 0;
        private int _currentWave = 0;
        private int _currentWaveTotalAmount;
        private readonly Stack<BaseMonster.MonsterType> _typeStack = new Stack<BaseMonster.MonsterType>();
        private bool _allWavesSpawned = false;
        private Mutex _leftMutex = new Mutex();
        private int _monstersLeft;
        private HealthController _healthController;

        public int CurrentWaveTotal => _currentWaveTotalAmount;
        // standard set to false to false so the first wave doesn't automatically start when entered
        private bool _waveBusy = false;
        public bool AllWavesSpawned => _allWavesSpawned;
        public int MonstersLeft => _monstersLeft;
        public bool PreviousWaveFinished => _monstersLeft <= 0;
        public int CurrentWave => _currentWave;
        
        public event EventHandler<int> AllWavesFinished;
        public event EventHandler<int> FinishedWave; 

        // Start is called before the first frame update
        private void Start()
        {
            _healthController = GameObject.FindWithTag("HealthController").GetComponent<HealthController>();
            // setup the first wave
            SetupWave(0);
            UpdateWaveCounter();

            AllWavesFinished += GameWon;
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

            if (!SpawnMonster(next, waves[_currentWave].difficultyMultiplier))
            {
                _currentTimeout = 0;
                Debug.LogError($"Couldn't spawn next monster, Monster was of type {next}", this);
                return;
            }
            
            WaveData curWave = waves[_currentWave];
            System.Random rnd = new System.Random();
            _currentTimeout = (float)(rnd.NextDouble() * (curWave.spawnTimeouts[1] - curWave.spawnTimeouts[0]) + curWave.spawnTimeouts[0]);
        }

        private bool SpawnMonster(BaseMonster.MonsterType type, float multiplier)
        {
            if (!factory.CreateMonster(type, out GameObject monster))
                return false;

            SpawnPoint chosenPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            var clone = Instantiate(monster, monstersParent);
            chosenPoint.SetupMonster(clone, multiplier);
            return true;
        }

        private void SetupWave(int wave)
        {
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
            return false;
        }

        // Method is meant to be called by a button-press in the UI
        public void StartWave()
        {
            if (!PreviousWaveFinished)
                return;
            _monstersLeft = _typeStack.Count;
            _waveBusy = true;
            ui.nextBtn.interactable = false;
            UpdateWaveCounter();
            UpdateEnemyLeftCounter();
        }

        private void StopWave()
        {
            if (!PreviousWaveFinished)
                return;
            // set the wave to not being busy
            _waveBusy = false;
            // reset the timeout
            _currentTimeout = 0;
            // prepare the next wave
            FinishedWave?.Invoke(this, _currentWave);
            if (++_currentWave >= waves.Length)
            {
                SpawnedAllWaves();
            }
        }

        public void DecreaseLeft()
        {
            // always in order
            _leftMutex.WaitOne();
            _monstersLeft--;
            _leftMutex.ReleaseMutex();
            UpdateEnemyLeftCounter();

            if (PreviousWaveFinished)
            {
                StopWave();
                if (_allWavesSpawned)
                {
                    if(_healthController.CurrentHealth > 0)
                        FinishedAllWaves();
                    
                    return;
                }
                SetupWave(_currentWave);
                ui.nextBtn.interactable = true;
            }
            
        }

        private void SpawnedAllWaves()
        {
            _waveBusy = false;
            _allWavesSpawned = true;
        }

        private void FinishedAllWaves()
        {
            Debug.Log("Finished all waves!");
            AllWavesFinished?.Invoke(this, _currentWave);
        }

        private void GameWon(object sender, int wave)
        {
            Debug.Log($"YOU WON THE GAME AFTER {wave} WAVES");
            SceneManager.LoadScene("GameWonScene");
        }

        private void UpdateWaveCounter()
        {
            if(ui.waveCountText)
                ui.waveCountText.text = $"Wave {(_currentWave+1):D2}";
        }

        private void UpdateEnemyLeftCounter()
        {
            if(ui.enemyLeftText)
                ui.enemyLeftText.text = $"{_monstersLeft} / {_currentWaveTotalAmount}";
        }
    }
}
