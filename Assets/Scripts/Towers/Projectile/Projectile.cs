using System;
using Monsters;
using UnityEngine;

namespace Towers.Projectile
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private string enemyTag;
        public Vector3 UDir { get; private set; }
        public float Speed { get; private set; }
        public int Damage { get; private set; }
        public Effect Effect { get; private set; }

        // life-time in seconds;
        private float _lifeTime = 10;
        
        // Start is called before the first frame update
        private void Start()
        {
        
        }

        // Update is called once per frame
        private void Update()
        {
            if (_lifeTime <= 0)
            {
                Destroy(gameObject);
                return;
            }
            
            transform.Translate(UDir * Speed * Time.deltaTime);
            _lifeTime -= Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            // if it hit a monster, even though the life-time went below zero
            if (_lifeTime <= 0)
                return;
            
            if (!other.tag.Equals(enemyTag))
                return;
            
            if (!other.TryGetComponent(out BaseMonster monster))
                return;
            
            monster.GainDamage(Damage);
            Effect.WearOn(monster);
            Destroy(gameObject);
        }

        public void Setup(Vector3 uDir, float speed, int damage, Effect effect)
        {
            UDir = uDir.normalized;
            Speed = speed;
            Damage = damage;
            Effect = effect;
        }
    }
}
