using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FireX : MonoBehaviour
{

    [SerializeField] GameObject firePrefab;
    [SerializeField] int numberOfObjects;
    List<GameObject> allObjects = new List<GameObject>();

    List<float> Xlist = new List<float>();


    float time = 0f;
    public Transform point;
    private float xPivot;

    void Start()
    {


        //x
        Vector3 spawnPositionX = transform.position;

        //y
        Vector3 spawnPositionY = transform.position + new Vector3(5.4f, -5.4f, 0f);

        for (int j = 0; j < numberOfObjects; j++)
        {
            GameObject fireX = Instantiate(firePrefab, spawnPositionX, Quaternion.identity);
            spawnPositionX.x += 0.15f; //0.12
            allObjects.Add(fireX);
            fireX.transform.SetParent(gameObject.transform);

            GameObject fireY = Instantiate(firePrefab, spawnPositionY, Quaternion.identity);
            spawnPositionY.y += 0.15f; //0.12
            allObjects.Add(fireY);
            fireY.transform.SetParent(gameObject.transform);
        }

        Xlist.Add(spawnPositionX.x);


    }


    void Update()
    {
        time += 0.00002f; //Time.deltaTime;

        foreach (var obj in allObjects)
        {
            float x = Mathf.Cos(time) / 30f + obj.transform.position.x;
            float y = Mathf.Sin(time) / 30f + obj.transform.position.y;

            xPivot = point.position.x;
            xPivot += 5.4f;  //3f

            obj.transform.RotateAround(new Vector3(xPivot, point.position.y, point.position.z), Vector3.back, Time.deltaTime * 50);
        }
    }
}