using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class PointScript : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0)
        {
            // To spin the coin in every update
            transform.Rotate(4f, 4f, 4f * Time.deltaTime);
        }
    }
}
