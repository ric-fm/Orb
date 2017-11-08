using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour {

	bool isActive;
	public bool IsActive
	{
		get { return isActive; }
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			Debug.Log("Activate");
			isActive = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			Debug.Log("Deactivate");
			isActive = false;
		}
	}
}
