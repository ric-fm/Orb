using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	const float GROUND_CHECK_DISTANCE = 0.2f;

	public float walkSpeed = 2.0f;
	public float runSpeed = 5.0f;

	public float speedSmoothTime = 0.2f;

	[Range(0.0f, 1.0f)]
	public float airControlPercent = 0.05f;

	public float gravity = -12.0f;

	public float jumpHeight = 1.0f;

	public float slopeLimit = 45.0f;

	float currentSpeed;
	float speedSmoothVelocity;
	float velocityY;
	Vector2 moveDirection;

	bool isGrounded;

	public Transform cameraTransform;

	public Transform orbPoint;
	public OrbController orb;
	public float shootSpeed = 10.0f;
	public float attractSpeed = 5.0f;

	bool haveOrb = true;

	CharacterController characterController;

	void Start () {
		characterController = GetComponent<CharacterController>();
	}

	void Update()
	{
		// Check input
		Vector2 inputDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

		Vector2 lookDirection = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		bool isRunning = Input.GetAxis("Run") != 0;

		bool jump = Input.GetButtonDown("Jump");

		bool fire1 = Input.GetButtonDown("Fire1");
		bool fire2 = Input.GetButton("Fire2");
		bool resetOrb = Input.GetButtonDown("ResetOrb");
		bool teleport = Input.GetButtonDown("Teleport");

		if (fire1)
		{
			ShootOrb();
		}

		if(resetOrb)
		{
			ResetOrb();
		}
		if(teleport)
		{
			Teleport();
		}

		if(fire2)
		{
			AttractOrb();
		}

		// Update character
		Move(inputDirection, isRunning);

		if (jump)
		{
			Jump();
		}
	}

	void Move(Vector2 direction, bool isRunning)
	{
		// Handle inertia and decrease input control when character is on air
		moveDirection = isGrounded ? direction : Vector2.Lerp(moveDirection, direction, airControlPercent);

		Vector3 desiredDirection = transform.forward * direction.y + transform.right * direction.x;
		float targetSpeed = (isRunning ? runSpeed : walkSpeed) * direction.magnitude;

		MoveCharacter(desiredDirection, targetSpeed);
	}

	void MoveCharacter(Vector3 direction, float speed)
	{
		// Smooth speed
		currentSpeed = Mathf.SmoothDamp(currentSpeed, speed, ref speedSmoothVelocity, speedSmoothTime);

		// Gravity
		velocityY += gravity * Time.deltaTime;

		// Check OnGround-OnAir
		RaycastHit hit;
		if (Physics.Raycast(transform.position, Vector3.down, out hit, GROUND_CHECK_DISTANCE) && velocityY < 0.0f)
		{
			isGrounded = true;

			// Check slope limit and project on plane
			float groundAngle = Vector3.Angle(Vector3.up, hit.normal);
			if (groundAngle < characterController.slopeLimit)
			{
				direction = Vector3.ProjectOnPlane(direction, hit.normal);
			}
		}
		else
		{
			isGrounded = false;
		}

		// Check OnMovablePlatform
		if (isGrounded && hit.collider.gameObject.tag == "MovablePlatform")
		{
			transform.parent = hit.collider.gameObject.transform;
		}
		else
		{
			transform.parent = null;
		}

		// Move character
		Vector3 movement = direction * speed + Vector3.up * velocityY;
		characterController.Move(movement * Time.deltaTime);

		// Reset velocity to avoid "speed accumulation by gravity"
		if (isGrounded)
		{
			velocityY = 0;
		}
	}

	void Jump()
	{
		if (isGrounded)
		{
			velocityY = Mathf.Sqrt(-2 * gravity * jumpHeight);
		}
	}

	void ShootOrb()
	{
		if(haveOrb)
		{
			orb.Shoot(cameraTransform.forward, shootSpeed);
			haveOrb = false;
		}
	}

	void AttractOrb()
	{
		if(!haveOrb)
		{
			haveOrb = orb.Attract(orbPoint.position, attractSpeed);
		}
	}

	void ResetOrb()
	{
		if(!haveOrb)
		{
			orb.Reset();
			haveOrb = true;
		}
	}

	void Teleport()
	{
		if(!haveOrb)
		{
			Vector3 orbPosition = orb.transform.position;
			orb.Reset();
			haveOrb = true;
			transform.position = orbPosition;
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = isGrounded? Color.green : Color.red;
		Gizmos.DrawLine(transform.position, transform.position - transform.up * GROUND_CHECK_DISTANCE);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + transform.forward);
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(cameraTransform.position, cameraTransform.position + cameraTransform.forward);
		Gizmos.color = Color.white;
	}
}
