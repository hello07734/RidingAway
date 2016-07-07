using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LapCounter : MonoBehaviour {

	public static int lap = 0;
	public Text lapText;
	public Text speed;
	public Text healthText;
	public GameObject Rider;

	private Rigidbody rb;
	private Health health;
	
	void Start() {
		Reset();
		lapText.text = "Lap: " + lap;
		rb = Rider.GetComponent<Rigidbody>();
		health = Rider.GetComponent<Health> ();
		}

	void Update() {
		float cSpeed = rb.velocity.magnitude * 2;
		cSpeed = Mathf.Round (cSpeed);
		speed.text = cSpeed.ToString() + " MPH";
		int healthLev = (int) health.healthLev;
		healthText.text = "Health: " + healthLev.ToString ();

	}			
	
	void OnTriggerEnter(Collider LapCounter) {
		if (LapCounter.gameObject == (Rider)) {
			lap ++;
			lapText.text = "Lap: " + lap;
		}
	}

	public static void Reset() {
		lap = 0;

	}
}
