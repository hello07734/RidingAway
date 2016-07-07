using UnityEngine;
using System.Collections;

public class AIWheelScript : MonoBehaviour {

	public WheelCollider myWheelCollider;

	void Start () {
	
	}
	


	void Update () {
		transform.Rotate(myWheelCollider.rpm/60 * -360 * Time.deltaTime,0,0);

	}
}
