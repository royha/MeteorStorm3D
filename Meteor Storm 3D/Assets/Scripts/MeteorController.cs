using UnityEngine;
using System.Collections;

/// <summary>
/// Meteor controller controls an individial meteor.
/// </summary>
public class MeteorController : MonoBehaviour 
{
	// Public variables.

	/// <summary>
	/// The meteor's rigid body.
	/// </summary>
	public Rigidbody rb;

	/// <summary>
	/// The initial velocity range for this meteor.
	/// </summary>
	public Vector3 initialVelocityRange;

	/// <summary>
	/// The initial torque range for this meteor.
	/// </summary>
	public Vector3 initialTorqueRange;

	/// <summary>
	/// Access to the player game object and its components.
	/// </summary>
	public GameObject player;

	/// <summary>
	/// Access to the PlayerController and its public variables.
	/// </summary>
	public PlayerController playerController;

	/// <summary>
	/// The standing XY velocity of the meteor, since meteors have no Z velocity.
	/// </summary>
	private Vector3 standingVelocity;

	/// <summary>
	///  Initialize the Meteor.
	/// </summary>
	void Start () 
	{
		// Get access to the PlayerController's public variables.
		player = GameObject.FindGameObjectWithTag("Player");
		playerController = player.GetComponent<PlayerController>();

		// Set the initial velocity of the meteor.
		rb = GetComponent<Rigidbody>();

		// Calculate this meteor's standing velocity -- the drift of the meteor in x and y.
		standingVelocity = new Vector3(Random.Range(-initialVelocityRange.x, initialVelocityRange.x), 
			Random.Range(-initialVelocityRange.y, initialVelocityRange.y), 0f);

		// Calculate the player's current velocity for steering in x and y, and speed in z.
		Vector3 playerVelocity = new Vector3(playerController.xySteeringAmount.x, 
			playerController.xySteeringAmount.y, playerController.playerVelocity);

		// Calculate the meteor's actual velocity by combining standing and player velocities.
		Vector3 meteorVelocity = standingVelocity - playerVelocity;

		// Start meteor in the appropriate direction and speed.
		rb.AddForce(meteorVelocity, ForceMode.VelocityChange);

		// Set the initial torque of the meteor.
		rb.AddTorque(Random.Range(-initialTorqueRange.x, initialTorqueRange.x), 
			Random.Range(-initialTorqueRange.y, initialTorqueRange.y),  
			Random.Range(-initialTorqueRange.z, initialTorqueRange.z), 
			ForceMode.VelocityChange);
	}
	
	/// <summary>
	/// Updates the meteor position.
	/// </summary>
	void FixedUpdate() 
	{
		// Destroy the object when it goes behind the camera.
		if (transform.position.z < -10)
		{
			Destroy(gameObject);
		}

		// Move the meteors.

		// Set the z speed to the player's speed.
		rb.velocity = new Vector3(0f, 0f, -playerController.playerVelocity);

		// Add the force of the xy steering
		rb.velocity -= playerController.xySteeringAmount;

		// Add the standing force for this meteor.
		rb.velocity += standingVelocity;
	}
}
