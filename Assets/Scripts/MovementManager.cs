using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{

    [SerializeField] GameObject spawnPointRB_a;
    [SerializeField] GameObject spawnPointRB_b;
    public float forwardSpeed = 60f;    // Adjust the forward speed as needed
    public float regularForwardSpeedSpaceShip = 25f;    // Adjust the forward speed as needed for space ship
    public float newForwardSpeedSpaceShip = 25f;
    public GameObject spaceShip;

    private void Awake()
    {
        forwardSpeed = 0;
        newForwardSpeedSpaceShip = 30;
    }
    public void MMSpaceShipExit()
    {
        newForwardSpeedSpaceShip = 80;
    }
    public void SpaceShipEnterMM()
    {
        newForwardSpeedSpaceShip = regularForwardSpeedSpaceShip;
    }
    public void StartGameMM(int forwardSpeedSP, int spaceShipSpeed)
    {
        forwardSpeed = forwardSpeedSP;
        newForwardSpeedSpaceShip = spaceShipSpeed;
    }

    void FixedUpdate()
    {
        Vector3 forwardMovement = transform.forward * forwardSpeed;
        Vector3 forwardMovementSpaceShip = transform.forward * newForwardSpeedSpaceShip;

        // Move space ship
        spaceShip.transform.Translate(forwardMovementSpaceShip * Time.deltaTime, Space.World);

        // Move spawn points using Rigidbody
        spawnPointRB_a.transform.Translate(forwardMovementSpaceShip * Time.deltaTime, Space.World);
        spawnPointRB_b.transform.Translate(forwardMovementSpaceShip * Time.deltaTime, Space.World);
    }


}
