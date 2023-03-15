using MouseControl;
using UnityEngine;

namespace PathFinding
{
    [RequireComponent(typeof(MouseControlledCamera))]
    public class CameraTraverser : PathTraverser
    {
        private Vector3 _lastPos;
        private float _lastSize;
        private Camera _cam;
        private float _progression;

        private void Start()
        {
            _lastPos = transform.position;
            _cam = GetComponent<Camera>();
            _lastSize = _cam.orthographicSize;
        }

        protected override void Update()
        {
            if (CheckArrive(targetNode))
            {
                _lastPos = transform.position;
                _lastSize = _cam.orthographicSize;
                targetNode.CallBack(this);
            }

            _progression = GetProgression(targetNode);
            Towards(targetNode);
            SetSize(targetNode as CameraPathNode);
        }

        protected override void Towards(BasePathNode nextNode)
        {
            Vector3 position = transform.position;
            position = Vector3.Lerp(position,
                Vector3.MoveTowards(position, nextNode.transform.position, maxMoveDelta * Time.deltaTime),
                DampForm(_progression));
            
            transform.position = position;
        }

        private float GetProgression(BasePathNode nextNode)
        {
            float totalLen = Vector3.Distance(_lastPos, nextNode.transform.position);
            float progLen = Vector3.Distance(transform.position, _lastPos);

            return progLen / totalLen;
        }

        private void SetSize(CameraPathNode nextNode)
        {
            float nextNodeSize = 10;
            if (nextNode is not null)
                nextNodeSize = nextNode.size;

            float nextSize = Lerp(_lastSize, nextNodeSize, _progression);
            if (!float.IsNaN(nextSize))
                _cam.orthographicSize = nextSize;
        }

        private float Lerp(float from, float to, float t) =>
            from + (to - from) * t;

        /// <summary>
        /// parabola for dampening the camera movement. <br/>
        /// Fill it into Desmos to plot the parabola as: <br/>-3,9(x-½)² + 1
        /// </summary>
        /// <param name="x">The x input of the parabola</param>
        /// <returns>The y value of the parabola</returns>
        private float DampForm(float x) =>
            // dampening strength, lower is less dampening
            -3.9f * 
            // quadratic component
            Mathf.Pow(
                // x
                x 
                // x-offset
                - 0.5f,
                2)
            // y-offset
            + 1;
    }
}