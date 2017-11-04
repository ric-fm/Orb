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


		// Handle inertia and decrease input control when character is on air
		moveDirection = isGrounded ? inputDirection : Vector2.Lerp(moveDirection, inputDirection, airControlPercent);

		// Update character
		Move(moveDirection, isRunning);

		if (jump)
		{
			Jump();
		}
	}

	void Move(Vector2 direction, bool isRunning)
	{
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

	void OnDrawGizmos()
	{
		Gizmos.color = isGrounded? Color.green : Color.red;
		Gizmos.DrawLine(transform.position, transform.position - transform.up * GROUND_CHECK_DISTANCE);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + transform.forward);
		Gizmos.color = Color.white;
	}
}
