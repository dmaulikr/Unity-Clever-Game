using UnityEngine;
using System.Collections;

public class MoveMessage : MonoBehaviour {
	
	void Start () {
		transform.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, 60);
		Destroy (gameObject, 2.5f);
	}
}
