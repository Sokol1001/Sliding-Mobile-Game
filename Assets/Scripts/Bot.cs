using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public float forwardSpeed = 5f; // Forward movement speed
    public float slideForce = 10f; // Force to keep the bot on the slide
    public List<Transform> slides = new List<Transform>(); // List of slide transforms

    private Rigidbody rb;
    private Transform currentTargetSlide;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentTargetSlide = FindClosestSlide();
    }

    private void Update()
    {
        // Move the bot forward constantly
        Vector3 forwardVelocity = transform.forward * forwardSpeed;
        rb.velocity = new Vector3(forwardVelocity.x, rb.velocity.y, forwardVelocity.z);

        // Check if the bot has reached the current target slide
        float distanceToTarget = Vector3.Distance(transform.position, currentTargetSlide.position);
        if (distanceToTarget < 100f) // Adjust the threshold as needed
        {
            // Switch target slide
            currentTargetSlide = FindClosestSlide();
        }

        // Calculate the direction towards the current target slide
        Vector3 directionToSlide = currentTargetSlide.position - transform.position;

        // Apply force to keep the bot on the slide
        rb.AddForce(directionToSlide.normalized * slideForce);
    }

    Transform FindClosestSlide()
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Transform potentialTarget in slides)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }
        return bestTarget;
    }
    public void AddSlides(Transform slide)
    {
        slides.Add(slide);
        currentTargetSlide = FindClosestSlide();
    }
    public void RemoveSlides()
    {
        slides.Clear();
    }
}

