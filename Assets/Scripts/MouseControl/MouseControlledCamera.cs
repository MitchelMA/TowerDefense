using UnityEditor;
using UnityEngine;

namespace MouseControl
{
    [RequireComponent(typeof(Camera))]
    public class MouseControlledCamera : MonoBehaviour
    {
        [SerializeField] private Vector4 bounds = new Vector4(10, 10, -10, -10);
        [SerializeField] private Vector2 dragSpeed = new Vector2(50, 50);

        private bool _followsMouse = false;

        private Vector3 _moveStartWorldPos;
        private Vector3 _curWorldPos;
        private Vector3 _moveEndWorldPos;

        private Vector3 _cameraStartPos;
        private Vector3 _cameraEndPos;

        private Camera _camera;
        // Start is called before the first frame update
        private void Start()
        {
            if (!TryGetComponent(out _camera))
            {
                Debug.LogError($"There wasn't a component of type {typeof(Camera)} to be found when explicitly required", this);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_followsMouse)
                return;
            
            FollowMouse();
        }

        private void FollowMouse()
        {
            _curWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float xNew = InXBoundValue(_cameraStartPos.x - (_curWorldPos.x - _moveStartWorldPos.x));
            float yNew = InYBoundValue(_cameraStartPos.y - (_curWorldPos.y - _moveStartWorldPos.y));

            transform.position = new Vector3(xNew, yNew, transform.position.z);
        }

        public void StartFollowMouse(Vector3 from)
        {
            _cameraStartPos = transform.position;
            _moveStartWorldPos = _camera.ScreenToWorldPoint(from) - transform.position;
            _followsMouse = true;
        }

        public void StopFollow(Vector3 to)
        {
            _cameraEndPos = transform.position;
            // _moveEndWorldPos = new Vector3(to.x, to.y, to.z);
            _moveEndWorldPos = _camera.ScreenToWorldPoint(to) - transform.position;
            _followsMouse = false;
        }

        private float InXBoundValue(float xValue)
        {
            if (xValue > bounds.y || xValue < bounds.w)
                return transform.position.x;

            return xValue;
        }

        private float InYBoundValue(float yValue)
        {
            if (yValue > bounds.x || yValue < bounds.z)
                return transform.position.y;

            return yValue;
        }
    }
}
