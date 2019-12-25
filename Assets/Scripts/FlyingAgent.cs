using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingAgent : MonoBehaviour
{
	[SerializeField] Transform target;
	[Range(0,10)]
	[SerializeField] float speed;
	[Range(0, 10)]
	[SerializeField] float turningSpeed;
	Rigidbody rigidBody;

	bool thrustActive = false;

	[SerializeField] GameObject thrustIndicator;

	private void Awake()
	{
		rigidBody = GetComponent<Rigidbody>();
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		thrustActive = Input.GetKey(KeyCode.Space);
		thrustIndicator.SetActive(thrustActive);

		if(Input.GetKeyDown(KeyCode.R))
		{
			rigidBody.MovePosition(Vector3.zero);
			rigidBody.velocity = Vector3.zero;
			rigidBody.angularVelocity = Vector3.zero;
		}

		if(Input.GetKey(KeyCode.W))
		{
			// rotate to match vertically
			Vector3 planePoint = Vector3.ProjectOnPlane(target.position, transform.right);
			Vector3 direction = (planePoint - transform.position).normalized;
			float signedAngle = Vector3.Angle(transform.right, direction);
			Debug.Log("angle: " + signedAngle);

			Quaternion rotation = Quaternion.AngleAxis(signedAngle * turningSpeed * Time.deltaTime, transform.right);
			transform.rotation *= rotation;
		}

		if(Input.GetKey(KeyCode.D))
		{
			// rotate to match horizontally
			Vector3 planePoint = Vector3.ProjectOnPlane(target.position, transform.forward);
			Vector3 direction = (planePoint - transform.position).normalized;
			float signedAngle = Vector3.Angle(direction, transform.forward);
			Debug.Log("angle: " + signedAngle);

			Quaternion rotation = Quaternion.AngleAxis(signedAngle * turningSpeed * Time.deltaTime, transform.forward);
			transform.rotation *= rotation;

			//Debug.DrawLine(transform.position, planePoint);
		}

		Vector3 targetInVerticalPlane = Vector3.ProjectOnPlane(target.position, transform.right);
		Debug.DrawLine(transform.position, targetInVerticalPlane);

		Vector3 targetInHorizontalPlane = Vector3.ProjectOnPlane(target.position, transform.forward);
		Debug.DrawLine(transform.position, targetInHorizontalPlane);
	}

	// tells us if we need to bank to reach our target
	bool TargetInVerticalPlane(Vector3 targetPosition)
	{
		Plane verticalPlane = new Plane(transform.right, transform.position);
		return Mathf.Abs(verticalPlane.GetDistanceToPoint(targetPosition)) < 0.01f;
	}

	// tells us if we need to climb to reach our target
	bool TargetInHorizontalPlane(Vector3 targetPosition)
	{
		Plane verticalPlane = new Plane(transform.up, transform.position);
		return Mathf.Abs(verticalPlane.GetDistanceToPoint(targetPosition)) < 0.01f;
	}

	private void FixedUpdate()
	{
		thrustActive = speed > 0;

		Vector3 thrustVelocity = transform.forward * speed * Time.fixedDeltaTime;

		if(thrustActive) rigidBody.AddForce(thrustVelocity);
	}
}
