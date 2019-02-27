using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using IDEALENS.IVR.EventSystems;
using IDEALENS.IVR.InputPlugin;

/////////////////////////////
/// Description : Show Laser 
/// Author 		: TanSir
/// Date 		: 2019
/// Copyright   : IDEALENS
/////////////////////////////

namespace IDEALENS.IVR
{
	[RequireComponent(typeof(LineRenderer))]
	public class IVRTrackLaser : IVRBasePointer
	{

		public const float DEFAULT_POINTER_DISTANCE = 20.0f;
		public const float RETICLE_SIZE_METERS = 0.017f;


		[Tooltip("Maximum distance of the laser (meters).")]
		[Range(0.0f, 10.0f)]
		public float maxLaserDistance = 0.75f;

		[Tooltip("Determines if the visual is allowed to rotate toward the final reticle position.")]
		public bool allowRotation = true;

		[Tooltip("References the reticle that will be positioned.")]
		[SerializeField]
		private Transform reticle;

		[Tooltip("References to the controller visual.")]
		public Transform controller;

		[Tooltip("The rate that the currentPosition changes.")]
		public float lerpSpeed = 20.0f;

		[Tooltip("Determines if lerping is used.")]
		public float lerpThreshold = 1.5f;

		[Range(-32767, 32767)]
		public int reticleSortingOrder = 32767;

		private const float LERP_CLAMP_THRESHOLD = 0.02f;


		public Transform Reticle
		{
			get
			{
				return reticle;
			}
			set
			{
				reticle = value;
				ReticleMeshSizeMeters = 1.0f;
				ReticleMeshSizeRatio = 1.0f;

				if (reticle != null)
				{
					MeshFilter meshFilter = reticle.GetComponent<MeshFilter>();
					if (meshFilter != null && meshFilter.mesh != null)
					{
						ReticleMeshSizeMeters = meshFilter.mesh.bounds.size.x;
						if (ReticleMeshSizeMeters != 0.0f)
						{
							ReticleMeshSizeRatio = 1.0f / ReticleMeshSizeMeters;
						}
					}
				}
			}
		}

		public float ReticleMeshSizeMeters { get; private set; }

		public float ReticleMeshSizeRatio { get; private set; }

		public LineRenderer Laser { get; private set; }

		public delegate Vector3 GetPointForDistanceDelegate(float distance);
		public GetPointForDistanceDelegate GetPointForDistanceFunction { get; set; }

		private float targetDistance;
		private float currentDistance;
		private Vector3 currentPosition;
		private Vector3 currentLocalPosition;
		private Quaternion currentLocalRotation;
		private Vector3 mPointerOrigin;
		private Vector3 mPointerForward;
		private string ModelPath_Ximmers = "Prefab/Ximers_Design";
		private string ModelPath_H1000 = "Prefab/H1000_Design";
		private GameObject ModelPath_Ximmers_prefab;
		private GameObject ModelPath_H1000_prefab;
		private Dictionary<IVRInput.Controller, GameObject> mControllerMap = new Dictionary<IVRInput.Controller, GameObject>();
		private IVRInput.Controller mPreviousConnected = IVRInput.Controller.None;
		public void SetDistance(float distance, bool immediate = false)
		{
			targetDistance = distance;
			if (immediate)
			{
				currentDistance = targetDistance;
			}

			if (targetDistance > lerpThreshold)
			{
				currentDistance = targetDistance;
			}
		}

		void Awake()
		{
			Laser = GetComponent<LineRenderer>();

			Reticle = reticle;

			if (reticle != null)
			{
				Renderer reticleRenderer = reticle.GetComponent<Renderer>();
				Assert.IsNotNull(reticleRenderer);
				reticleRenderer.sortingOrder = reticleSortingOrder;
			}
			SetDistance(DEFAULT_POINTER_DISTANCE);

			ModelPath_Ximmers_prefab = Resources.Load<GameObject>(ModelPath_Ximmers);
			ModelPath_H1000_prefab = Resources.Load<GameObject>(ModelPath_H1000);

			CreatHandeModel();
		}
		void OnEnable()
		{
			overridePointerCamera = IVRManager.Instance.monoCamera;
		}
		void CreatHandeModel()
		{
			//Debug.Log("Connect Remot " + (IVRInput.GetConnectedControllers()));

			if (ModelPath_Ximmers_prefab == null)
			{
				Debug.LogError("Ximmers model creation failed,because of the ximmerse prefab was removed frome " + ModelPath_Ximmers + " .");
			}
			else
			{
				GameObject ximmersobj = Instantiate(ModelPath_Ximmers_prefab, transform.parent, false) as GameObject;
				ximmersobj.name = "_ximmers_design";
				ximmersobj.SetActive(false);
				mControllerMap.Add(IVRInput.Controller.Remote_Ximmerse, ximmersobj);
			}


			if (ModelPath_H1000_prefab == null)
			{
				Debug.LogError("H1000 model creation failed,because of the H1000 prefab was removed " + ModelPath_H1000 + " .");
			}
			else
			{
				GameObject h1000obj = Instantiate(ModelPath_H1000_prefab, transform.parent, false) as GameObject;
				h1000obj.name = "_h1000_design";
				h1000obj.SetActive(false);
				mControllerMap.Add(IVRInput.Controller.Remote_H1000, h1000obj);
			}

		}
		void SwitchHandModel()
		{
			IVRInput.Controller connected = IVRInput.GetConnectedControllers() & IVRInput.Controller.Remote;
			if ((mPreviousConnected & connected) == 0)
			{
				switch (connected)
				{
				case IVRInput.Controller.Remote_Ximmerse:
					if (mControllerMap.ContainsKey(IVRInput.Controller.Remote_Ximmerse) && !mControllerMap[IVRInput.Controller.Remote_Ximmerse].activeInHierarchy)
					{
						mControllerMap[IVRInput.Controller.Remote_Ximmerse].SetActive(true);
						controller = mControllerMap[IVRInput.Controller.Remote_Ximmerse].transform;

					}
					if (mControllerMap.ContainsKey(IVRInput.Controller.Remote_H1000) && mControllerMap[IVRInput.Controller.Remote_H1000].activeInHierarchy)
					{
						mControllerMap[IVRInput.Controller.Remote_H1000].SetActive(false);
					}
					break;
				case IVRInput.Controller.Remote_H1000:
					if (mControllerMap.ContainsKey(IVRInput.Controller.Remote_H1000) && !mControllerMap[IVRInput.Controller.Remote_H1000].activeInHierarchy)
					{
						mControllerMap[IVRInput.Controller.Remote_H1000].SetActive(true);
						controller = mControllerMap[IVRInput.Controller.Remote_H1000].transform;

					}
					if (mControllerMap.ContainsKey(IVRInput.Controller.Remote_Ximmerse) && mControllerMap[IVRInput.Controller.Remote_Ximmerse].activeInHierarchy)
					{
						mControllerMap[IVRInput.Controller.Remote_Ximmerse].SetActive(false);

					}
					break;
				default:
					break;
				}
			}

			mPreviousConnected = connected;
		}
		void Update()
		{
			SwitchHandModel();
		}
		void OnDisable()
		{
		}

		void LateUpdate()
		{
			UpdateCurrentPosition();
			UpdateControllerOrientation();
			UpdateReticlePosition();
			UpdateLaserEndPoint();
			UpdateLaserAlpha();
		}

		void OnWillRenderObject()
		{
			Camera camera = Camera.main;
			UpdateReticleSize(camera);
			UpdateReticleOrientation(camera);
		}

		private void UpdateCurrentPosition()
		{
			if (currentDistance != targetDistance)
			{
				float speed = GetSpeed();
				currentDistance = Mathf.Lerp(currentDistance, targetDistance, speed);
				float diff = Mathf.Abs(targetDistance - currentDistance);
				if (diff < LERP_CLAMP_THRESHOLD)
				{
					currentDistance = targetDistance;
				}
			}

			//Debug.DrawRay(mCurrentPointerRay.ray.origin, mCurrentPointerRay.ray.direction * Camera.main.farClipPlane,Color.black);
			currentPosition = GetPointAlongPointer(currentDistance);

			currentLocalPosition = transform.InverseTransformPoint(currentPosition);

			if (allowRotation)
			{
				currentLocalRotation =
					Quaternion.FromToRotation(Vector3.forward, currentLocalPosition);
			}
			else
			{
				currentLocalRotation = Quaternion.identity;
			}
		}
		private Vector3 GetPointAlongPointer(float distance)
		{
			PointerRay pointerRay = CalculateRay(this, raycastMode);
			return pointerRay.ray.GetPoint(distance - pointerRay.distanceFromStart);
		}

		private void UpdateControllerOrientation()
		{
			if (controller == null)
			{
				return;
			}

			controller.localRotation = currentLocalRotation;
		}

		private void UpdateReticlePosition()
		{
			if (reticle == null)
			{
				return;
			}
			reticle.position = currentPosition;
		}

		private void UpdateLaserEndPoint()
		{
			if (Laser == null)
			{
				return;
			}

			Vector3 laserStartPoint = Vector3.zero;
			Vector3 laserEndPoint;

			if (allowRotation)
			{
				if (controller != null)
				{
					Vector3 worldPosition = transform.position;
					Vector3 rotatedPosition = controller.InverseTransformPoint(worldPosition);
					rotatedPosition = currentLocalRotation * rotatedPosition;
					laserStartPoint = controller.TransformPoint(rotatedPosition);
					laserStartPoint = transform.InverseTransformPoint(laserStartPoint);
				}

				laserEndPoint = Vector3.ClampMagnitude(currentLocalPosition, maxLaserDistance);
			}
			else
			{
				Vector3 projected = Vector3.Project(currentLocalPosition, Vector3.forward);
				laserEndPoint = Vector3.ClampMagnitude(projected, maxLaserDistance);
			}

			Laser.useWorldSpace = false;
			Laser.SetPosition(0, laserStartPoint);
			Laser.SetPosition(1, laserEndPoint);
		}

		private void UpdateLaserAlpha()
		{
			float alpha = IVRTrackArm.mHandAlpha;
			Color resultColor = Color.white;
			resultColor.a = alpha;
			Laser.material.SetColor("_Color", resultColor);
		}

		private void UpdateReticleSize(Camera camera)
		{
			if (reticle == null)
			{
				return;
			}

			if (camera == null)
			{
				return;
			}

			float reticleDistanceFromCamera = (reticle.position - camera.transform.position).magnitude;
			float scale = RETICLE_SIZE_METERS * ReticleMeshSizeRatio * reticleDistanceFromCamera;
			reticle.localScale = new Vector3(scale, scale, scale);
		}

		private void UpdateReticleOrientation(Camera camera)
		{
			if (reticle == null)
			{
				return;
			}

			if (camera == null)
			{
				return;
			}
			Vector3 direction = reticle.position - camera.transform.position;
			reticle.rotation = Quaternion.LookRotation(direction, Vector3.up);

		}

		private float GetSpeed()
		{
			return lerpSpeed > 0.0f ? lerpSpeed * Time.deltaTime : 1.0f;
		}

		void OnValidate()
		{
			if (Application.isPlaying && Laser != null)
			{
				Reticle = reticle;
			}
		}

		public override void OnPointerEnter(RaycastResult raycastResult, bool isInteractive)
		{
			SetDistance(raycastResult.distance);
		}

		public override void OnPointerHover(RaycastResult raycastResultResult, bool isInteractive)
		{
			SetDistance(raycastResultResult.distance);
		}

		public override void OnPointerExit(GameObject previousObject)
		{
		}

		public override void OnPointerClickDown()
		{
		}

		public override void OnPointerClickUp()
		{
		}

		public override float MaxPointerDistance { get { return DEFAULT_POINTER_DISTANCE; } }
		public override void GetPointerRadius (out float enterRadius, out float exitRadius)
		{
			enterRadius = 0;
			exitRadius = 0;
		}
	}

}
