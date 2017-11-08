using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

	const float GROUND_CHECK_DISTANCE = 0.2f;
	const float WALL_CHECK_DISTANCE = 1f;

	public float walkSpeed = 2.0f;
	public float runSpeed = 5.0f;

	public float speedSmoothTime = 0.2f;

	[Range(0.0f, 1.0f)]
	public float airControlPercent = 0.05f;

	public float gravity = -12.0f;
	public float maxVelocityY = 48.0f;

	public float jumpHeight = 1.0f;

	public float slopeLimit = 45.0f;

	float currentSpeed;
	float speedSmoothVelocity;
	float velocityY;
	Vector2 moveDirection;

	bool isGrounded;

	public Camera camera;

	public Transform orbPoint;
	public OrbController orb;
	public float shootSpeed = 10.0f;
	public float attractSpeed = 5.0f;

	bool haveOrb = true;

	public Transform grabCheck;

	CharacterController characterController;

	Transform defaultParent;
	bool grabbedToWall;

	public Animator animator;

	public InverseKinematics leftHandIK;

	bool canMove = true;

	void Start()
	{
		characterController = GetComponent<CharacterController>();
		defaultParent = transform.parent;

		leftHandIK.enabled = false;
	}

	void Update()
	{
		// Check input
		Vector2 inputDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

		Vector2 lookDirection = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		bool jump = Input.GetButtonDown("Jump");

		bool fire1 = Input.GetButtonDown("Fire1");
		bool fire2 = Input.GetButton("Fire2");
		bool resetOrb = Input.GetButtonDown("ResetOrb");

		bool grabDown = Input.GetButtonDown("Grab");
		bool grabUp = Input.GetButtonUp("Grab");
		bool grab = Input.GetButton("Grab");

		if (haveOrb)
		{
			if (fire1)
			{
				ShootOrb();
			}
		}
		else
		{
			if (fire1 && orb.CanTeleport())
			{
				Teleport(grab);
			}

			if (fire2 && !haveOrb)
			{
				AttractOrb();
			}

			if (resetOrb)
			{
				ResetOrb();
			}
		}

		animator.SetBool("Attracting", fire2 && !haveOrb);

		if (grabDown)
		{
			GrabToWall();
		}
		if (grabUp && grabbedToWall)
		{
			UngrabWall();
		}

		// Update character
		if (canMove)
		{
			Move(inputDirection, true);
		}

		if (jump && isGrounded)
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
		velocityY = Mathf.Clamp(velocityY, -maxVelocityY, maxVelocityY);

		// Reset velocity to avoid "speed accumulation by gravity"
		if (grabbedToWall)
		{
			velocityY = 0;
		}

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
		if (isGrounded && !grabbedToWall && hit.collider.gameObject.tag == "MovablePlatform")
		{
			transform.parent = hit.collider.gameObject.transform;
		}
		else if (!grabbedToWall)
		{
			transform.parent = defaultParent;
		}

		// Move character
		Vector3 movement = direction * speed + Vector3.up * velocityY;
		characterController.Move(movement * Time.deltaTime);

		// Reset velocity to avoid "speed accumulation by gravity"
		if (isGrounded || grabbedToWall)
		{
			velocityY = 0;
		}
	}

	void Jump()
	{
		velocityY = Mathf.Sqrt(-2 * gravity * jumpHeight);
	}

	void ShootOrb()
	{
		Vector3 shootDirection = camera.transform.forward;

		//Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		Ray ray = new Ray(camera.transform.position, camera.transform.forward);
		Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow, 2f);


		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo))
		{
			animator.SetTrigger("Shoot");
			shootDirection = (hitInfo.point - orb.transform.position).normalized;
			orb.Shoot(shootDirection, shootSpeed);
			haveOrb = false;
		}
	}

	void AttractOrb()
	{
		haveOrb = orb.Attract(orbPoint.position, attractSpeed);
	}

	void ResetOrb()
	{
		orb.Reset();
		haveOrb = true;
	}

	void Teleport(bool grabOrbWall)
	{
		GameObject wall = orb.transform.parent.gameObject;

		Vector3 teleportPosition = Vector3.zero;
		Vector3 teleportContactPosition = Vector3.zero;
		Vector3 teleportNormal = Vector3.zero;

		orb.GetTeleportInfo(out teleportPosition, out teleportContactPosition, out teleportNormal);

		Vector3 newPosition = teleportPosition - Vector3.up * grabCheck.localPosition.y;
		transform.position = newPosition;

		// If grab input is on, grab the wall orb is attached
		if (grabOrbWall)
		{
			UngrabWall();

			//if (teleportNormal.y < 0.5)
			if (Mathf.Abs(teleportNormal.y) < 0.5)
			{
				GrabToWall(wall, teleportContactPosition, teleportNormal);
			}
		}

		haveOrb = true;
		orb.Reset();
	}

	bool GrabToWall()
	{
		RaycastHit hit;
		if (Physics.Raycast(grabCheck.position, transform.forward, out hit, WALL_CHECK_DISTANCE))
		{
			GameObject wall = hit.collider.gameObject;

			if (Physics.Raycast(grabCheck.position, -hit.normal, out hit, WALL_CHECK_DISTANCE))
			{
				if (wall == hit.collider.gameObject)
				{
					GrabToWall(wall, hit.point, hit.normal);

					return true;
				}
			}
		}
		return false;
	}

	void GrabToWall(GameObject wall, Vector3 position, Vector3 normal)
	{
		// Attach player to wall
		transform.parent = wall.transform;

		// IK management
		leftHandIK.enabled = true;
		leftHandIK.target.parent = wall.transform;
		leftHandIK.target.position = position;
		leftHandIK.enabled = true;

		// Calculate IK target rotation relative to the wall face normal
		// (1, 0, 0) => 0�
		// (0, 0, -1) => 90�
		// (0, 0, 1) => -90�
		// (-1, 0, 0) => -180� || 180�
		Vector3 rot = leftHandIK.target.eulerAngles;
		float x = 90 * (-1 + normal.x) * (1 - Mathf.Abs(normal.z));
		float y = 90 * -normal.z;
		rot.y = x + y;

		leftHandIK.target.eulerAngles = rot;

		grabbedToWall = true;
		canMove = false;
	}

	void UngrabWall()
	{
		transform.parent = defaultParent;
		grabbedToWall = false;
		canMove = true;
		leftHandIK.enabled = false;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = isGrounded ? Color.green : Color.red;
		Gizmos.DrawLine(transform.position, transform.position - transform.up * GROUND_CHECK_DISTANCE);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + transform.forward);
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(camera.transform.position, camera.transform.position + camera.transform.forward);
		Gizmos.color = Color.white;
	}
}
