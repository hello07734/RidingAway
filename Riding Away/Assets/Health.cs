using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public float healthLev = 100f;



	public void Fatigue (float damage) {
		if ((healthLev -= damage) > 0) {
			healthLev -= damage;
		} else {
			Bonk();
		}
	}

	public void Bonk() {

	}

}
