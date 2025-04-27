using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.Euler(new Vector3(0, 5f, 0));

        transform.Rotate(Vector3.up, 4f * Time.deltaTime);
    }
}
