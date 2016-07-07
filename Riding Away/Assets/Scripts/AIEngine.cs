using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class AIEngine : MonoBehaviour {

	public Transform pathGroup;
	public float maxSteer = 15.0f;
	public WheelCollider wheelF;
	public WheelCollider wheelB;
	public int currentPathObj;
	public float distFromPath = 10.0f;
	public float maxTorque = 5.0f;
	public float currentSpeed;
	public float topSpeed = 50.0f;
	public float decelSpeed = 10.0f;
	public float sensorLength = 5.0f;
	public float frontSensorStartPoint = 5.0f;
	public float frontSensorSideDist = 5.0f;
	public float frontSensorAngle = 30.0f;
	public float sidewaySensorLength = 5.0f;
	public float sidewaySensorLengthR = 5.0f;
	public float avoidSpeed = 10.0f;
	public float upSensorStartPoint = 3.0f;
	public float draftSensorLength = 1.0f;
	public float draftAssist = 0.02f;

	private List<Transform> path; 
	private Animator anim;
	private int flag = 0;
	private Rigidbody rb;

	void Start () {
		GetPath();
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
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
		GetSteer ();
		Move ();
		Sensors ();
		Draft ();

	}

	void GetSteer () {
		Vector3 steerVector = transform.InverseTransformPoint(new Vector3(path[currentPathObj].position.x, transform.position.y, path[currentPathObj].position.z));
		float newSteer = maxSteer * (steerVector.x / steerVector.magnitude);
		//dir = steerVector.x / steerVector.magnitude;
		wheelF.steerAngle = newSteer;

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

		if (currentSpeed <= topSpeed) {
			wheelB.brakeTorque = 0;
			wheelB.motorTorque = maxTorque;
		} else {
			wheelB.brakeTorque = decelSpeed;
			wheelB.motorTorque = 0;
		}

		anim.speed = currentSpeed/topSpeed * 2;
	}

	void Sensors() {
		flag = 0;
		float avoidSensitivity = 0;
		Vector3 pos;
		RaycastHit hit;
		Vector3 rightAngle = Quaternion.AngleAxis (frontSensorAngle, transform.up) * transform.forward;
		Vector3 leftAngle = Quaternion.AngleAxis (-frontSensorAngle, transform.up) * transform.forward;

		pos = transform.position;
		pos += transform.forward * frontSensorStartPoint;

		//Braking Sensor
		if (Physics.Raycast (pos, transform.forward, out hit, sensorLength)) {
			flag++;
			wheelB.brakeTorque = decelSpeed;
			Debug.Log ("Braking");
			Debug.DrawLine (pos, hit.point, Color.red);
		} else {
			wheelB.brakeTorque = 0f;
		}


		//Front Right Sensor
		pos += transform.right * frontSensorSideDist;
		if (Physics.Raycast(pos,transform.forward, out hit, sensorLength) ) {
			if (hit.transform.tag != "track") {
				flag++;
				avoidSensitivity -= 0.2f;
				Debug.Log ("Avoiding RightF");
				Debug.DrawLine (pos, hit.point, Color.white);
			}
		} else if (Physics.Raycast(pos,rightAngle, out hit, sensorLength) ) {
			flag++;
			avoidSensitivity -= 0.5f;
			Debug.Log ("Avoiding RightFA");
			Debug.DrawLine (pos, hit.point, Color.white);
		}

		//Front Left Sensor
		pos = transform.position;
		pos += transform.forward * frontSensorStartPoint;
		pos -= transform.right * frontSensorSideDist;
		if (Physics.Raycast(pos,transform.forward, out hit, sensorLength) ) {
			if (hit.transform.tag != "track") {
				flag++;
				avoidSensitivity += 0.75f;
				Debug.Log ("Avoiding LeftF");
				Debug.DrawLine (pos, hit.point, Color.white);
			}		
		} else if (Physics.Raycast(pos,leftAngle, out hit, sensorLength) ) {
			flag++;
			avoidSensitivity += 0.25f;
			Debug.Log ("Avoiding LeftFA");
			Debug.DrawLine (pos, hit.point, Color.white);
		}

		// Right Sideway Sensor
		if (Physics.Raycast(transform.position,transform.right, out hit, sidewaySensorLengthR) ) {
			flag++;
			avoidSensitivity -= 0.75f;
			Debug.Log ("Avoiding Right");
			Debug.DrawLine (transform.position, hit.point, Color.white);
		}

		//Left Sideway Sensor
		if (Physics.Raycast(transform.position,-transform.right, out hit, sidewaySensorLength) ) {
			flag++;
			avoidSensitivity -= 0.25f;
			Debug.Log ("Avoiding Left");
			Debug.DrawLine (transform.position, hit.point, Color.white);
		}

		//Front Mid Sensor
		pos = transform.position;
		pos += transform.forward * frontSensorStartPoint;
		if (avoidSensitivity == 0) {
			if (Physics.Raycast(pos,transform.forward, out hit, sensorLength) ) {
				if (hit.transform.tag != "Rider") {
					if (hit.normal.x < 0) {
						avoidSensitivity = -1f;
					} else {
						avoidSensitivity = 1f;
					}
					Debug.DrawLine (pos, hit.point, Color.white);
					Debug.Log ("Avoiding Center");
				}
			}
		}

		if (flag != 0) {
			AvoidSteer (avoidSensitivity);
		}
	}

	void AvoidSteer(float sensitivity) {
		wheelF.steerAngle = maxSteer * sensitivity;
	}


	void Draft() {
		Vector3 pos;
        RaycastHit hit;

        pos = transform.position;
        pos += transform.forward * frontSensorStartPoint;
        pos += transform.up * upSensorStartPoint;

        //Drafting Sensor
        if (Physics.Raycast (pos, transform.forward, out hit, draftSensorLength)) {
            if (hit.transform.tag == "Rider") {
                wheelB.brakeTorque = decelSpeed;
                rb.drag = draftAssist;
                maxTorque += 5f;
                Debug.Log ("Drafting");
                Debug.DrawLine (pos, hit.point, Color.green);
            }
        }  else {
            rb.drag = 0.05f;
            Debug.Log ("Not Drafting");
        }
	}
}﻿