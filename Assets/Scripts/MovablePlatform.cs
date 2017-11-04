using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatform : MonoBehaviour
{
	const float CHECK_TARGET_SQR_DISTANCE = 0.05f * 0.05f;

	public Transform platform;
	public Transform pointA;
	public Transform pointB;

	public float speed = 1.0f;

	Transform target;

	void Start()
	{
		target = pointB;
	}

	void Update()
	{
		Vector3 direction = target.localPosition - platform.localPosition;
		Vector3 movement = direction.normalized * speed * Time.deltaTime;

		platform.localPosition += movement;

		if (direction.sqrMagnitude <= CHECK_TARGET_SQR_DISTANCE)
		{
			ChangeTarget();	
		}
	}

	void ChangeTarget()
	{
		if (target == pointA)
		{
			target = pointB;
		}
		else
		{
			target = pointA;
		}
	}
}
