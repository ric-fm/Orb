using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{
	const float CHECK_TARGET_SQR_DISTANCE = 1;
	const float ORB_RADIUS = 0.1f;

	Transform defaultParent;

	public Vector3 directionToPlayer;

	public Vector3 contactPosition;
	public Vector3 contactNormal;

	Vector3 currentVelocity = Vector3.zero;

	public Vector2 teleportOffset = new Vector2(0.95f, 0.5f);

	Vector3 lastTeleportPosition = Vector3.zero;
	Vector3 lastTeleportContactPosition;


	void Start()
	{
		defaultParent = transform.parent;
	}

	void Update()
	{
		if (transform.parent != defaultParent)
		{
			transform.position += currentVelocity * Time.deltaTime;

			directionToPlayer = defaultParent.transform.position - transform.position;
		}
	}

	public void Reset()
	{
		currentVelocity = Vector3.zero;

		transform.SetParent(defaultParent);
		transform.localPosition = Vector3.zero;
	}

	public void Shoot(Vector3 direction, float speed)
	{
		currentVelocity = direction * speed;
		transform.SetParent(null);
	}

	bool NearToOwner()
	{
		return directionToPlayer.sqrMagnitude <= CHECK_TARGET_SQR_DISTANCE;
	}

	public bool Attract(Vector3 position, float speed)
	{
		if (NearToOwner())
		{
			Reset();
			return true;
		}

		// Check if there is an obstacle in the movement path to not attract
		RaycastHit hit;
		Debug.DrawLine(transform.position, transform.position + directionToPlayer.normalized * currentVelocity.magnitude);
		if (Physics.SphereCast(transform.position, 0.1f, directionToPlayer.normalized, out hit, 1, ~(1 << LayerMask.NameToLayer("Player"))))
		{
			return false;
		}

		// Unattach to collided wall if exists
		transform.SetParent(null);

		// Calculate velocity relative to player
		currentVelocity = Vector3.Lerp(currentVelocity, directionToPlayer.normalized * speed, Time.deltaTime);

		return false;
	}

	void OnCollisionEnter(Collision collision)
	{
		if (transform.parent != null)
		{
			return;
		}
		// Stop the movement
		currentVelocity = Vector3.zero;

		contactNormal = collision.contacts[0].normal;

		// Attach to collided wall and fix position
		transform.SetParent(collision.collider.gameObject.transform);
		transform.position -= contactNormal * collision.contacts[0].separation;

		// Calculate the contact point on collided wall face
		contactPosition = transform.position - contactNormal * ORB_RADIUS;
	}

	public bool CanTeleport()
	{
		// Can teleport if is attached to wall
		return transform.parent != null && transform.parent != defaultParent;
	}

	public void GetTeleportInfo(out Vector3 teleportPosition, out Vector3 contactPosition, out Vector3 teleportNormal)
	{
		contactPosition = transform.position - contactNormal * ORB_RADIUS;
		teleportPosition = transform.position;

		teleportNormal = contactNormal;

		if (transform.parent.tag == "PassablePlatform")
		{
			teleportNormal *= -1;
			Vector3 dirToWall = transform.parent.transform.position - contactPosition;
			contactPosition += teleportNormal * Vector3.Scale(dirToWall, teleportNormal).magnitude * 2;
		}

		teleportPosition = contactPosition + Vector3.Scale(new Vector3(teleportOffset.x, teleportOffset.y, teleportOffset.x), teleportNormal);
		lastTeleportPosition = teleportPosition;
		lastTeleportContactPosition = contactPosition;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(lastTeleportPosition, ORB_RADIUS);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(lastTeleportContactPosition, ORB_RADIUS);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(contactPosition, ORB_RADIUS);
		Gizmos.color = Color.white;
	}
}
