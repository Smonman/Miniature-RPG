using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace MiniRPG
{
	public class CameraOrbit : MonoBehaviour
	{
		#region Variables
		[SerializeField]
		private Transform target;
		[SerializeField]
		private Vector3 offset;
		[SerializeField]
		private int minDistanceAway;
		[SerializeField]
		private int maxDistanceAway;
		[SerializeField]
		private float curDistanceAway;
		[SerializeField]
		private float zoomSpeed;
		[SerializeField]
		private float rotateSpeed;
		[SerializeField]
		private float smooth;

		private float curXRotation;
		private float curYRotation;
		[SerializeField]
		private float minXRotation;
		[SerializeField]
		private float maxXRotation;

		private const string orbitHorizontalSnap = "OrbitHorizontalSnap";
		private const string orbitHorizontal = "OrbitHorizontal";
		private const string orbitVertical = "OrbitVertical";
		private const string zoom = "Mouse ScrollWheel";

		private Camera cam;

		private float orbitVInput;
		private float orbitHInput;
		private float zoomInput;
		private float orbitHSnapInput;

		private Vector3 position;
		private Vector3 destination;

		private Vector2Int lastCursorPos;
		[DllImport("user32.dll")]
		static extern bool SetCursorPos(int x, int y);

		public delegate void OnCameraTargetChangedDelegate(Transform newTarget);
		public event OnCameraTargetChangedDelegate OnCameraTargetChanged;

		public Transform Target
		{
			get
			{
				return target;
			}

			set
			{
				target = value;
				if (OnCameraTargetChanged != null)
				{
					OnCameraTargetChanged(Target);
				}
			}
		}

		public Vector3 Offset
		{
			get
			{
				return offset;
			}

			set
			{
				offset = value;
			}
		}

		public int MinDistanceAway
		{
			get
			{
				return minDistanceAway;
			}

			set
			{
				minDistanceAway = value;
			}
		}

		public int MaxDistanceAway
		{
			get
			{
				return maxDistanceAway;
			}

			set
			{
				maxDistanceAway = value;
			}
		}

		public float CurDistanceAway
		{
			get
			{
				return curDistanceAway;
			}

			set
			{
				curDistanceAway = Mathf.Clamp(value, MinDistanceAway, MaxDistanceAway);
			}
		}

		public float ZoomSpeed
		{
			get
			{
				return zoomSpeed;
			}

			set
			{
				zoomSpeed = Mathf.Abs(value);
			}
		}

		public float RotateSpeed
		{
			get
			{
				return rotateSpeed;
			}

			set
			{
				rotateSpeed = value;
			}
		}

		public float Smooth
		{
			get
			{
				return smooth;
			}

			set
			{
				smooth = value;
			}
		}

		public float MinXRotation
		{
			get
			{
				return minXRotation;
			}

			set
			{
				minXRotation = value;
			}
		}

		public float MaxXRotation
		{
			get
			{
				return maxXRotation;
			}

			set
			{
				maxXRotation = value;
			}
		}
		#endregion

		void OnEnable()
		{
			OnCameraTargetChanged += CameraTargetChanged;
		}

		void OnDisable()
		{
			OnCameraTargetChanged -= CameraTargetChanged;
		}

		void Awake()
		{
			cam = GetComponent<Camera>();
			curXRotation = 0;
		}

		void Start()
		{

		}

		void CameraTargetChanged(Transform newTarget)
		{
			MoveToTarget();
			RotateCamera();
		}

		void GetInput()
		{
			if (Input.GetAxisRaw("Fire1") != 0 || Input.GetAxisRaw("Fire2") != 0)
			{
				orbitVInput = Input.GetAxisRaw(orbitVertical);
				orbitHInput = Input.GetAxisRaw(orbitHorizontal);
				//lastCursorPos = Vector2Int.RoundToInt(Input.mousePosition);
				Cursor.lockState = CursorLockMode.Locked;
			}
			else
			{
				orbitVInput = 0;
				orbitHInput = 0;
				Cursor.lockState = CursorLockMode.None;
				// Does not work! The cursor position has to be set when the mouse buttons are released.
				//Debug.Log(lastCursorPos);
				//SetCursorPos(lastCursorPos.x, lastCursorPos.y);
			}

			orbitHSnapInput = Input.GetAxisRaw(orbitHorizontalSnap);

			zoomInput = Input.GetAxisRaw(zoom);
		}

		void ZoomInOnTarget()
		{
			CurDistanceAway += -zoomInput * ZoomSpeed;
		}

		void OrbitTarget()
		{
			if (orbitHSnapInput > 0)
			{
				curYRotation = -180;
			}

			curXRotation += -orbitVInput * RotateSpeed * Smooth * Time.deltaTime;
			curYRotation += orbitHInput * RotateSpeed * Smooth * Time.deltaTime;

			curXRotation = Mathf.Clamp(curXRotation, MinXRotation, MaxXRotation);
		}

		void MoveToTarget()
		{
			position = Target.transform.position + Offset;
			destination = Quaternion.Euler(curXRotation, curYRotation, 0) * -Vector3.forward * curDistanceAway;
			destination += position;
			cam.transform.position = destination;
		}

		void RotateCamera()
		{
			Quaternion targetRotation = Quaternion.LookRotation(Target.transform.position + Offset - cam.transform.position);
			cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, targetRotation, Smooth * Time.deltaTime);
		}

		void FixedUpdate()
		{
			GetInput();
			OrbitTarget();

			MoveToTarget();
			RotateCamera();

			ZoomInOnTarget();

			if (Input.GetButton("Fire2"))
			{
				Target.transform.localEulerAngles = new Vector3(0, cam.transform.localRotation.eulerAngles.y, 0);
			}
		}
	}
}