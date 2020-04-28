using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering2 : MonoBehaviour
{
	public enum CurrentOperation
	{
		None,
		ClimbToTarget,
		BankToTarget
	}

	// let's create data structs to store the state of each operation
	// that way we can show them in the inspector and step through

	/// <summary>
	/// Both the ClimbToTarget and BankToTarget operations use this,
	/// since they are effectively the same maneuver, just with different planes.
	/// </summary>
	 [System.Serializable]
	public struct PlaneManeuverData
	{
		public Plane OrthhogonalPlane;
		public Vector3 PointOnPlane;

		public Vector3 Direction;
		public float Angle;
		public float MaxAngle;
		public float ClampedAngle;

		public Quaternion Rotation;
	}

	// then when we need to optimize, we can just take them off the heap and put them back on the stack
	[Header("Maneuver Data")]
	[SerializeField] bool isClimbing;
	[SerializeField] PlaneManeuverData climbData;

	[SerializeField] bool isBanking;
	[SerializeField] PlaneManeuverData bankData;

	[SerializeField] Transform target;
	[Range(0, 45)]
	[SerializeField] float turningSpeed;
	Rigidbody rigidBody;

	[Range(0, 0.1f)]
	[SerializeField] float inPlaneThreshold = 0.01f;

	[SerializeField] CurrentOperation currentOperation = CurrentOperation.None;
	bool autoTurn = false;

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
		if (Input.GetKeyDown(KeyCode.T)) autoTurn = !autoTurn;
	}

	PlaneManeuverData CalculatePlaneManeuver(Vector3 targetPosition, Vector3 planeNormal, Vector3 planePoint,
		Vector3 craftAxis)
	{
		Plane orthogonalPlane = new Plane(planeNormal, planePoint);
		Vector3 pointOnPlane = orthogonalPlane.ClosestPointOnPlane(targetPosition);

		Vector3 direction = (pointOnPlane - planePoint).normalized;
		float angle = Vector3.SignedAngle(craftAxis, direction, planeNormal);
		float maxAngle = turningSpeed * Time.fixedDeltaTime;
		float clampedAngle = Mathf.Clamp(angle, -maxAngle, maxAngle);

		Quaternion rotation = Quaternion.AngleAxis(clampedAngle, planeNormal);

		return new PlaneManeuverData()
		{
			OrthhogonalPlane = orthogonalPlane,
			PointOnPlane = pointOnPlane,
			Angle = angle,
			ClampedAngle = clampedAngle,
			Direction = direction,
			MaxAngle = maxAngle,
			Rotation = rotation
		};
	}

	void ClimbToTarget()
	{
		PlaneManeuverData climbData = CalculatePlaneManeuver(target.transform.position, transform.right, transform.position,
			transform.forward);
		this.climbData = climbData;
		isClimbing = true;

		rigidBody.rotation = climbData.Rotation * rigidBody.rotation;
	}

	void BankToTarget()
	{
		PlaneManeuverData bankData = CalculatePlaneManeuver(target.transform.position, transform.forward, transform.position,
			transform.up);
		this.bankData = bankData;
		isBanking = true;

		rigidBody.rotation = bankData.Rotation * rigidBody.rotation;
	}

	private void FixedUpdate()
	{
		isClimbing = false;
		isBanking = false;

		if (autoTurn)
		{
			switch (currentOperation)
			{
				case CurrentOperation.None:
					// if we have a lock on our target (it is dead ahead) do nothing
					// otherwise, if the target is out of the horizontal plane, start a climb or dive maneuver
					// if the target is in the horizontal plane but out of the vertical plane, start banking
					if (!TargetInHorizontalPlane(target.position)) currentOperation = CurrentOperation.ClimbToTarget;
					else if (!TargetInVerticalPlane(target.position)) currentOperation = CurrentOperation.BankToTarget;
					break;
				case CurrentOperation.ClimbToTarget:
					if (TargetInHorizontalPlane(target.position) && 
						!TargetInVerticalPlane(target.position)) currentOperation = CurrentOperation.BankToTarget;
					// if we've finished climbing, enter either none or bank depending on which action is appropriate
					break;

				case CurrentOperation.BankToTarget:
					if (TargetInVerticalPlane(target.position) &&
						!TargetInHorizontalPlane(target.position)) currentOperation = CurrentOperation.ClimbToTarget;
					break;

				default:
					break;
			}

			switch (currentOperation)
			{
				case CurrentOperation.None:
					break;
				case CurrentOperation.ClimbToTarget:
					ClimbToTarget();
					break;
				case CurrentOperation.BankToTarget:
					BankToTarget();
					break;
				default:
					break;
			}

			/*if (!TargetInHorizontalPlane(target.position)) ClimbToTarget();
			else if (!TargetInVerticalPlane(target.position)) BankToTarget();*/
		}
		else if (Input.GetKey(KeyCode.W)) BankToTarget();
		else if (Input.GetKey(KeyCode.D)) ClimbToTarget();
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
	void DebugDrawPoint(Vector3 point, Color color, float size = 1)
	{
		Vector3 up = point + Vector3.up * (size * 0.5f);
		Vector3 down = point + Vector3.down * (size * 0.5f);
		Debug.DrawLine(up, down, color);

		Vector3 right = point + Vector3.right * (size * 0.5f);
		Vector3 left = point + Vector3.left * (size * 0.5f);
		Debug.DrawLine(left, right, color);

		Vector3 forward = point + Vector3.forward * (size * 0.5f);
		Vector3 back = point + Vector3.back * (size * 0.5f);
		Debug.DrawLine(forward, back, color);
	}

	Plane GetVerticalPlane()
	{
		return new Plane(transform.position, GetVerticalPlaneNormal());
	}

	Vector3 GetVerticalPlaneNormal()
	{
		return transform.right;
	}

	// tells us if we need to bank to reach our target
	bool TargetInVerticalPlane(Vector3 targetPosition)
	{
		Plane verticalPlane = new Plane(GetVerticalPlaneNormal(), transform.position);
		return Mathf.Abs(verticalPlane.GetDistanceToPoint(targetPosition)) < inPlaneThreshold;
	}

	#region Horizontal Plane Legacy
	Plane GetHorizontalPlane()
	{
		return new Plane(transform.position, GetHorizontalPlaneNormal());
	}

	Vector3 GetHorizontalPlaneNormal()
	{
		return transform.up;
	}

	// tells us if we need to climb to reach our target
	bool TargetInHorizontalPlane(Vector3 targetPosition)
	{
		Plane verticalPlane = new Plane(GetHorizontalPlaneNormal(), transform.position);
		return Mathf.Abs(verticalPlane.GetDistanceToPoint(targetPosition)) < inPlaneThreshold;
	}
	#endregion

	private void OnDrawGizmos()
	{
		Gizmos.color = (TargetInVerticalPlane(target.position)) ? Color.green : Color.red;
		DrawPlane(transform.position, GetVerticalPlaneNormal());

		Gizmos.color = (TargetInHorizontalPlane(target.position)) ? Color.green : Color.red;
		DrawPlane(transform.position, GetHorizontalPlaneNormal());
	}
}
