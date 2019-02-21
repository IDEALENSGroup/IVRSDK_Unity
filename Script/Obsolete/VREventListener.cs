/******************************************************************

** auth : xmh
** date : 2016/5/5 21:51:15
** desc : This program is anchor controller,Is a subclass of AnchorWidget
** Ver. : V1.0.0

******************************************************************/
using UnityEngine;
using System.Collections;

[System.Obsolete("Legacy Class, please implement your own event listener or use UGUI one!")]
public class VREventListener : AnchorWidget
{

    /// <summary>
    /// Get the specified control.
    /// </summary>
    /// <param name="go">The anchor control</param>
    public static AnchorWidget Get(GameObject go)
    {
        AnchorWidget listener = go.GetComponent<AnchorWidget>();
        if (listener == null) listener = go.AddComponent<VREventListener>();
        return listener;
    }


    public override void RunOnClick(GameObject obj)
    {
        base.RunOnClick(obj);
        //Debug.LogError("Click");
    }

    public override void RunOnLongPress()
    {
        base.RunOnLongPress();
        //Debug.LogError("DoubleClick");
    }

}
