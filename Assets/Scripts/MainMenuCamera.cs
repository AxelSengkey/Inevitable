using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    // Create the variable for the direction, and to set the direction up and down
    private Vector3 direction, upward = new Vector3(-1f, 0f, 0f), downward = new Vector3(1f, 0f, 0f);
    private float rotateSpeed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // The camera will rotate slowly upward (x-axis) until it reach rotation x=-0.1,
        // and then rotate slowly downward (x-axis) until it reach rotation x=0
        if (transform.rotation.x >= 0.034)
            direction = upward;
        else if (transform.rotation.x <= 0.019)
            direction = downward;

        transform.Rotate(direction, rotateSpeed * Time.deltaTime);
    }
}
