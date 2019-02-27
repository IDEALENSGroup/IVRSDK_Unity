//-----------------------------------------------------------------------------
//  Copyright (c) 2017 Qualcomm Technologies, Inc.
//  All Rights Reserved. Qualcomm Technologies Proprietary and Confidential.
//-----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Svr controller.
/// </summary>
public class SvrController : MonoBehaviour, SvrManager.SvrEventListener {

    string controllerParams = "";

    //! \brief Events to use in svrControllerSendEvent
    public enum svrControllerMessageType
    {
        kControllerMessageRecenter = 0,
        kControllerMessageVibration = 1
    };

    //! \brief Query Values
    public enum svrControllerQueryType
    {
        kControllerBatteryRemaining,
		kControllerControllerCaps
    };

    //! Controller Connection state
    public enum svrControllerConnectionState {
        kNotInitialized = 0,
        kDisconnected   = 1,
        kConnected      = 2,
        kConnecting     = 3,
        kError          = 4
    };

    /// <summary>
    /// Start this instance.
    /// </summary>
    //---------------------------------------------------------------------------------------------
    void Start ()
    {
        //Register for SvrEvents
        SvrManager.Instance.AddEventListener (this);
    }

    /// <summary>
    /// Raises the svr event event.
    /// </summary>
    /// <param name="ev">Ev.</param>
    //---------------------------------------------------------------------------------------------
    public void OnSvrEvent(SvrManager.SvrEvent ev)
    {
        switch (ev.eventType) {
            case SvrManager.svrEventType.kEventVrModeStarted:
                handle = SvrManager.Instance.ControllerStartTracking (controllerParams);
                space = GetCapability.caps != 0 ? 1 : 0; // Has Position so needs to be transformed from HMD to World space
                break;
        }
    }

    /// <summary>
    /// Raises the application pause event.
    /// </summary>
    /// <param name="isPaused">If set to <c>true</c> is paused.</param>
    //---------------------------------------------------------------------------------------------
    void OnApplicationPause(bool isPaused)
    {
        if (isPaused) {
            SvrManager.Instance.ControllerStopTracking (handle);
        }
    }

    /// <summary>
    /// Get the current controller state
    /// </summary>
    /// <returns>The state.</returns>
    //---------------------------------------------------------------------------------------------
    public SvrControllerState State
    {
        get {
            return currentState;
        }
    }

    /// <summary>
    /// Gets the state of the connection.
    /// </summary>
    /// <value>The state of the connection.</value>
    //---------------------------------------------------------------------------------------------
    public svrControllerConnectionState ConnectionState {
        get {
            return (svrControllerConnectionState)currentState.connectionState;
        }
    }

    /// <summary>
    /// Sends the message.
    /// </summary>
    /// <param name="what">What.</param>
    /// <param name="arg1">Arg1.</param>
    /// <param name="arg2">Arg2.</param>
    //---------------------------------------------------------------------------------------------
    public void SendMessage(svrControllerMessageType what, int arg1, int arg2)
    {
        SvrManager.Instance.ControllerSendMessage(handle, what, arg1, arg2);

    }

    /// <summary>
    /// Recenter this instance.
    /// </summary>
    //---------------------------------------------------------------------------------------------
    public void Recenter( )
    {
        SvrManager.Instance.ControllerSendMessage(handle,
                                                    svrControllerMessageType.kControllerMessageRecenter,
                                                    0,
                                                    0);
    }

    /// <summary>
    /// Send message to vibrate
    /// </summary>
    //---------------------------------------------------------------------------------------------
    public void Vibrate(int arg1, int arg2)
    {
        SvrManager.Instance.ControllerSendMessage (handle,
                                                    svrControllerMessageType.kControllerMessageVibration,
                                                    arg1,
                                                    arg2);
    }

    /// <summary>
    /// Gets the current state of the button.
    /// </summary>
    /// <returns><c>true</c>, if button is down, <c>false</c> otherwise.</returns>
    /// <param name="buttonId">Button identifier.</param>
    //---------------------------------------------------------------------------------------------
    public bool GetButton(svrControllerButton buttonId)
    {
        int mask = (int)buttonId;
        return ((currentState.buttonState & mask) != 0);
    }

    /// <summary>
    /// Gets the button up.
    /// </summary>
    /// <returns><c>true</c>, if button is up this frame, <c>false</c> otherwise.</returns>
    /// <param name="buttonId">Button identifier.</param>
    //---------------------------------------------------------------------------------------------
    public bool GetButtonUp(svrControllerButton buttonId)
    {
        int mask = (int)(buttonId);
        return ((previousButtonState & mask) != 0) && ((currentState.buttonState & mask) == 0);
    }

    /// <summary>
    /// Gets the button down.
    /// </summary>
    /// <returns><c>true</c>, if button is down this frame, <c>false</c> otherwise.</returns>
    /// <param name="buttonId">Button identifier.</param>
    //---------------------------------------------------------------------------------------------
    public bool GetButtonDown(svrControllerButton buttonId)
    {
        int mask = (int)buttonId;
        return ((previousButtonState & mask) == 0) && ((currentState.buttonState & mask) != 0);
    }

    /// <summary>
    /// Get the current orientation.
    /// </summary>
    /// <value>The orientation.</value>
    //---------------------------------------------------------------------------------------------
    public Quaternion Orientation
    {
        get {
            return currentState.rotation;
        }
    }

    /// <summary>
    /// Get the current position.
    /// </summary>
    /// <value>The position.</value>
    //---------------------------------------------------------------------------------------------
    public Vector3 Position {
        get {
            return currentState.position;
        }
    }

    /// <summary>
    /// the timestamp.
    /// </summary>
    /// <value>The timestamp.</value>
    //---------------------------------------------------------------------------------------------
    public long Timestamp {
        get {
            return currentState.timestamp;
        }
    }

    /// <summary>
    /// Gets the analog.
    /// </summary>
    /// <returns>The analog.</returns>
    /// <param name="id">Identifier.</param>
    //---------------------------------------------------------------------------------------------
    public Vector2 GetAxis2D(svrControllerAxis2D axi2d)
    {
        return currentState.analog2D != null ? currentState.analog2D [(int)axi2d] : Vector2.zero;
    }

    /// <summary>
    /// Gets the analog.
    /// </summary>
    /// <returns>The analog.</returns>
    /// <param name="id">Identifier.</param>
    //---------------------------------------------------------------------------------------------
    public float GetAxis1D(svrControllerAxis1D axis1d)
    {
        return currentState.analog1D != null ? currentState.analog1D [(int)axis1d] : 0f;
    }


    /// <summary>
    /// Determines whether this instance is touching the specified id.
    /// </summary>
    /// <returns><c>true</c> if this instance is touching the specified id; otherwise, <c>false</c>.</returns>
    /// <param name="id">Identifier.</param>
    //---------------------------------------------------------------------------------------------
    public bool GetTouch(svrControllerTouch touch)
    {
        int mask = (int)touch;
        return ((currentState.isTouching & mask) != 0);
    }

    /// <summary>
    /// Gets the touch down.
    /// </summary>
    /// <returns><c>true</c>, if touch down was gotten, <c>false</c> otherwise.</returns>
    /// <param name="id">Identifier.</param>
    //---------------------------------------------------------------------------------------------
    public bool GetTouchDown(svrControllerTouch touch)
    {
        int mask = (int)touch;
        return ((previousTouchState & mask) == 0) && ((currentState.isTouching & mask) != 0);
    }

    /// <summary>
    /// Gets the touch up.
    /// </summary>
    /// <returns><c>true</c>, if touch up was gotten, <c>false</c> otherwise.</returns>
    /// <param name="id">Identifier.</param>
    //---------------------------------------------------------------------------------------------
    public bool GetTouchUp(svrControllerTouch touch)
    {
        int mask = (int)touch;
        return ((previousTouchState & mask) != 0) && ((currentState.isTouching & mask) == 0);
    }

    /// <summary>
    /// Gets the battery.
    /// </summary>
    /// <value>The battery.</value>
    //---------------------------------------------------------------------------------------------
    public int BatteryLevel {
        get {
            int batteryLevel = -1;
            object obj = (SvrManager.Instance.ControllerQuery(handle, svrControllerQueryType.kControllerBatteryRemaining));
            if (obj != null) {
                batteryLevel = (int)(obj);
            }
            return batteryLevel;
        }
    }

    /// <summary>
    public SvrControllerCaps GetCapability
    {
        get
        {
            SvrControllerCaps Cap = new SvrControllerCaps();
            object obj = (SvrManager.Instance.ControllerQuery(handle, svrControllerQueryType.kControllerControllerCaps));
            if (obj != null)
            {
                Cap = (SvrControllerCaps)(obj);
            }
            return Cap;
        }
    }
    /// Raises the enable event.
    /// </summary>
    //---------------------------------------------------------------------------------------------
    public void OnEnable()
    {
        frameDelimiter = StartCoroutine (OnFrameEnd());
    }

    /// <summary>
    /// Raises the disable event.
    /// </summary>
    //---------------------------------------------------------------------------------------------
    public void OnDisable()
    {
        StopCoroutine (frameDelimiter);
    }

    /// <summary>
    /// Raises the per frame event.
    /// </summary>
    //---------------------------------------------------------------------------------------------
    IEnumerator OnFrameEnd()
    {
        while (true) {
            yield return waitForEndOfFrame;
            previousButtonState = currentState.buttonState;
            previousTouchState = currentState.isTouching;
            currentState = SvrManager.Instance.ControllerGetState (handle, space);
        }
    }

    /**
     *
     */
    public object Query(svrControllerQueryType what)
    {
        return SvrManager.Instance.ControllerQuery (handle, what);
    }

    /**
     * Handle for the Controller.
     */
    private int handle = -1;

    /**
     * Handle for the Controller.
     */
    private int space = 0;

    /**
     * The Current State. Updated each frame.
     */
    private SvrControllerState currentState;

    /**
     * Previous Button State.
     */
    private int previousButtonState = 0;

    /**
     * Previous Touch State.
     */
    private int previousTouchState = 0;

    /**
     * Coroutine for WaitForEndOfFrame
     */
    private Coroutine frameDelimiter = null;

    /**
     *
     */
    private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    //! Controller Touch button enumerations
    public enum svrControllerTouch {
        None                = 0x00000000,
        One                 = 0x00000001,
        Two                 = 0x00000002,
        Three               = 0x00000004,
        Four                = 0x00000008,
        PrimaryThumbstick   = 0x00000010,
        SecondaryThumstick  = 0x00000020,
        Any                 = ~None
    };

    //! Controller Trigger enumerations
    public enum svrControllerAxis1D {
        PrimaryIndexTrigger   = 0x00000000,
        SecondaryIndexTrigger = 0x00000001,
        PrimaryHandTrigger    = 0x00000002,
        SecondaryHandTrigger  = 0x00000003
    };

    //! Controller Joystick enumerations
    public enum svrControllerAxis2D {
        PrimaryThumbstick     = 0x00000000,
        SecondaryThumbstick   = 0x00000001
    };

    //! Controller Button enumerations
    public enum svrControllerButton {
        None                    = 0x00000000,
        One                     = 0x00000001,
        Two                     = 0x00000002,
        Three                   = 0x00000004,
        Four                    = 0x00000008,
        DpadUp                  = 0x00000010,
        DpadDown                = 0x00000020,
        DpadLeft                = 0x00000040,
        DpadRight               = 0x00000080,
        Start                   = 0x00000100,
        Back                    = 0x00000200,
        PrimaryShoulder         = 0x00001000,
        PrimaryIndexTrigger     = 0x00002000,
        PrimaryHandTrigger      = 0x00004000,
        PrimaryThumbstick       = 0x00008000,
        PrimaryThumbstickUp     = 0x00010000,
        PrimaryThumbstickDown   = 0x00020000,
        PrimaryThumbstickLeft   = 0x00040000,
        PrimaryThumbstickRight  = 0x00080000,
        SecondaryShoulder       = 0x00100000,
        SecondaryIndexTrigger   = 0x00200000,
        SecondaryHandTrigger    = 0x00400000,
        SecondaryThumbstick     = 0x00800000,
        SecondaryThumbstickUp   = 0x01000000,
        SecondaryThumbstickDown = 0x02000000,
        SecondaryThumbstickLeft = 0x04000000,
        SecondaryThumbstickRight = 0x08000000,
        Up                      = 0x10000000,
        Down                    = 0x20000000,
        Left                    = 0x40000000,
        Right                   = unchecked((int)0x80000000),
        Any                     = ~None
    };
}
