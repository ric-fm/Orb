using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{

	const float CHECK_TARGET_SQR_DISTANCE = 1;

	Transform owner;

	Vector3 currentVelocity = Vector3.zero;

	Vector3 hitNormal = Vector3.zero;

	public Vector2 teleportOffset = new Vector2(0.95f, 0.5f);

	void Start()
	{
		owner = transform.parent;
	}

	void Update()
	{
		transform.position += currentVelocity * Time.deltaTime;
	}

	public void Reset()
	{
		currentVelocity = Vector3.zero;
		transform.parent = owner;
		transform.localPosition = Vector3.zero;
		hitNormal = Vector3.zero;
	}

	public void Shoot(Vector3 direction, float speed)
	{
		currentVelocity = direction * speed;
		transform.SetParent(null);
	}

	public bool Attract(Vector3 position, float speed)
	{
		transform.SetParent(null);
		Vector3 direction = position - transform.position;

		if (direction.sqrMagnitude <= CHECK_TARGET_SQR_DISTANCE)
		{
			Reset();
			return true;
		}

		Vector3 desiredVelocity = direction.normalized * speed;
		currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, Time.deltaTime);

		return false;
	}

	void OnCollisionEnter(Collision collision)
	{
		if (transform.parent != null)
		{
			return;
		}
		currentVelocity = Vector3.zero;
		hitNormal = collision.contacts[0].normal;

		transform.SetParent(collision.collider.gameObject.transform);
		transform.position -= hitNormal * collision.contacts[0].separation;
	}

	Vector3 lastTeleportPosition = Vector3.zero;
	public Vector3 GetTeleportPosition()
	{
		Vector3 position = transform.position;
		Vector3 teleportNormal = hitNormal;

		if (transform.parent != null)
		{
			if (transform.parent.tag == "PassablePlatform")
			{
				teleportNormal = -hitNormal;
				Vector3 difference = transform.parent.transform.position - position;
				Vector3 offset = Vector3.Scale(difference, hitNormal);
				float dist = offset.magnitude;

				position += teleportNormal * dist * 2;
			}
			position += Vector3.Scale(new Vector3(teleportOffset.x, teleportOffset.y, teleportOffset.x), teleportNormal);

		}
		lastTeleportPosition = position;

		return position;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawSphere(lastTeleportPosition, 0.2f);
	}
}
