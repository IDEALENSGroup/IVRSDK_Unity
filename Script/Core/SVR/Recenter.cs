using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using SnapdragonVR;

public class Recenter : MonoBehaviour 
{
    public Transform Reference = null;
	public float Damping = .5f;
	public float ExpDampCoef = -20;

	private Transform[] objectTransforms;
	private List<Vector3> objectPositions;

	private bool recenter = false;

	void Awake()
	{
        if (Reference == null) Reference = SvrManager.Instance.head;
		objectTransforms = gameObject.GetComponentsInChildren<Transform>();
		objectPositions = new List<Vector3>(objectTransforms.Length);
		for(int i=0; i<objectTransforms.Length; i++)
		{
			objectPositions.Add(objectTransforms[i].localPosition);
		}
	}

	void LateUpdate()
	{
		for(int i=0; i<objectTransforms.Length; i++)
		{
			Vector3 targetPosition = Reference.TransformPoint(objectPositions[i]);
			Quaternion targetRotation = Reference.rotation;
			if (!recenter && Damping > 0)
			{
				objectTransforms[i].position = Vector3.Lerp(objectTransforms[i].position, targetPosition, Damping * (1f - Mathf.Exp(ExpDampCoef * Time.deltaTime)));
				objectTransforms[i].rotation = Quaternion.Slerp(objectTransforms[i].rotation, targetRotation, Damping * (1f - Mathf.Exp(ExpDampCoef * Time.deltaTime)));
			}
			else
			{
				objectTransforms[i].position = targetPosition;
				objectTransforms[i].rotation = targetRotation;
			}
		}
	}

}
