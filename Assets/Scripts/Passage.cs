using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passage : MonoBehaviour
{
    public Transform passageOut;
    public GameObject player, info;
    public ParticleSystem teleport;

    // Create the variables for the AudioClip and AudioSource component
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        info.SetActive(false);

        // Assign the AudioSource component to the audioSource variable
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (info.activeInHierarchy)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject objectClicked = hit.transform.gameObject;
                    if (objectClicked == gameObject)
                    {
                        Vector3 position = player.transform.position;
                        position.x = passageOut.position.x;
                        position.y = passageOut.position.y;
                        player.transform.position = position;
                        Debug.Log("Teleported");

                        GameManager.Particles(teleport, player.transform);

                        audioSource.Play(); // Play the sound effect
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            info.SetActive(true);
            Debug.Log("Player is near the button");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            info.SetActive(false);
        }
    }
}
