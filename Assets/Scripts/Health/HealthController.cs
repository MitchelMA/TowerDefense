using System;
using System.Threading;
using Monsters;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Health
{
    public class HealthController : MonoBehaviour
    {
        public struct HealthChangeEventArgs
        {
            public int PreviousAmount;
            public int NewAmount;
        }

        [SerializeField] private int startHealth;
        public int StartHealth => startHealth;
        private int _currentHealth;
        private readonly Mutex _healthMutex = new Mutex();

        public int CurrentHealth => _currentHealth;

        public event EventHandler<HealthChangeEventArgs> HealthChanged;
        public event EventHandler<WaveController> GameOver;

        private void Start()
        {
            _currentHealth = startHealth;
            HealthChanged?.Invoke(this, new HealthChangeEventArgs
            {
                NewAmount = _currentHealth,
                PreviousAmount = 0,
            });
            GameOver += OnGameOver;
        }
        
        public bool Deplete(int amount)
        {
            _healthMutex.WaitOne();
            int old = _currentHealth;
            _currentHealth -= amount;
            HealthChanged?.Invoke(this,new HealthChangeEventArgs
            {
                PreviousAmount = old,
                NewAmount = _currentHealth,
            });
            _healthMutex.ReleaseMutex();
            if (_currentHealth <= 0)
            {
                GameOver?.Invoke(this, GameObject.FindWithTag("WaveController").GetComponent<WaveController>());
                return false;
            }
            return true;
        }

        public void Increase(int amount)
        {
            _healthMutex.WaitOne();
            int old = _currentHealth;
            _currentHealth += amount;
            HealthChanged?.Invoke(this, new HealthChangeEventArgs
            {
                PreviousAmount = old,
                NewAmount = _currentHealth,
            });
            _healthMutex.ReleaseMutex();
        }

        private void OnGameOver(object sender, WaveController waveController)
        {
            Debug.Log($"YOU MADE IT TILL WAVE {waveController.CurrentWave} WITH {waveController.MonstersLeft} MONSTERS LEFT");
            SceneManager.LoadScene("GameOverScene");
        }
    }
}
