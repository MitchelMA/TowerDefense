using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

        [SerializeField] private GameObject towerStatsUI;
        [SerializeField] private GameObject towerBuyUI;

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
                _controlledCamera.StartFollowMouse(_onMouseDownPos);
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
                    RaycastHit2D[] hits = Physics2D.RaycastAll(worldPos, Vector2.zero, 0);
                    BaseSelectable selectable = null;
                    bool shouldSelect = false;

                    foreach (RaycastHit2D hit in hits)
                    {
                        if (selectable is null && selectableTags.Contains(hit.collider.tag) &&
                            hit.collider.TryGetComponent(out selectable))
                        {
                            shouldSelect = true;
                        }

                        if (hit.collider.gameObject == towerBuyUI || hit.collider.gameObject == towerStatsUI)
                        {
                            shouldSelect = false;
                            break;
                        }
                    }

                    if (shouldSelect && selectable is not null)
                    {
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
