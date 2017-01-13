using UnityEngine;
using System.Collections;

public class BasicMovement : MonoBehaviour {
	public float movementSpeed = 2;

	new private Rigidbody2D rigidbody2D;

	void Start() {
		rigidbody2D = GetComponent<Rigidbody2D> ();
	}

	void FixedUpdate() {
		Vector2 movementDirection = Vector2.zero;
		if (Input.GetKey (KeyCode.W))
			movementDirection.y = 1;
		else if (Input.GetKey (KeyCode.S))
			movementDirection.y = -1;

		if (Input.GetKey (KeyCode.A))
			movementDirection.x = -1;
		else if (Input.GetKey (KeyCode.D))
			movementDirection.x = 1;

		rigidbody2D.velocity = movementDirection.normalized * movementSpeed;
	}
}
