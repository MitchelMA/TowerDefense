using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace PathFinding
{
    public class PathTraverser : MonoBehaviour
    {
        [SerializeField] private BasePathNode targetNode;
        [SerializeField] private float minDistance = 0.4f;
        [SerializeField] private float maxMoveDelta = 4f;
        [SerializeField] private float maxDegreesDelta = 720f;
        [SerializeField] private float rotationOffset = 0f;
        [SerializeField] private bool backwards;

        public Stack<BasePathNode> PathHistory { get; } = new Stack<BasePathNode>();

        public bool Backwards => backwards;

        public BasePathNode TargetNode
        {
            get => targetNode;
            set => targetNode = value;
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if(CheckArrive(targetNode))
                targetNode.CallBack(this);

            Vector3 curTargetPos = targetNode.gameObject.transform.position;
            Vector3 position = transform.position;
            position = Vector3.MoveTowards(position, curTargetPos, maxMoveDelta * Time.deltaTime);
            transform.position = position;
            
            RotateTowards(transform, targetNode.transform, maxDegreesDelta * Time.deltaTime);
        }

        private void RotateTowards(Transform from, Transform to, float maxDeltaDegrees)
        {
            // https://stackoverflow.com/questions/72502263/how-to-rotate-2d-sprite-towards-moving-direction
            Vector3 relativeTargetPos = to.position - from.position;
            
            float angle = Vector2.SignedAngle(Vector2.right, relativeTargetPos) - rotationOffset;
            Vector3 targetRotation = new Vector3(0, 0, angle);
            Quaternion lookTo = Quaternion.Euler(targetRotation);

            from.rotation = Quaternion.RotateTowards(from.rotation, lookTo, maxDeltaDegrees);
        }

        private bool CheckArrive(BasePathNode node) =>
            Vector3.Distance(transform.position, node.gameObject.transform.position) <= minDistance;
        
        public void GoBackwards()
        {
            backwards = true;
        }

        public void GoForwards()
        {
            backwards = false;
        }
    }
}
