using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public Camera cam; // Assign the camera in the inspector
    public float rotationSpeed = 1f;
    private Vector2 startTouchPosition;
    private bool isRotating;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    isRotating = true;
                    startTouchPosition = touch.position;
                    break;
                case TouchPhase.Ended:
                    isRotating = false;
                    break;
            }

            if (isRotating)
            {
                Vector2 offset = touch.position - startTouchPosition;
                float rotation = offset.x * -rotationSpeed * Time.deltaTime;
                transform.Rotate(0f, rotation, 0f);
            }
        }
    }
}
