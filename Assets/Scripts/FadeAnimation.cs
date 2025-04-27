using UnityEngine;

public class FadeAnimation : MonoBehaviour
{
    private float fadeDuration = 1.5f; // Duration of the fade animation in seconds
    private float targetAlpha = 0.0f; // Target alpha value (0 = fully transparent, 1 = fully opaque)

    private Material material;
    private Color originalColor;
    private Color targetColor;

    private float startTime;
    public bool hasCollided = false;

    void Start()
    {

        // Get the material of the prefab
        Renderer renderer = GetComponent<Renderer>();
        material = renderer.material;

        // Store the original color
        originalColor = material.color;

        // Set the target color with the same RGB values as the original color and the target alpha
        targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, targetAlpha);

        // Start the fade animation
        startTime = Time.time;
    }

    void Update()
    {
        if (hasCollided)
        {
            // Calculate the time elapsed since the animation started
            float elapsedTime = Time.time - startTime;

            // Calculate the interpolation factor (0 to 1) based on the elapsed time and fade duration
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);

            // Interpolate between the original color and the target color
            material.color = Color.Lerp(originalColor, targetColor, t);

            float moveAmount = 15f * Time.deltaTime;
            transform.position += Vector3.down * moveAmount;

            if (t >= 1.0f)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Base"))
        {
            hasCollided = true;
        }
    }
}
