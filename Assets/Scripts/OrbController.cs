using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour {

	const float CHECK_TARGET_SQR_DISTANCE = 0.05f * 0.05f;

	Transform owner;

	Vector3 currentDirection = Vector3.zero;
	float currentSpeed = 0;

	void Start()
	{
		owner = transform.parent;
	}

	void Update()
	{
		transform.position += currentDirection * currentSpeed * Time.deltaTime;
	}

	public void Reset()
	{
		currentDirection = Vector3.zero;
		transform.parent = owner;
		transform.localPosition = Vector3.zero;
	}

	public void Shoot(Vector3 direction, float speed)
	{
		currentDirection = direction;
		currentSpeed = speed;
		transform.SetParent(null);
	}

	public bool Attract(Vector3 position, float speed)
	{
		transform.SetParent(null);
		Vector3 direction = position - transform.position;
		currentSpeed = speed;

		if (direction.sqrMagnitude <= CHECK_TARGET_SQR_DISTANCE)
		{
			transform.SetParent(owner);
			return true;
		}

		transform.position += direction.normalized * currentSpeed * Time.deltaTime;

		return false;
	}

	void OnCollisionEnter(Collision collision)
	{
		if(transform.parent != null)
		{
			return;
		}
		currentDirection = Vector3.zero;

		transform.position = collision.contacts[0].point;
		transform.SetParent(collision.collider.gameObject.transform);

	}
}
