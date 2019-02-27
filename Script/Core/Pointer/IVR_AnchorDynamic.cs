using UnityEngine;
using UnityEngine.EventSystems;
using IDEALENS.IVR.EventSystems;
using UnityEngine.UI;

/////////////////////////////
/// Description : Anchor (You can implement IVRBasepointer to wirte your own anchor)
/// Author 		: TanSir
/// Date 		: 2019
/// Copyright   : IDEALENS
/////////////////////////////

namespace IDEALENS.IVR
{
	public class IVR_AnchorDynamic : IVRBasePointer
	{

		public float maxReticleDistance = 20.0f;

		private Vector3 mInitLocalPosition;
		private Vector3 mInitLocalScal;
		private float Depth;
		public Image imageGazeProgress;
		public float Speed = 10;
		private float mStartTime;
		private bool haveinit;
		private bool Unhovieinit;
		private float mMoveLenght;
		private bool started = false;
		public static IVR_AnchorDynamic Instance { get; private set; }

		[HideInInspector]
		public float depth = 5f;
		[HideInInspector]
		public float initScal = 0.07998779f;
		public float Width = 100;
		public float Height = 100;
		private MeshRenderer mRender;
		private Material mDynamicMat;
		private int mShaderClipID;
		private Vector3 startforward, startright, startup, startworldpos;
		private float MaxAngleX, MaxAngleY, LengthAngle;
		private float mDistance;
		private float mChangSpeed;
		void Awake()
		{
			Instance = this;
			mRender = GetComponent<MeshRenderer>();
		}
		// Use this for initialization
		protected override void Start()
		{
			base.Start();
			startforward = transform.forward;
			startright = transform.right;
			startup = transform.up;
			startworldpos = transform.position;
			mDynamicMat = mRender.material;
			mInitLocalPosition = transform.localPosition;
			mInitLocalScal = transform.localScale;
			Depth = Mathf.Abs(mInitLocalPosition.z);
			started = true;
			//IVRManager.OnResetPos += IVRManager_OnResetPos;
			mShaderClipID = Shader.PropertyToID("_Offset");

			MaxAngleX = Vector3.Angle(transform.parent.position - transform.position,
				transform.parent.position - (transform.position + transform.right * (Width / 2 - transform.localScale.x)));
			MaxAngleY = Vector3.Angle(transform.parent.position - transform.position,
				transform.parent.position - (transform.position + transform.up * (Height / 2 - transform.localScale.y)));
			LengthAngle = Vector3.Angle(transform.parent.position - (transform.position + transform.up * transform.localScale.y / 2),
				transform.parent.position - (transform.position - transform.up * transform.localScale.y / 2));

			mMoveLenght = transform.localPosition.z;

			overridePointerCamera = IVRManager.Instance.monoCamera;
		}
		public void Hide()
		{
			mRender.enabled = false;
		}
		public void Show()
		{
			mRender.enabled = true;
		}

		private void IVRManager_OnResetPos()
		{
			transform.localPosition = mInitLocalPosition;
			transform.localScale = mInitLocalScal;
		}

		void OnDestroy()
		{
			//IVRManager.OnResetPos -= IVRManager_OnResetPos;
		}
		// Update is called once per frame
		void Update()
		{



			Vector3 currentPos = transform.localPosition;

			float scaleDivisor = Vector3.Distance(currentPos, Vector3.zero) / Depth;

			Vector3 targetScale = mInitLocalScal * scaleDivisor;
			transform.localScale = targetScale;
			Vector3 anchordirection = transform.position - transform.parent.position;
			Vector3 pUpanchordirection = Vector3.ProjectOnPlane(anchordirection, Vector3.up);
			float wAngle = Vector3.Angle(pUpanchordirection, startforward);
			Vector3 pRightanchordirection = Vector3.ProjectOnPlane(anchordirection, Vector3.right);
			float hAngle = Vector3.Angle(pRightanchordirection, startforward);

			if (hAngle > MaxAngleY || wAngle > MaxAngleX)
			{
				//Up or Down
				//if Up
				Vector3 pFowrdanchordirection = Vector3.ProjectOnPlane(anchordirection, startforward).normalized;
				float offsetV = (hAngle - MaxAngleY) / LengthAngle;

				Vector4 clipValue = mDynamicMat.GetVector(mShaderClipID);
				if (pFowrdanchordirection.y > 0)
				{
					clipValue.y = offsetV;
				}
				else
				{
					clipValue.w = offsetV;

				}

				float offseth = (wAngle - MaxAngleX) / LengthAngle;
				if (pFowrdanchordirection.x < 0)
				{
					clipValue.x = offseth;
				}
				else
				{
					clipValue.z = offseth;
				}
				mDynamicMat.SetVector(mShaderClipID, clipValue);
			}
			if (hAngle <= MaxAngleY && wAngle <= MaxAngleX)
			{
				mDynamicMat.SetVector(mShaderClipID, new Vector4(0f, 0, 0, 0));
			}

		}

		[ContextMenu("reset")]
		void reset()
		{
			transform.localPosition = new Vector3(0, 0, depth);
			transform.localScale = new Vector3(initScal, initScal, initScal);
			transform.localRotation = Quaternion.identity;
		}
		[ContextMenu("Checkout")]
		void Checkout()
		{
			GameObject o = Instantiate<GameObject>(gameObject);
			o.transform.parent = transform.parent;
			o.name = gameObject.name;
			//DestroyImmediate(gameObject);
		}

		void OnDrawGizmos()
		{

			if (!Application.isPlaying)
			{
				return;
			}

			Vector3 pleftup, prightup, pleftdown, prightdown;
			pleftup = startworldpos - startright * Width / 2 + startup * Height / 2;
			prightup = startworldpos + startright * Width / 2 + startup * Height / 2;
			pleftdown = startworldpos - startright * Width / 2 - startup * Height / 2;
			prightdown = startworldpos + startright * Width / 2 - startup * Height / 2;
			Gizmos.DrawLine(transform.parent.position, pleftup);
			Gizmos.DrawLine(transform.parent.position, prightup);
			Gizmos.DrawLine(transform.parent.position, pleftdown);
			Gizmos.DrawLine(transform.parent.position, prightdown);
			Gizmos.DrawLine(pleftup, prightup);
			Gizmos.DrawLine(prightup, prightdown);
			Gizmos.DrawLine(prightdown, pleftdown);
			Gizmos.DrawLine(pleftdown, pleftup);
		}
		private void CalculateEndPointAndSpeed(RaycastResult cast)
		{
			mStartTime = Time.time;
			mMoveLenght = Mathf.Abs(mMoveLenght - cast.distance);
		}
		public override void OnPointerEnter(RaycastResult raycastResult, bool isInteractive)
		{
			CalculateEndPointAndSpeed(raycastResult);
		}

		public override void OnPointerHover(RaycastResult raycastResultResult, bool isInteractive)
		{
			float deltastep = ((Time.time - mStartTime) * Speed) / mMoveLenght;
			transform.position = Vector3.Lerp(transform.position, raycastResultResult.worldPosition, deltastep);
		}

		public override void OnGazePointerProcess (float gazeTriggerProgress)
		{
			base.OnGazePointerProcess (gazeTriggerProgress);
			imageGazeProgress.fillAmount = gazeTriggerProgress;

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

		public override void GetPointerRadius (out float enterRadius, out float exitRadius)
		{
			enterRadius = 0;
			exitRadius = 0;
		}

		public override float MaxPointerDistance { get { return maxReticleDistance; } }

	}
}

