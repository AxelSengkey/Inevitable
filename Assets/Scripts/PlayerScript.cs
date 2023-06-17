using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // Making variables for moving forward, backward, right, & left; speed; and to control the character movement
    private bool moveForward, moveBackward, moveLeft, moveRight;
    public float speed = 8f;
    public float rotateSpeed = 5f;
    private CharacterController myCharacterController;

    GameObject turnTarget;   //A reference for the turning target

    // Making variable for the scoring system
    public static int scoreCount, curLives;
    public static int lives = 3;

    // Particles
    public ParticleSystem destroyedPoint, destroyedPotion;

    // Create the variables for the AudioClip and AudioSource component
    private AudioSource audioSource;
    public AudioClip point, potion, kill, damagedSound, teleported;

    // Start is called before the first frame update
    void Start()
    {
        // This is used to get character's position from CharacterController
        myCharacterController = GetComponent<CharacterController>();

        turnTarget = GameObject.FindGameObjectWithTag("TurnTarget");
        transform.position = SpawnPoint.GetRandomLocation(60f, turnTarget);

        // Assign the AudioSource component to the audioSource variable
        audioSource = GetComponent<AudioSource>();

        curLives = lives;

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0)
        {
            if (Input.GetMouseButton(1))
            {
                transform.eulerAngles += rotateSpeed * new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
            }
        }

        // Making the key for the movement
        moveForward = Input.GetKey(KeyCode.W);
        moveBackward = Input.GetKey(KeyCode.S);
        moveLeft = Input.GetKey(KeyCode.A);
        moveRight = Input.GetKey(KeyCode.D);

        if (moveForward)
            MoveForward();
        if (moveBackward)
            MoveBackward();
        if (moveLeft)
            MoveLeft();
        if (moveRight)
            MoveRight();
    }

    private void MoveForward()
    {
        // To move the character with direction and speed
        myCharacterController.SimpleMove(transform.forward * speed);
    }

    private void MoveBackward()
    {
        // To move the character with direction and speed
        myCharacterController.SimpleMove(-transform.forward * speed);
    }

    private void MoveLeft()
    {
        // To move the character with direction and speed
        myCharacterController.SimpleMove(-transform.right * speed);
    }

    private void MoveRight()
    {
        // To move the character with direction and speed
        myCharacterController.SimpleMove(transform.right * speed);
    }

    // Function to increase the score
    public void IncreaseScore(GameObject objectCollided)
    {
        if (objectCollided.GetComponent<PointScript>() != null)
        {
            scoreCount += 1;

            audioSource.clip = point; // Set the audio clip
            audioSource.Play(); // Play the sound effect
        }
        else if (objectCollided.GetComponent<PotionScript>() != null)
        {
            scoreCount += 5;

            audioSource.clip = potion; // Set the audio clip
            audioSource.Play(); // Play the sound effect
        }
        else if (objectCollided.GetComponent<EnemyScript>() != null)
        {
            scoreCount += 10;

            audioSource.clip = kill; // Set the audio clip
            audioSource.Play(); // Play the sound effect
        }
    }

    // Function to handle damaged
    public void Damaged()
    {
        curLives--;

        if (curLives < 1)
        {
            Debug.Log("GAME OVER!");
            GameManager.GObool = true; // true -> Indicates that is Game Over
        }

        Input.ResetInputAxes();
        transform.position = SpawnPoint.GetRandomLocation(60f, gameObject);

        audioSource.clip = damagedSound; // Set the audio clip
        audioSource.Play(); // Play the sound effect
    }

    // Function to check if the other objects are collide or not
    public void OnTriggerEnter(Collider other)
    {
        // Condition when the point collided
        if (other.gameObject.GetComponent<PointScript>() != null)
        {
            Destroy(other.gameObject);
            IncreaseScore(other.gameObject);
            Debug.Log("Player Collide point");

            GameManager.Particles(destroyedPoint, transform);
        }
        // Condition when the potion collided
        if (other.gameObject.GetComponent<PotionScript>() != null)
        {
            Destroy(other.gameObject);
            IncreaseScore(other.gameObject);
            Debug.Log("Player Collide potion");

            GameManager.Particles(destroyedPotion, transform);
        }
        // Condition when the enemy collided
        if (other.gameObject.GetComponent<EnemyScript>() != null)
        {
            if (AllyScript.turn)
            {
                Damaged();
                Debug.Log("Player's life decrease");
            }
            else
            {
                IncreaseScore(other.gameObject);
            }
            Debug.Log("Player Collide enemy");
        }
        // Condition when the ally collided
        if (other.gameObject.GetComponent<AllyScript>() != null)
        {
            Damaged();
            Debug.Log("Player's life decrease");
            Debug.Log("Player Collide ally");
        }
    }
}
