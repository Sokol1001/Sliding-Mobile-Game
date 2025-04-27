using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SlideMagnet : MonoBehaviour
{
    public float magnetStrength = 10f; // Adjust this value to change the strength of the "magnet"

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) // Replace "Player" with the tag of your player object
        {
            Vector3 direction = (transform.position - other.transform.position).normalized;
            other.GetComponent<Rigidbody>().AddForce(direction * magnetStrength);
        }
    }
}
