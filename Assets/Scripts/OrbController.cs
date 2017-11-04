using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{

	const float CHECK_TARGET_SQR_DISTANCE = 1 * 1;

	public float teleportDistance = 1.7f;


	Transform owner;

	Vector3 currentVelocity = Vector3.zero;

	Vector3 hitNormal = Vector3.zero;

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
		Debug.DrawRay(transform.position, hitNormal, Color.blue, 5f);

		transform.SetParent(collision.collider.gameObject.transform);
		transform.position -= hitNormal * collision.contacts[0].separation;
	}

	public Vector3 GetTeleportPosition()
	{
		Vector3 position = transform.position;

		position += hitNormal * teleportDistance;

		return position;
	}
}
