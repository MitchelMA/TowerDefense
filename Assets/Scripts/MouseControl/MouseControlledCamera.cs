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

            _curWorldPos = _camera.ScreenToViewportPoint(Input.mousePosition - _moveStartWorldPos);
            Vector3 moveX = new Vector3(_curWorldPos.x * dragSpeed.x, 0, 0) * Time.deltaTime;
            Vector3 moveY = new Vector3(0, _curWorldPos.y * dragSpeed.y, 0) * Time.deltaTime;

            if(InXAxisBounds(moveX))
                transform.Translate(moveX);
            
            if(InYAxisBounds(moveY))
                transform.Translate(moveY);

        }

        public void FollowMouse(Vector3 from)
        {
            _moveStartWorldPos = new Vector3(from.x, from.y, from.z);
            _followsMouse = true;
        }

        public void StopFollow(Vector3 to)
        {
            _moveEndWorldPos = new Vector3(to.x, to.y, to.z);
            _followsMouse = false;
        }

        private bool InXAxisBounds(Vector3 addition)
        {
            Vector3 pos = transform.position + addition;
            return pos.x > bounds.w && pos.x < bounds.y;
        }

        private bool InYAxisBounds(Vector3 addition)
        {
            Vector3 pos = transform.position + addition;
            return pos.y > bounds.z && pos.y < bounds.x;
        }
    }
}
