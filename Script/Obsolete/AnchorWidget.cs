/******************************************************************

** auth : xmh
** date : 2016/5/5 21:51:15
** desc : Processing anchor point event, the event will be assigned to a single widget.
** Ver. : V1.0.0

******************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using IDEALENS.IVR.EventSystems;
using IDEALENS.IVR;

[System.Obsolete("Legacy Class, please implement your own IEventSystemHandler or use UGUI one!")]
public abstract class AnchorWidget : MonoBehaviour, IEventSystemHandler,
    IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerHoverHandler,IPointerEnterHandler,IPointerExitHandler, IMoveHandler, IDragHandler, IBeginDragHandler
{
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (isActiveAndEnabled)
        {
            RunOnPress(true);
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (isActiveAndEnabled)
        {
            RunOnPress(false);
            bSwipe = false;
            mBeginSwipPosition = Vector2.zero;
        }
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData pointerData)
    {
        if (isActiveAndEnabled)
        {
            RunOnClick(gameObject);
            if (pointerData.clickCount == 2)
            {
                RunOnDoubleClick();
            }
        }
    }

    private Vector2 mBeginSwipPosition = Vector2.zero;
    private bool bSwipe = false;
    void IMoveHandler.OnMove(AxisEventData eventData)
    {
        if (bSwipe) return;
        if (mBeginSwipPosition == Vector2.zero)
        {
            mBeginSwipPosition = eventData.moveVector;
            return;
        }
        if (isActiveAndEnabled)
        {
            Vector2 delta = eventData.moveVector - mBeginSwipPosition;
            if (delta.magnitude > 25)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    RunOnSwip(delta.x > 0 ? SwipEnum.MOVE_UP : SwipEnum.MOVE_DOWN);
                }
                else
                {
                    RunOnSwip(delta.y < 0 ? SwipEnum.MOVE_FOWRAD : SwipEnum.MOVE_BACK);
                }
                bSwipe = true;
            }
        }
    }

    private Vector2 mBeginDragPosition = Vector2.zero;
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        //IVRRayPointerEventData pointer = eventData as IVRRayPointerEventData;
        //if (pointer == null) return;
        mBeginDragPosition = eventData.pointerCurrentRaycast.worldPosition;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        //IVRRayPointerEventData pointer = eventData as IVRRayPointerEventData;
        //if (pointer == null) return;
        //if (isActiveAndEnabled)
        //{
        //    Vector2 delta = pointer.TouchPadPosition - mBeginDragPosition;
        //    //if (delta.magnitude > 50)
        //    //{
        //    //    mBeginDragPosition = pointer.TouchPadPosition;
        //    //    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        //    //    {
        //    //        RunOnSwip(delta.x > 0 ? SwipEnum.MOVE_UP : SwipEnum.MOVE_DOWN);
        //    //    }
        //    //    else
        //    //    {
        //    //        RunOnSwip(delta.y > 0 ? SwipEnum.MOVE_FOWRAD : SwipEnum.MOVE_BACK);
        //    //    }
        //    //}
            
        //}

        RunOnDrag(eventData.delta);
    }

    /// <summary>
    /// Anchor point contacted world coordinates
    /// </summary>
    public Vector3 hitPoint
    {
        get;
        private set;
    }

	public bool isHover{get;private set;}

    /// <summary>
    /// Deleting
    /// </summary>
    private bool isDestroying;


    #region delegate
    public delegate void VoidDelegate(GameObject go);
    public delegate void BoolDelegate(GameObject go, bool state);
    public delegate void FloatDelegate(GameObject go, float delta);
    public delegate void VectorDelegate(GameObject go, Vector2 delta);
    public delegate void ObjectDelegate(GameObject go, GameObject obj);
    public delegate void KeyCodeDelegate(GameObject go, KeyCode key);
    public delegate void SwipeDelegate(GameObject go, SwipEnum type);

    /// <summary>
    /// long press event
    /// </summary>
    public VoidDelegate onLongPress;
    /// <summary>
    /// Click event, if the argument is an empty object means it was not its own click
    /// </summary>
    public VoidDelegate OnClickEvent;
    /// <summary>
    /// double click
    /// </summary>
    public VoidDelegate onDoubleClick;
    /// <summary>
    /// focuse events
    /// </summary>
    public BoolDelegate onHover;
    /// <summary>
    /// Touchpad press/lift event
    /// </summary>
    public BoolDelegate onPress;
    /// <summary>
    /// Touchpad long-swipe event
    /// </summary>
    public VectorDelegate onDrag;
    /// <summary>
    /// key event
    /// </summary>
    public KeyCodeDelegate onKey;
    /// <summary>
    /// swipe event
    /// </summary>
    public SwipeDelegate onSwipe;
    #endregion

    #region Public Func
    public void LoseFocus()
    {
        hitPoint = Vector3.zero;
        RunOnHover(false);
    }

    public void SetHiter(Vector3 hit)
    {
        hitPoint = hit;
    }

    [System.Obsolete("Legacy function, please don't use this!")]
    public void OnTransformParentChanged()
    {
        //if (isDestroying)
        //{
        //    return;
        //}

        //CheckPanel();
    }

    [System.Obsolete("Legacy function, please don't use this!")]
    static public T FindInParents<T>(GameObject go) where T : Component
    {
        //if (go == null) return null;
        T comp = null;
        //Transform t = go.transform.parent;

        //while (t != null && comp == null)
        //{
        //    comp = t.gameObject.GetComponent<T>();
        //    t = t.parent;
        //}
        return comp;
    }
    #endregion

    #region Virtual Func
    public virtual void RunOnLongPress() { if (onLongPress != null) onLongPress(gameObject); }
    /// <summary>
    /// Trigger click event
    /// </summary>
    public virtual void RunOnClick(GameObject obj) { if (OnClickEvent != null)  OnClickEvent(obj); }
    /// <summary>
    /// trigger double click event
    /// </summary>
    public virtual void RunOnDoubleClick() { if (onDoubleClick != null) onDoubleClick(gameObject); }

    /// <summary>
    /// trigger focuse event
    /// </summary>
	public virtual void RunOnHover(bool isOver) { isHover = isOver;if (onHover != null) onHover(gameObject, isOver); }

    /// <summary>
    /// trigger press event
    /// </summary>
    public virtual void RunOnPress(bool isPressed) { if (onPress != null) onPress(gameObject, isPressed); }

    /// <summary>
    /// trigger sliding event
    /// </summary>
    public virtual void RunOnDrag(Vector2 delta) { if (onDrag != null) onDrag(gameObject, delta); }

    /// <summary>
    /// trigger key event
    /// </summary>
    public virtual void RunOnKey(GameObject go, KeyCode key) { if (onKey != null) onKey(go, key); }

    /// <summary>
    /// trigger swip event
    /// </summary>
    /// <param name="go">Go.</param>
    /// <param name="type">Type.</param>
    public virtual void RunOnSwip(SwipEnum type) { if (onSwipe != null) onSwipe(gameObject, type); }

    protected virtual void Awake() {}

    protected virtual void Start() { }

    protected virtual void OnEnable()
    {
        //CheckPanel();
        //IVRTouchPad.TouchEvent_onSingleTap += HandleTouchEvent_onSingleTap;
        //IVRTouchPad.KeyEvent_Back += HandleKeyEvent_Back;
    }

    protected virtual void OnDisable()
    {
        //IVRTouchPad.TouchEvent_onSingleTap -= HandleTouchEvent_onSingleTap;
        //IVRTouchPad.KeyEvent_Back -= HandleKeyEvent_Back;
    }

    protected virtual void OnDestroy()
    {
        isDestroying = true;
    }

    public void OnPointerHover(PointerEventData eventData)
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        RunOnHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RunOnHover(false);
    }

    #endregion

}
