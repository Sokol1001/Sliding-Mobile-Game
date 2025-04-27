using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Zigzags : MonoBehaviour
{
    public GameObject prefab; // The prefab to instantiate
    public int numberOfObjects = 10; // Number of objects to instantiate
    public float horizontalSpacing = 10f; // Spacing between objects horizontally
    public float verticalSpacing = 10f; // Spacing between objects vertically
    public float size = 150f;
    public Quaternion rotation;

    private void Awake()
    {
        Vector3 startPos = transform.position;

        for (int i = 0; i < numberOfObjects; i++)
        {
            GameObject obj = Instantiate(prefab, startPos + new Vector3(i * horizontalSpacing, 0, i % 2 == 0 ? 0f : verticalSpacing), Quaternion.identity);

            obj.gameObject.transform.SetParent(gameObject.transform);

            // If the index is odd, move the object up by verticalSpacing

            Vector3 newScale = new Vector3(size, size, size);
            Transform objectTransform = obj.transform;
            objectTransform.localScale = newScale;
            objectTransform.rotation = rotation;

        }
    }
}
