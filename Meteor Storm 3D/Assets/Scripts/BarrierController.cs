using UnityEngine;
using System.Collections;

public class BarrierController : MonoBehaviour {

	/// <summary>
	/// Removes meteors that have moved off-screen.
	/// </summary>
	/// <param name="other">The meteor object.</param>
	void OnTriggerEnter(Collider other)
	{
		// Remove the meteor that went off-screen.
		Destroy(other.gameObject);
	}
}
