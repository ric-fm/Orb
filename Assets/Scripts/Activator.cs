using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour {
	bool state;

	public void InitState(bool value)
	{
		this.state = value;
		OnStateChange(value);
	}

	public void SetState(bool value)
	{
		if (value != state)
		{
			OnStateChange(value);
			state = value;
		}
	}

	public virtual void OnStateChange(bool newState)
	{

	}
}
