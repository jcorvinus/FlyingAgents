using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnHandler : MonoBehaviour
{
	// draw a turn arc based off of
	// maximum turn rate
	// and current speed

	[Range(0, 10)]
	[SerializeField] float speed;

	[Range(0, 20)]
	[SerializeField] float time;

	[Range(0,180)]
	[SerializeField] float maxTurnDegreesPerSecond;


	// if current speed = 0, turn radius is also 0
	// turn radius should increase with speed


	[Range(10, 30)]
	[SerializeField] int sampleCount = 20;

	private void OnDrawGizmos()
	{
		float distance = speed * time;

		float maxTurnPossible = maxTurnDegreesPerSecond * time;
		//bool fullTurn = 		
	}
}
