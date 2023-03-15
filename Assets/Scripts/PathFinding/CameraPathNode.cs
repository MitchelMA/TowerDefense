using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PathFinding
{
    public class CameraPathNode : BasePathNode
    {
        [SerializeField] private BasePathNode nextNode;
        public float size;
        public override void CallBack(PathTraverser traverser)
        {
            if (traverser.Backwards)
            {
                HandleBackwards(traverser);
                return;
            }
            
            traverser.PathHistory.Push(this);
            traverser.TargetNode = nextNode;
        }
    }
}