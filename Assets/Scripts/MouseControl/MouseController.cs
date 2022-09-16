using System;
using System.Linq;
using UnityEngine;

namespace MouseControl
{
    public class MouseController : MonoBehaviour
    {
        #region Enums
        public enum MouseState
        {
            Idle,
            Click,
            Move,
        };

        private enum MouseBtn
        {
            Left,
            Right,
            Middle,
        };

        #endregion

        [SerializeField] private Camera targetCamera;
        [SerializeField] private float sensitivity = 2f;
        [SerializeField] private MouseBtn mouseBtn = MouseBtn.Left;
        [SerializeField] private string[] selectableTags = new string[1];

        private MouseState _mouseState = MouseState.Idle;

        private MouseControlledCamera _controlledCamera;
        
        private Vector3 _curMousePos;
        private Vector3 _onMouseDownPos;
        private Vector3 _onMouseUpPos;

        private BaseSelectable _lastSelected;
        
        
        // Start is called before the first frame update
        private void Start()
        {
            if (!targetCamera.TryGetComponent(out _controlledCamera))
            {
                Debug.LogError($"Target-Camera does not contain component of type {typeof(MouseControlledCamera)}");
            }
        }

        // Update is called once per frame
        private void Update()
        {
            _curMousePos = Input.mousePosition;

            if (Input.GetMouseButtonDown((int) mouseBtn))
            {
                OnMouseDown();
            }

            if (Vector3.Distance(_curMousePos, _onMouseDownPos) >= sensitivity && _mouseState == MouseState.Click)
            {
                _mouseState = MouseState.Move;
                // Have the controlled Camera follow the mouse after OnmouseDown and moving
                _controlledCamera.FollowMouse(_onMouseDownPos);
            }

            if (Input.GetMouseButtonUp((int) mouseBtn))
            {
                OnMouseUp();
            }
        }

        private void OnMouseDown()
        {
            if (_mouseState != MouseState.Idle)
                return;

            _mouseState = MouseState.Click;
            _onMouseDownPos = new Vector3(_curMousePos.x, _curMousePos.y, _curMousePos.z);
        }

        private void OnMouseUp()
        {
            _onMouseUpPos = new Vector3(_curMousePos.x, _curMousePos.y, _curMousePos.z);
            
            switch (_mouseState)
            {

                case MouseState.Click:
                {
                    // First, Deselect the previous selected `Selectable` if the selection mode was AutoDeselect
                    if (_lastSelected is not null && _lastSelected.SelectMode == BaseSelectable.SelectType.AutoDeselect)
                    {
                        _lastSelected.Deselect();

                        // Set to null so it won't be deselected multiple times
                        _lastSelected = null;
                    }

                    Vector3 worldPos = targetCamera.ScreenToWorldPoint(_onMouseUpPos);
                    RaycastHit2D hitData = Physics2D.Raycast(worldPos, Vector2.zero, 0);

                    if (hitData && selectableTags.Contains(hitData.collider.tag) &&
                        hitData.collider.TryGetComponent(out BaseSelectable selectable))
                    {
                        // Deselect the previous selected if it had `DeselectOnSelect` selectmode
                        if (_lastSelected is not null &&
                            _lastSelected.SelectMode == BaseSelectable.SelectType.DeselectOnSelect)
                        {
                            _lastSelected.Deselect();
                            _lastSelected = null;
                        }

                        selectable.Select();
                        _lastSelected = selectable;
                    }

                    break;
                }

                case MouseState.Move:
                    _controlledCamera.StopFollow(_onMouseUpPos);
                    break;
            }

            _mouseState = MouseState.Idle;
        }
    }
}
