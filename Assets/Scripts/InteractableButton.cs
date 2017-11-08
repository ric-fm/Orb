using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SensorButton))]
public class InteractableButton : Interactable {

	SensorButton button;

	private void Start()
	{
		button = GetComponent<SensorButton>();
	}

	public override void Interact()
	{
		button.Push();
	}
}
