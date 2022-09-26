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
        public BaseEffect Effect { get; private set; }
        public BaseTower Owner { get; private set; }

        // Update is called once per frame
        private void Update()
        {
            transform.Translate(UDir * Speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.tag.Equals(enemyTag))
                return;
            
            if (!other.TryGetComponent(out BaseMonster monster))
                return;
            
            monster.GainDamage(Damage, Owner);
            // `Effect` may be null (for the possibility of towers not having an effect on their projectiles)
            Effect?.WearOn(monster);
            Destroy(gameObject);
        }

        public void Setup(Vector3 uDir, float speed, int damage, BaseEffect effect, BaseTower owner)
        {
            UDir = uDir.normalized;
            Speed = speed;
            Damage = damage;
            Effect = effect;
            Owner = owner;
        }
    }
}
