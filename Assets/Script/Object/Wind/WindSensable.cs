using UnityEngine;
using System.Collections;

public interface WindSensable {

	 void SenseWind(Vector2 velocity);
	 void ExitWind();

	 Vector3 getPosition();

	 WindSensablParameter getParameter();
}
