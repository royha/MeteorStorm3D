using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Player controller controls the movement of the meteors as a whole to give the
/// appearance of moving the player.
/// </summary>
public class PlayerController : MonoBehaviour 
{
	// Public variables.

	/// <summary>
	/// Adjusts the sensitivity of the mouse. Zero turns off mouse control.
	/// </summary>
	public float mouseSpeed;

	/// <summary>
	/// The four meteor types.
	/// </summary>
	public Rigidbody meteorAccel;
	public Rigidbody meteorBlock;
	public Rigidbody meteorSuperAccel;
	public Rigidbody meteorSuperBlock;

	// The distribution of meteors in the meteor storm.
	public float pctSuperAccel;
	public float pctAccel;
	public float pctSuperBlock;

	/// <summary>
	/// The density of meteors in the meteor storm.
	/// </summary>
	public float meteorsPerUnit;

	/// <summary>
	/// The Z-axis location of the spawn area for meteors.
	/// </summary>
	public float meteorRange;

	/// <summary>
	/// The change in speed for a Block meteor.
	/// </summary>
	public float meteorBlockDelta;

	/// <summary>
	/// The change in speed for a Super Block meteor.
	/// </summary>
	public float meteorSuperBlockDelta;

	/// <summary>
	/// The change in speed for an Accelerate meteor.
	/// </summary>
	public float meteorAccelDelta;

	/// <summary>
	/// The change in speed for an Super Accelerate meteor.
	/// </summary>
	public float meteorSuperAccelDelta;

	/// <summary>
	/// The player velocity.
	/// </summary>
	public float playerVelocity;

	/// <summary>
	/// The velocity decay subtracted each pass.
	/// </summary>
	public float decayVelocitySubtract;

	/// <summary>
	/// The velocity decay factor (ie., 0.99).
	/// </summary>
	public float decayVelocityFactor;

	/// <summary>
	/// The player steering controls.
	/// </summary>
	public Vector3 xySteeringAmount;
	public bool xySteeringChanged;
	public Vector3 xySteeringDelta;
	public float xySteeringFactor;

	/// <summary>
	/// The audio sources for each type of meteor.
	/// </summary>
	public AudioSource meteorSuperAccelSound;
	public AudioSource meteorAccelSound;
	public AudioSource meteorBlockSound;
	public AudioSource meteorSuperBlockSound;

	// Private variables.
	private Rigidbody rb;

	/// <summary>
	/// If there are 2.8 meteors to add this FixedUpdate, this stores the fractional amount
	/// of 0.8 for the next FixedUpdate.
	/// </summary>
	private float meteorFraction;

	/// <summary>
	/// Retains the previous steering amount to calculate the steering delta.
	/// </summary>
	private Vector3 previousXYSteeringAmount;

	/// <summary>
	/// Returns true if a Super Accelerate meteor was hit this turn.
	/// </summary>
	private bool hitSuperAccelMeteor;

	/// <summary>
	/// Returns true if an Accelerate meteor was hit this turn.
	/// </summary>
	private bool hitAccelMeteor;

	/// <summary>
	/// Returns true if a Block meteor was hit this turn.
	/// </summary>
	private bool hitBlockMeteor;

	/// <summary>
	/// Returns true if a Super Block meteor was hit this turn.
	/// </summary>
	private bool hitSuperBlockMeteor;

	/// <summary>
	/// Returns true when the game is over.
	/// </summary>
	private bool gameOver = false;

	/// <summary>
	/// Stores the start time of the game to calculate how long the game was played.
	/// </summary>
	private System.DateTime gameStartTime;

	/// <summary>
	/// Stores the moment the game ends.
	/// </summary>
	private System.DateTime gameStopTime;

	/// <summary>
	/// The longest game time this session.
	/// </summary>
	private System.TimeSpan maxGameTime = new System.TimeSpan(0);

	/// <summary>
	/// The fastest speed this round.
	/// </summary>
	private float maxPlayerVelocity;

	/// <summary>
	/// The fastest speed since starting the game.
	/// </summary>
	private float maxOfAllPlayerVelocities = 0f;

	Text textGameOver;
	Text textRestart;

	Text textTarget;
	Text textTime;
	Text textVelocity;

	/// <summary>
	/// Initialize the Player.
	/// </summary>
	void Start() 
	{
		rb = GetComponent<Rigidbody>();
		previousXYSteeringAmount = Vector3.zero;
		hitSuperAccelMeteor = false;
		hitAccelMeteor = false;
		hitBlockMeteor = false;
		hitSuperBlockMeteor = false;
		gameStartTime = System.DateTime.Now;
		maxPlayerVelocity = 0f;

		textGameOver = GameObject.Find("GameOverText").GetComponent<Text>();
		textGameOver.text = "";
		textRestart = GameObject.Find("RestartText").GetComponent<Text>();
		textRestart.text = "";

		textTarget = GameObject.Find("TargetText").GetComponent<Text>();
		textTarget.text = "+";
		textTime = GameObject.Find("TimeText").GetComponent<Text>();
		textTime.text = "";
		textVelocity = GameObject.Find("VelocityText").GetComponent<Text>();
		textVelocity.text = "";
	}

	/// <summary>
	/// Update the Player progress.
	/// </summary>
	void FixedUpdate() 
	{
		// Slow the player down over time.
		ChangePlayerVelocity();

		// Accept user input.
		ProcessInput();

		// Add new meteors to fly past.
		AddNewMeteors();

		// Update the on screen score display.
		UpdateScore();
	}

	/// <summary>
	/// Slow the player down over time, and change speed based on which type of meteor
	/// the player collided with.
	/// </summary>
	void ChangePlayerVelocity()
	{
		if (!gameOver)
		{
			// Reduce velocity by the factor amount.
			playerVelocity *= decayVelocityFactor;

			// Reduce velocity by the subtraction amount.
			playerVelocity -= decayVelocitySubtract;

			// If the player collided with a meteor, adjust the player's velocity accordingly
			// and play the appropriate sound for the meteor type.
			if (hitSuperAccelMeteor)
			{
				// Change velocity by the Accelerate meteor amount.
				playerVelocity += meteorSuperAccelDelta;

				// Reset the hit flag.
				hitSuperAccelMeteor = false;

				// Play the sound.
				meteorSuperAccelSound.Play();
			}
			else if (hitAccelMeteor)
			{
				// Change velocity by the Accelerate meteor amount.
				playerVelocity += meteorAccelDelta;

				// Reset the hit flag.
				hitAccelMeteor = false;

				// Play the sound.
				meteorAccelSound.Play();
			}
			else if (hitBlockMeteor)
			{
				// Change velocity by the Block meteor amount.
				playerVelocity += meteorBlockDelta;

				// Reset the hit flag.
				hitBlockMeteor = false;

				// Play the sound.
				meteorBlockSound.Play();
			}
			else if (hitSuperBlockMeteor)
			{
				// Change velocity by the Super Block meteor amount.
				playerVelocity += meteorSuperBlockDelta;

				// Reset the hit flag.
				hitSuperBlockMeteor = false;

				// Play the sound.
				meteorSuperBlockSound.Play();
			}

			// Set velocity to zero if the velocity falls below zero.
			playerVelocity = (playerVelocity < 0) ? 0 : playerVelocity;

			if (playerVelocity == 0)
			{
				UpdateScore();
				SetGameOver();
			}

			// Record the maximum velocity.
			maxPlayerVelocity = (playerVelocity > maxPlayerVelocity) ? playerVelocity : maxPlayerVelocity;
		}
	}

	/// <summary>
	/// Process user input from the keyboard and mouse.
	/// </summary>
	void ProcessInput()
	{
		if (gameOver)
		{
			// If the game is over, check for "R" to restart the game or "X" to exit.
			if (Input.GetKeyDown(KeyCode.R))
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
			else if (Input.GetKeyDown(KeyCode.X))
			{
				Application.Quit();
			}

			return;
		}

		// Change X and Y axis steering speed based on keyboard input.
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		// Change X and Y axis steering speed based on mouse input.
		if (mouseSpeed != 0)
		{
			moveHorizontal += Input.GetAxis ("Mouse X") * mouseSpeed;
			moveVertical += Input.GetAxis ("Mouse Y") * mouseSpeed;

			moveHorizontal = (moveHorizontal < -1) ? -1 : moveHorizontal;
			moveHorizontal = (moveHorizontal > 1) ? 1 : moveHorizontal;
			moveVertical = (moveVertical < -1) ? -1 : moveVertical;
			moveVertical = (moveVertical > 1) ? 1 : moveVertical;
		}

		Vector3 xyMovement = new Vector3 (moveHorizontal, moveVertical, 0.0f);

		// Adjust steering force by the player's velocity.
		xySteeringAmount = xyMovement * playerVelocity * xySteeringFactor;

		if (xySteeringAmount == previousXYSteeringAmount)
		{
			xySteeringChanged = false;
		} 
		else
		{
			xySteeringChanged = true;
			xySteeringDelta = previousXYSteeringAmount - xySteeringAmount;
		}

		previousXYSteeringAmount = xySteeringAmount;
	}

	/// <summary>
	/// Adds the new meteors for this turn. 
	/// </summary>
	void AddNewMeteors()
	{
		if (!gameOver)
		{
			int meteorsToAdd; // Number of meteors to add this turn.

			meteorsToAdd = (int)((playerVelocity * meteorsPerUnit) + meteorFraction);
			meteorFraction = ((playerVelocity * meteorsPerUnit) + meteorFraction) - (float)meteorsToAdd;

			// Add new meteors.
			for (int i = 0; i < meteorsToAdd; ++i)
			{
				float rv = Random.value;
				float emitterHalfWidth = meteorRange * 0.54f;
				float emitterHalfHeight = meteorRange * 0.3f;

				if (rv < pctSuperAccel / 100f)
				{
					// Rarely, create a super acceleration meteor.
					Instantiate(meteorSuperAccel, 
						-xySteeringAmount +
						new Vector3(Random.Range(-emitterHalfWidth, emitterHalfWidth), 
							Random.Range(-emitterHalfHeight, emitterHalfHeight), 
							meteorRange), 
						Quaternion.identity);
				}
				else if (rv < pctAccel / 100f)
				{
					// Perodically, create an acceleration meteor.
					Instantiate(meteorAccel, 
						-xySteeringAmount +
						new Vector3(Random.Range(-emitterHalfWidth, emitterHalfWidth), 
							Random.Range(-emitterHalfHeight, emitterHalfHeight), 
							meteorRange), 
						Quaternion.identity);
				}
				else if (rv > pctSuperBlock / 100f)
				{
					// Rarely, create a super block meteor.
					Instantiate(meteorSuperBlock, 
						-xySteeringAmount +
						new Vector3(Random.Range(-emitterHalfWidth, emitterHalfWidth), 
							Random.Range(-emitterHalfHeight, emitterHalfHeight), 
							meteorRange), 
						Quaternion.identity);
				}
				else
				{
					// Create a normal block meteor.
					Instantiate(meteorBlock, 
						-xySteeringAmount +
						new Vector3(Random.Range(-emitterHalfWidth, emitterHalfWidth), 
							Random.Range(-emitterHalfHeight, emitterHalfHeight), 
							meteorRange), 
						Quaternion.identity);
				}
			}
		}
	}

	/// <summary>
	/// Updates the time, velocity, and maximum velocity on screen.
	/// </summary>
	void UpdateScore()
	{
		if (!gameOver)
		{
			System.TimeSpan currentGameTime = (System.DateTime.Now - gameStartTime);
			maxGameTime = (currentGameTime > maxGameTime) ? currentGameTime : maxGameTime;

			textTime.text = System.String.Format("    Time: {0:F1}\n", currentGameTime.TotalSeconds);

			maxPlayerVelocity = (playerVelocity > maxPlayerVelocity) ? playerVelocity : maxPlayerVelocity;
		
			textVelocity.text = System.String.Format("{0:F1} :Velocity            \n{1:F1} :Max Velocity    \n",
				playerVelocity, maxPlayerVelocity);
		}
	}

	/// <summary>
	/// Sets the game over state.
	/// </summary>
	void SetGameOver()
	{
		gameOver = true;
		gameStopTime = System.DateTime.Now;

		if (maxPlayerVelocity > maxOfAllPlayerVelocities)
		{
			maxOfAllPlayerVelocities = maxPlayerVelocity;
		}

		textTarget.text = "";

		textGameOver.text = "Game Over";
		textRestart.text = "Press \"R\" to Restart, or \"X\" to Exit.";
	}

	/// <summary>
	/// Sets the flag for the type of meteor that collided with the player.
	/// </summary>
	/// <param name="other">The meteor that collided with the player.</param>
	void OnTriggerEnter(Collider other) 
	{
		// Base action on which GameObject collided with the player.
		switch (other.gameObject.tag)
		{
			case "SuperAccelerate":
				// Set the flag to super accelerate player.
				hitSuperAccelMeteor = true;
				break;

			case "Accelerate":
				// Set the flag to accelerate player.
				hitAccelMeteor = true;
				break;
			
			case "Block":
				// Set the flag to decelerate player.
				hitBlockMeteor = true;
				break;
			
			case "SuperBlock":
				// Set the flag to super decelerate player.
				hitSuperBlockMeteor = true;
				break;
		}
	}
}
