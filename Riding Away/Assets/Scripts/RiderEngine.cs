using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class RiderEngine : MonoBehaviour {

	public Transform pathGroup;
	public float maxSteer = 15.0f;
	public WheelCollider wheelF;
	public WheelCollider wheelB;
	public int currentPathObj;
	public float distFromPath = 10.0f;
	public float maxTorque = 5.0f;
	public float sprintTorque = 10.0f;
	public static float currentSpeed;
	public float topSpeed = 50.0f;
	public float sprintSpeed = 60.0f;
	public float decelSpeed = 10.0f;
	public float steerPad = 2.0f;


	private List<Transform> path; 
	private Animator anim;
	private Rigidbody rb;
	private Health health;


	void Start () {
		GetPath();
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		health = GetComponent<Health>();
		float eulerZ = transform.rotation.eulerAngles.z;
		eulerZ = 50f;
	}

	void GetPath () {
		Transform[] path_objs = pathGroup.GetComponentsInChildren<Transform>();
		path = new List<Transform>();

		for (int i = 0; i < path_objs.Length; i++)
		{
			if (path_objs[i] != pathGroup)
			{
				path.Add(path_objs[i]);
			}
		}
	}

	void Update () {
		GetSteer();
		Move();
		anim.speed = currentSpeed/topSpeed * 2;

	}

	void GetSteer () {
		Vector3 steerVector = transform.InverseTransformPoint(new Vector3(path[currentPathObj].position.x, transform.position.y, path[currentPathObj].position.z));

		float newSteer = maxSteer * (steerVector.x / steerVector.magnitude);
		//dir = steerVector.x / steerVector.magnitude;
		wheelF.steerAngle = newSteer;

		//For human Rider
		if (Input.GetKey(KeyCode.RightArrow) ) {
			wheelF.steerAngle = wheelF.steerAngle + steerPad;
		}
		if (Input.GetKey(KeyCode.LeftArrow) ) {
			wheelF.steerAngle = wheelF.steerAngle - steerPad;
		}

		if (steerVector.magnitude <= distFromPath) {
			currentPathObj++;
			}
		if (currentPathObj >= path.Count) {
			currentPathObj = 0;
			}		
	}

	void Move() {
		currentSpeed = 2 * (22/7) * wheelB.radius * wheelB.rpm * 60/1000;
		currentSpeed = Mathf.Round(currentSpeed);

		if (Input.GetKey(KeyCode.UpArrow) &&  (currentSpeed <= sprintSpeed)) {
			wheelB.brakeTorque = 0;
			wheelB.motorTorque = sprintTorque;
			health.Fatigue(0.0125f);

		} else if (currentSpeed <= topSpeed) {
			wheelB.brakeTorque = 0;
			wheelB.motorTorque = maxTorque;
			health.Fatigue(-0.0025f);
		} else {
			wheelB.brakeTorque = decelSpeed;
			wheelB.motorTorque = 0;
		}

	}


	void Crash() {
		
		print(rb.angularVelocity);
		if (rb.angularVelocity.magnitude > 2 || rb.angularVelocity.magnitude < -2){
			anim.SetTrigger ("isCrashing");
		}
	}
}﻿