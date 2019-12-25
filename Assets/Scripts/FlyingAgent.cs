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

		Plane verticalPlane = new Plane(transform.up, transform.position);
		Plane horizontalPlane = new Plane(transform.forward, transform.position);

		if (Input.GetKey(KeyCode.W))
		{
			// rotate to match vertically
			Vector3 planePoint = verticalPlane.ClosestPointOnPlane(target.position);
			Vector3 direction = (planePoint - transform.position).normalized;
			float signedAngle = Vector3.Angle(transform.right, direction);
			Debug.Log("angle: " + signedAngle);

			Quaternion rotation = Quaternion.AngleAxis(signedAngle * turningSpeed * Time.deltaTime, transform.right);
			transform.rotation = rotation * transform.rotation;

			//Debug.DrawLine(transform.position, planePoint);
		}

		if(Input.GetKey(KeyCode.D))
		{
			// rotate to match horizontally
			Vector3 planePoint = horizontalPlane.ClosestPointOnPlane(target.position);
			Vector3 direction = (planePoint - transform.position).normalized;
			float signedAngle = Vector3.Angle(direction, transform.forward);
			Debug.Log("angle: " + signedAngle);

			Quaternion rotation = Quaternion.AngleAxis(signedAngle * turningSpeed * Time.deltaTime, transform.forward);
			transform.rotation = rotation * transform.rotation;

			//Debug.DrawLine(transform.position, planePoint);
		}

		Vector3 verticalPlanePoint = verticalPlane.ClosestPointOnPlane(target.position);
		Debug.DrawLine(transform.position, verticalPlanePoint, Color.red);

		Vector3 horizontalPlanePoint = horizontalPlane.ClosestPointOnPlane(target.position);
		Debug.DrawLine(transform.position, horizontalPlanePoint, Color.blue);
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

	private void DrawPlane(Vector3 position, Vector3 normal)
	{
		Vector3 v3;

		if (normal.normalized != Vector3.forward)
			v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
		else
			v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;

		v3 *= 5f;

		var corner0 = position + v3;
		var corner2 = position - v3;
		var q = Quaternion.AngleAxis(90.0f, normal);
		v3 = q * v3;
		var corner1 = position + v3;
		var corner3 = position - v3;

		Gizmos.DrawLine(corner0, corner2);
		Gizmos.DrawLine(corner1, corner3);
		Gizmos.DrawLine(corner0, corner1);
		Gizmos.DrawLine(corner1, corner2);
		Gizmos.DrawLine(corner2, corner3);
		Gizmos.DrawLine(corner3, corner0);
		//Gizmos.DrawRay(position, normal, Color.red);
	}

	private void OnDrawGizmos()
	{
		DrawPlane(transform.position, transform.up);
		DrawPlane(transform.position, transform.right);
	}
}
