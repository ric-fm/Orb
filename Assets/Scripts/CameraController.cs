using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public float turnSpeed = 200.0f;
	public Vector2 pitchMinMax = new Vector2(-45f, 80f);

	public Transform body;
	public Camera camera;

	float pitch = 0;

	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update () {
		Vector2 lookDirection = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		RotateCharacter(lookDirection);
	}

	void RotateCharacter(Vector2 direction)
	{
		body.transform.Rotate(Vector3.up * direction.x * turnSpeed * Time.deltaTime);

		pitch -= direction.y * turnSpeed * Time.deltaTime;
		pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
		camera.transform.eulerAngles = new Vector3(pitch, camera.transform.eulerAngles.y, camera.transform.eulerAngles.z);
	}
}
