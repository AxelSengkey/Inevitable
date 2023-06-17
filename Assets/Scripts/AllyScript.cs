using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using Unity.MLAgents;

public class AllyScript : MonoBehaviour
{
    GameObject target;   //A reference for the Enemy
    NavMeshAgent allyNav;         //A reference for the Ally Nav Mesh Agent component
    GameObject turnTarget;   //A reference for the turning target
    public static bool turn = false;
    public static float seconds;
    SphereCollider allyCollider;

    // // This is to update the speed according to the curriculum lessons
    // EnvironmentParameters ResetParams;

    // Start is called before the first frame update
    void Start()
    {
        allyNav = GetComponent<NavMeshAgent>();

        target = GameObject.FindGameObjectWithTag("Enemy");
        turnTarget = GameObject.FindGameObjectWithTag("TurnTarget");

        allyCollider = GetComponent<SphereCollider>();

        transform.position = SpawnPoint.GetRandomLocation(60f, turnTarget);

        // // These are to update the speed and collider according to the curriculum lessons
        // ResetParams = Academy.Instance.EnvironmentParameters;
        // allyNav.speed = ResetParams.GetWithDefault("allySpeed", 6);
        // allyCollider.radius = ResetParams.GetWithDefault("allyCollider", 0.5f);
        // allyCollider.radius = ResetParams.GetWithDefault("playerCollider", 0.5f);
        // Debug.Log("Academy EP test: " + ResetParams.GetWithDefault("allySpeed", 6));
        // Debug.Log("Academy EP test: " + ResetParams.GetWithDefault("allyCollider", 0.5f));
        // Debug.Log("Academy EP test: " + ResetParams.GetWithDefault("playerCollider", 0.5f));
    }

    // FixedUpdate is called every 0.02 seconds
    void FixedUpdate()
    {
        // Debug.Log(Time.deltaTime);
        if (turn)
        {
            allyNav.destination = turnTarget.transform.position;
            if (GameManager.potionTimeRemaining > 0)
            {
                GameManager.potionTimeRemaining -= Time.deltaTime * 0.4f;
                seconds = Mathf.RoundToInt(GameManager.potionTimeRemaining);
            }
            else
            {
                turn = false;
            }
        }
        else
        {
            GameManager.potionTimeRemaining = GameManager.potionDuration;
            seconds = 0;
            allyNav.destination = target.transform.position;
        }
    }

    // public void OnTriggerEnter(Collider other)
    // {
    //     // Condition when the enemy collided
    //     if (other.gameObject.GetComponent<EnemyScript>() != null)
    //     {
    //         if (AllyScript.turn)
    //         {
    //             transform.position = new Vector3(0f, 1f, -1f);
    //             Debug.Log("Ally back to 0,1,-1");
    //         }
    //         Debug.Log("Ally Collide enemy");
    //     }
    // }
}