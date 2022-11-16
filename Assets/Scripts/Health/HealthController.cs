using System;
using System.Threading;
using Monsters;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace Health
{
    public class HealthController : GenericSingleton<HealthController>
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
        public event EventHandler GameOver;

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
                GameOver?.Invoke(this, null);
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

        private void OnGameOver(object sender, EventArgs e)
        {
            var instance = WaveController.Instance;
            Debug.Log($"YOU MADE IT TILL WAVE {instance.CurrentWave} WITH {instance.MonstersLeft} MONSTERS LEFT");
            SceneManager.LoadScene("GameOverScene");
        }
    }
}
