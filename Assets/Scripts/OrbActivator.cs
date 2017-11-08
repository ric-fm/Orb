using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbActivator : MonoBehaviour
{
	public Transform orbPosition;

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Orb")
		{
			collision.transform.position = orbPosition.position;
			Debug.Log("OrbActivator on");
		}
	}

	void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.tag == "Orb")
		{
			Debug.Log("OrbActivator off");
		}
	}
}
