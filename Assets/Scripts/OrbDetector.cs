using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbDetector : Sensor
{
	public Transform orbPosition;

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Orb")
		{
			collision.transform.position = orbPosition.position;

			SetSignal(true);
		}
	}

	void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.tag == "Orb")
		{
			SetSignal(false);
		}
	}
}
