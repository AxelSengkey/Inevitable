using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.Barracuda;

public class EnemyScript : Agent
{
    public Transform targetPoint, targetPotion, targetAlly, targetPlayer, targetWall;
    public NNModel avoidAllyBrain, chaseAllyBrain;
    public float speed;
    private float normalSpeed;
    Vector3 lookDir;
    public float range = 15f;
    private bool isDamaged = false;
    private float damageDuration = 0.1f;
    private float damageTimeRemaining;

    GameObject turnTarget;   //A reference for the turning target

    private CharacterController myCharacterController, startingPosition;

    // Particles
    public ParticleSystem destroyedPoint, destroyedPotion;

    // Create the variables for the AudioClip and AudioSource component
    private AudioSource audioSource;
    public AudioClip point, potion, damagedSound;

    // // This is to update the speed according to the curriculum lessons
    // EnvironmentParameters ResetParams;

    // Initialize is called once when the agent is first enabled
    public override void Initialize()
    {
        // This is used to get character's position from CharacterController
        myCharacterController = GetComponent<CharacterController>();
        startingPosition = myCharacterController;

        lookDir = Vector3.zero;

        // Assign the AudioSource component to the audioSource variable
        audioSource = GetComponent<AudioSource>();

        turnTarget = GameObject.FindGameObjectWithTag("TurnTarget");
        transform.position = SpawnPoint.GetRandomLocation(60f, turnTarget);

        normalSpeed = speed;
        damageTimeRemaining = damageDuration;

        // // These are to update the speed according to the curriculum lessons
        // ResetParams = Academy.Instance.EnvironmentParameters;
        // speed = ResetParams.GetWithDefault("enemySpeed", 8);
        // Debug.Log("Academy EP test: " + ResetParams.GetWithDefault("enemySpeed", 8));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);
        sensor.AddObservation(targetPoint.localPosition.x - transform.localPosition.x);
        sensor.AddObservation(targetPoint.localPosition.z - transform.localPosition.z);
        sensor.AddObservation(targetPotion.localPosition.x - transform.localPosition.x);
        sensor.AddObservation(targetPotion.localPosition.z - transform.localPosition.z);
        sensor.AddObservation(targetAlly.localPosition.x - transform.localPosition.x);
        sensor.AddObservation(targetAlly.localPosition.z - transform.localPosition.z);
        sensor.AddObservation(targetAlly.rotation);
        sensor.AddObservation(targetPlayer.localPosition.x - transform.localPosition.x);
        sensor.AddObservation(targetPlayer.localPosition.z - transform.localPosition.z);
        sensor.AddObservation(targetPlayer.rotation);
        sensor.AddObservation(targetWall.localPosition.x - transform.localPosition.x);
        sensor.AddObservation(targetWall.localPosition.z - transform.localPosition.z);
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        for (var i = 0; i < vectorAction.Length; i++)
        {
            vectorAction[i] = Mathf.Clamp(vectorAction[i], -1f, 1f);
        }
        var x = vectorAction[0];
        var z = vectorAction[1];

        if (isDamaged)
        {
            Input.ResetInputAxes();
        }
        else
        {
            myCharacterController.Move(new Vector3(x, 0, z) * speed * Time.deltaTime);
        }

        AddReward(-0.001f);
        // Debug.Log(GetCumulativeReward());

        lookDir = new Vector3(x, 0, z);
    }

    public override void OnEpisodeBegin()
    {
        isDamaged = true;

        transform.position = SpawnPoint.GetRandomLocation(60f, turnTarget);

        float angle = Random.Range(0, Mathf.PI * 2);
        transform.localPosition = new Vector3(Mathf.Sin(angle) * range, 0.5f, Mathf.Cos(angle) * range);
    }

    // void FixedUpdate()
    // {
    //     if (this.StepCount > 1000)
    //     {
    //         EndEpisode();
    //     }
    // }

    // Update is called once per frame
    void Update()
    {
        if (lookDir.magnitude > 0)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(lookDir),
                Time.deltaTime * 10f);
        }

        if (isDamaged)
        {
            if (damageTimeRemaining > 0)
            {
                damageTimeRemaining -= Time.deltaTime;
            }
            else
            {
                isDamaged = false;
            }
        }
        else
        {
            damageTimeRemaining = damageDuration;
        }

        if (AllyScript.turn)
        {
            speed = normalSpeed * 2;
            OnAllyTurn();
        }
        else
        {
            speed = normalSpeed;
            OnAllyDefault();
        }
    }

    // Function to handle damaged
    public void Damaged()
    {
        AddReward(-5f);
        // Debug.Log(GetCumulativeReward());

        isDamaged = true;

        audioSource.clip = damagedSound; // Set the audio clip
        audioSource.Play(); // Play the sound effect

        Debug.Log("Enemy's damaged");

        EndEpisode();
    }

    // Function to check if the other objects are collide or not
    public void OnTriggerEnter(Collider other)
    {
        if (!AllyScript.turn)
        {
            // Condition when the point collided
            if (other.gameObject.GetComponent<PointScript>() != null)
            {
                AddReward(1f);
                // Debug.Log(GetCumulativeReward());

                Destroy(other.gameObject);
                Debug.Log("Enemy Collide point");

                GameManager.Particles(destroyedPoint, transform);

                audioSource.clip = point; // Set the audio clip
                audioSource.Play(); // Play the sound effect
            }
            // Condition when the potion collided
            if (other.gameObject.GetComponent<PotionScript>() != null)
            {
                AddReward(3f);
                // Debug.Log(GetCumulativeReward());

                Destroy(other.gameObject);
                AllyScript.turn = true;
                Debug.Log("Enemy Collide potion");

                GameManager.Particles(destroyedPotion, transform);

                audioSource.clip = potion; // Set the audio clip
                audioSource.Play(); // Play the sound effect
            }
        }
        // Condition when the ally or player collided
        if (other.gameObject.GetComponent<AllyScript>() != null || other.gameObject.GetComponent<PlayerScript>() != null)
        {
            if (AllyScript.turn)
            {
                if (other.gameObject.GetComponent<PlayerScript>() != null)
                {
                    AddReward(5f);
                    // Debug.Log(GetCumulativeReward());

                    audioSource.clip = damagedSound; // Set the audio clip
                    audioSource.Play(); // Play the sound effect
                }
            }
            else
            {
                Damaged();
            }
            Debug.Log("Enemy Collide ally/player");
        }
        // Condition when the wall collided
        if (other.tag == "Wall")
        {
            AddReward(-5f);
            // Debug.Log(GetCumulativeReward());

            Debug.Log("Collide wall");
            EndEpisode();
        }
    }

    public void OnAllyDefault()
    {
        SetModel("Enemy", avoidAllyBrain);
    }

    public void OnAllyTurn()
    {
        SetModel("Enemy", chaseAllyBrain);
    }
}
