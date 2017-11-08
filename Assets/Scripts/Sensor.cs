using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour {

	public List<Activator> activators;

	bool signal;
	public bool invert;

	protected virtual void Start()
	{
		for (int i = 0; i < activators.Count; i++)
		{
			activators[i].InitState(signal ^ invert);
		}
	}

	protected void SetSignal(bool value)
	{
		if(signal != value)
		{
			OnSignalChanged(value ^ invert);
			signal = value;
		}
	}

	protected void ToggleSignal()
	{
		SetSignal(!signal);
	}

	protected virtual void OnSignalChanged(bool value)
	{
		for (int i = 0; i < activators.Count; i++)
		{
			activators[i].SetState(value);
		}
	}
}
