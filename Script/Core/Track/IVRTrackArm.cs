using UnityEngine;
using IDEALENS.IVR.InputPlugin;

/////////////////////////////
/// Description : Hand(Handler)
/// Author 		: TanSir
/// Date 		: 2019
/// Copyright   : IDEALENS
/////////////////////////////

namespace IDEALENS.IVR
{
	public class IVRTrackArm : IVRTrackObject
	{
		[Tooltip("You can set left hand or right hand.We default to right hand.")]
		public HandSide handedness = HandSide.Right;

		[Tooltip("Height of the elbow.")]
		[Range(0.0f, 1f)]
		public float addedElbowHeight = 0.0f;

		[Tooltip("Depth of the elbow.")]
		[Range(0.0f, 0.2f)]
		public float addedElbowDepth = 0.2f;

		[Tooltip("Elbow to Hand in length.")]
		[Range(0.0f, 1.0f)]
		public float ElbowToHandLength = 0.25f;

		[Tooltip("The Downward tilt or pitch of the laser pointer relative to the controller.")]
		[HideInInspector]
		[Range(0.0f, 30.0f)]
		public float pointerTiltAngle = 15.0f;

		[Tooltip("Controller distance from the face after which the controller disappears.")]
		[Range(0.0f, 0.4f)]
		public float fadeDistanceFromFaceToHand = 0.32f;


		[Tooltip("If true, the root of the pose is locked to the local position of the player's head.")]
		public bool isLockedToHead = true;


		public  static float mHandAlpha
		{
			get
			{
				return handAlpha;
			}
		}
		private Vector3 handedMultiplier;

		private Vector3 wristPosition;

		private Quaternion wristRotation;

		private static float handAlpha;

		private static readonly Vector3 POINTER_OFFSET_H1000 = new Vector3(0.0f, 0.0121f, 0.05492f);
		private static readonly Vector3 POINTER_OFFSET_XIMMERS = new Vector3(0.0f, 0.0023f, 0.0599f);

		private static readonly Vector3 mArmRootPosition = new Vector3(0.195f, -0.5f, -0.075f);
		private const float DELTA_ALPHA = 4.0f;

		private Transform mLaserVisual;

		void OnEnable()
		{
			CalculatHandSide();
		}
		protected override void Awake()
		{
			base.Awake();
			mLaserVisual = GetComponentInChildren<IVRTrackLaser>().transform;
		}


		protected override void OnDataUpdate(Vector3 position, Quaternion rotation)
		{

			CalculatHandSide();
			ApplyArmModel();
			UpdateTransparency();


			transform.localPosition = wristPosition;
			transform.localRotation = wristRotation;
			Vector3 laservisual_offset = Vector3.zero;
			switch (IVRInput.GetConnectedControllers() & IVRInput.Controller.Remote)
			{

			case IVRInput.Controller.Remote_Ximmerse:
				laservisual_offset = POINTER_OFFSET_XIMMERS;
				break;
			case IVRInput.Controller.Remote_H1000:
				laservisual_offset = POINTER_OFFSET_H1000;

				break;
			default:
				break;
			}
			mLaserVisual.localPosition = laservisual_offset;
			mLaserVisual.localRotation = Quaternion.AngleAxis(pointerTiltAngle, Vector3.right);
		}
		/// Set The hand is Left or Right
		private void CalculatHandSide()
		{
			handedMultiplier.Set(0, 1, 1);
			if (handedness == HandSide.Right)
			{
				handedMultiplier.x = 1.0f;
			}
			else if (handedness == HandSide.Left)
			{
				handedMultiplier.x = -1.0f;
			}

		}

		private void ApplyArmModel()
		{
			Quaternion controllerOrientation = TrackPlugin.GetRemoteRotation();
			Vector3 resetarmposition = Vector3.zero;
			if (isLockedToHead)
			{
				resetarmposition = CalculatArmRoot(IVRManager.Instance.GetHeadPosition());
			}
			else
			{
				resetarmposition = mArmRootPosition;
			}
			// Get the relative positions of the joints.
			Matrix4x4 tt = Matrix4x4.TRS(resetarmposition, controllerOrientation, Vector3.one);
			Vector3 elbowPosition = resetarmposition + new Vector3(0.0f, addedElbowHeight, addedElbowDepth);
			elbowPosition = tt.MultiplyPoint(new Vector3(0,0,ElbowToHandLength));
			wristPosition = elbowPosition;
			wristRotation = controllerOrientation;
		}

		private Vector3 CalculatArmRoot(Vector3 headPosition)
		{
			Matrix4x4 matrix = Matrix4x4.TRS(headPosition,
				Quaternion.Euler(0, IVRManager.Instance.GetHeadRotation().eulerAngles.y, 0),Vector3.one);
			Quaternion headRotation = IVRManager.Instance.GetHeadRotation();
			return matrix.MultiplyPoint(Vector3.Scale(mArmRootPosition, handedMultiplier));
		}

		private void UpdateTransparency()
		{
			Vector3 wristRelativeToHead = wristPosition - IVRManager.Instance.GetHeadPosition();

			float animationDelta = DELTA_ALPHA * Time.deltaTime;
			float distToFace = Vector3.Distance(wristRelativeToHead, Vector3.zero);
			if (distToFace < fadeDistanceFromFaceToHand)
			{
				handAlpha = Mathf.Max(0.0f, handAlpha - animationDelta);
			}
			else
			{
				handAlpha = Mathf.Min(1.0f, handAlpha + animationDelta);
			}
		}

	}

}
