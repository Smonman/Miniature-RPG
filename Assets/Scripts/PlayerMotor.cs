using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniRPG
{

	// TODO

	public enum PlayerState
	{
		Standing, Walking, Jumping, Falling, Flying
	};

	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(SphereCollider))]
	[RequireComponent(typeof(Player))]
	public class PlayerMotor : MonoBehaviour
	{
		#region Variables
		[SerializeField]
		private bool canRecieveInput;
		[SerializeField]
		private float walkingSpeed;
		[SerializeField]
		private float smooth;
		[SerializeField]
		private float airbornMultiplier;
		[SerializeField]
		private float maxSlope;
		[SerializeField]
		private float playerSlopeRayLenght = 0.1f;
		private Vector3 groundVector;
		private Vector3 groundVectorRight;
		[SerializeField]
		private bool isGrounded;
		[SerializeField]
		private LayerMask ground;
		[SerializeField]
		private float jumpForce;
		[SerializeField]
		private PlayerState curPlayerState;
		[SerializeField]
		private float rotationSpeed;

		private float playerStateDetectionThreshold = 0.03f;

		private Rigidbody rb;

		[SerializeField]
		private Player player;

		private float curRotation;

		public delegate void OnIsGroundedChangedDelegate(Creature player, bool isCurGrounded);
		public event OnIsGroundedChangedDelegate OnIsGroundedChanged;

		public delegate void OnPlayerStateChangedDelegate(Creature player, PlayerState curPlayerState_);
		public event OnPlayerStateChangedDelegate OnPlayerStateChanged;

		public bool CanRecieveInput
		{
			get
			{
				return canRecieveInput;
			}

			set
			{
				canRecieveInput = value;
			}
		}

		public float WalkingSpeed
		{
			get
			{
				return walkingSpeed;
			}

			set
			{
				walkingSpeed = Mathf.Abs(value);
			}
		}

		public float MaxSlope
		{
			get
			{
				return maxSlope;
			}

			set
			{
				maxSlope = value;
			}
		}

		public bool IsGrounded
		{
			get
			{
				return isGrounded;
			}

			private set
			{
				isGrounded = value;
				if (OnIsGroundedChanged != null)
				{
					OnIsGroundedChanged(player, IsGrounded);
				}
			}
		}

		public PlayerState CurPlayerState
		{
			get
			{
				return curPlayerState;
			}

			set
			{
				curPlayerState = value;
				if (OnPlayerStateChanged != null)
				{
					OnPlayerStateChanged(player, CurPlayerState);
				}
			}
		}

		public float JumpForce
		{
			get
			{
				return jumpForce;
			}

			set
			{
				jumpForce = Mathf.Abs(value);
			}
		}

		public float RotationSpeed
		{
			get
			{
				return rotationSpeed;
			}

			set
			{
				rotationSpeed = value;
			}
		}
		#endregion

		void OnEnable()
		{

		}

		void OnDisable()
		{

		}

		void Awake()
		{
			rb = GetComponent<Rigidbody>();
			player = GetComponent<Player>();
			CurPlayerState = PlayerState.Standing;
		}

		void Start()
		{

		}

		void Update()
		{

		}

		void FixedUpdate()
		{
			if (CanRecieveInput)
			{
				Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
				UpdatePlayerState(movement);

				RaycastHit hit;
				if (Physics.Raycast(transform.position + Vector3.up * playerStateDetectionThreshold, Vector3.down, out hit, playerSlopeRayLenght, ground))
				{
					Debug.DrawRay(transform.position + Vector3.up * playerStateDetectionThreshold, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
					Debug.DrawRay(hit.point, hit.normal, Color.blue);

					Vector3 perpenticular = Vector3.Cross(hit.normal, Vector3.left);
					Debug.DrawRay(hit.point, perpenticular, Color.green);

					groundVector = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, hit.normal) * perpenticular;
					Debug.DrawRay(hit.point, groundVector, Color.red);

					groundVectorRight = Quaternion.AngleAxis(90, hit.normal) * groundVector;
					Debug.DrawRay(hit.point, groundVectorRight, Color.red);
				}

				// calculate the velocty
				Vector3 velTemp = groundVector * movement.z * WalkingSpeed * Time.fixedDeltaTime;
				velTemp += groundVectorRight * movement.x * WalkingSpeed * Time.fixedDeltaTime;
				velTemp.y = rb.velocity.y;

				// change responsibility if player is in the air
				if (!isGrounded)
				{
					velTemp.x *= airbornMultiplier;
					velTemp.z *= airbornMultiplier;
				}

				// Set velocity to rigidbody and lerp it
				rb.velocity = Vector3.Lerp(rb.velocity, velTemp, smooth * Time.fixedDeltaTime);

				// Jump
				if (Input.GetButtonDown("Jump"))
				{
					if (IsGrounded)
					{
						rb.AddForce(Vector3.up * jumpForce, ForceMode.Force);
					}
				}
			}
		}

		private void UpdatePlayerState(Vector3 movement)
		{
			if (rb.velocity.y > 0 + playerStateDetectionThreshold)
			{
				CurPlayerState = PlayerState.Jumping;
			}
			else if (Mathf.Abs(rb.velocity.y) > 0 + playerStateDetectionThreshold)
			{
				CurPlayerState = PlayerState.Falling;
			}
			else if (rb.velocity.y == 0)
			{
				if (movement.magnitude <= 0)
				{
					CurPlayerState = PlayerState.Standing;
				}
				else
				{
					CurPlayerState = PlayerState.Walking;
				}
			}
		}

		void OnTriggerStay(Collider collider)
		{
			// https://answers.unity.com/questions/8715/how-do-i-use-layermasks.html
			int layerIndex = collider.transform.gameObject.layer;
			if ((ground & 1 << layerIndex) == 1 << layerIndex)
			{
				// check for fall damage
				IsGrounded = true;
			}
		}

		void OnTriggerExit(Collider collider)
		{
			int layerIndex = collider.transform.gameObject.layer;
			if ((ground & 1 << layerIndex) == 1 << layerIndex)
			{
				IsGrounded = false;
			}
		}
	}
}
