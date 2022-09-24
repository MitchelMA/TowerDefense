using Health;
using Monsters;
using UnityEngine;

namespace PathFinding
{
    public class EndPathNode : BasePathNode
    {
        private HealthController _controller;

        protected override void Start()
        {
            base.Start();
            _controller = GameObject.FindWithTag("HealthController").GetComponent<HealthController>();
        }
        
        public override void CallBack(PathTraverser traverser)
        {
            // get the monster-data from the traverser
            BaseMonster monsterData = traverser.GetComponent<BaseMonster>();
            _controller.Deplete(monsterData.baseStats.damage);
            Destroy(traverser.gameObject);
        }
    }
}
