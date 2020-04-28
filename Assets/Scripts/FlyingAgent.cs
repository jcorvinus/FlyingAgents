using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CatchCo;

public class FlyingAgent : MonoBehaviour
{
	[SerializeField] Transform target;
	[Range(0,10)]
	[SerializeField] float speed;
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

	/*Vector3 GetVerticalPlaneNormal()
	{
		return transform.right;
	}

	Vector3 GetVerticalRotationAxis()
	{
		return transform.forward;
	}

	Vector3 GetHorizontalPlaneNormal()
	{
		return transform.up;
	}

	public Vector3 GetHorizontalRotationAxis()
	{
		return transform.forward;
	}

	void DoSeek(Plane plane, Vector3 rotateAxis)
	{
		Vector3 planePoint = plane.ClosestPointOnPlane(target.position);
		Vector3 direction = (planePoint - transform.position).normalized;
		Vector3 rotationAmount = Vector3.Cross(transform.forward, direction);

		rigidBody.angularVelocity = (rotationAmount * (turningSpeed)); // multiplay by deltatime
	}*/

	[ExposeMethodInEditor]
	void ResetAgent()
	{
		rigidBody.MovePosition(Vector3.zero);
		rigidBody.velocity = Vector3.zero;
		rigidBody.angularVelocity = Vector3.zero;
	}

    // Update is called once per frame
    void Update()
    {
		thrustActive = Input.GetKey(KeyCode.Space);
		thrustIndicator.SetActive(thrustActive);

		if(Input.GetKeyDown(KeyCode.R))
		{
			ResetAgent();
		}
	}

	private void FixedUpdate()
	{
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

	/// <summary>
	/// As opposed to gizmo draw point
	/// </summary>
	/// <param name="point"></param>
	void DebugDrawPoint(Vector3 point, Color color, float size=1)
	{
		Vector3 up = point + Vector3.up * (size * 0.5f);
		Vector3 down = point + Vector3.down * (size * 0.5f);
		Debug.DrawLine(up, down, color);

		Vector3 right = point + Vector3.right * (size * 0.5f);
		Vector3 left = point + Vector3.left *(size * 0.5f);
		Debug.DrawLine(left, right, color);

		Vector3 forward = point + Vector3.forward * (size * 0.5f);
		Vector3 back = point + Vector3.back * (size * 0.5f);
		Debug.DrawLine(forward, back, color);
	}
}
