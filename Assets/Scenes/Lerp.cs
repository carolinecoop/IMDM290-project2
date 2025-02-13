using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerp : MonoBehaviour
{
    GameObject[] spheres;
    static int numSphere = 1000;
    float time = 0f;
    Vector3[] startPosition, endPosition;
    float lerpFraction; 
    float t;
    public Transform centerPoint; 

    // New variables for controlling color change speed
    private float colorChangeInterval = 1f; // Seconds between color changes
    private float[] colorChangeTimers; // Stores color changes 

    void Start()
    {
        spheres = new GameObject[numSphere];
        startPosition = new Vector3[numSphere];
        endPosition = new Vector3[numSphere];
        colorChangeTimers = new float[numSphere]; 

        // Define target positions. Start = random, End = heart
        for (int i = 0; i < numSphere; i++)
        {
            float r = 15f;
            startPosition[i] = new Vector3(r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f));

            // Heart shape end position with warping
            t = i * 2 * Mathf.PI / numSphere;
            endPosition[i] = new Vector3(
                5f * Mathf.Sqrt(2f) * Mathf.Sin(t) * Mathf.Sin(t) * Mathf.Sin(t),
                5f * (-Mathf.Cos(t) * Mathf.Cos(t) * Mathf.Cos(t) - Mathf.Cos(t) * Mathf.Cos(t) + 2 * Mathf.Cos(t)) + 3f + Mathf.Sin(time * 2) * 2f,
                10f + Mathf.Sin(time)
            );

            // Create spheres
            spheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spheres[i].transform.position = startPosition[i];

            // Random size for variety
            float scale = Random.Range(0.3f, 1f);
            spheres[i].transform.localScale = Vector3.one * scale;

            // Set initial color to white
            Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
            sphereRenderer.material.color = Color.white;
            sphereRenderer.material.SetColor("_EmissionColor", Color.white * 2f);
        }
    }

    void Update()
    {
        time += Time.deltaTime;

        for (int i = 0; i < numSphere; i++)
        {
            colorChangeTimers[i] += Time.deltaTime; // Increment timer

            lerpFraction = Mathf.Sin(time) * 0.5f + 0.5f;

            // Warping effect
            float wave = Mathf.Sin(time + i * 0.1f) * 0.5f;
            Vector3 warpEffect = new Vector3(wave, wave, wave);

            //Lerping
            spheres[i].transform.position = Vector3.Lerp(startPosition[i], endPosition[i] + warpEffect, lerpFraction);

            // Rotate the sphere around a central point
            float rotateSpeed = 20f * Mathf.Sin(time * 0.5f);
            spheres[i].transform.RotateAround(Vector3.zero, Vector3.up, rotateSpeed * Time.deltaTime);

            // Change sphere size
            float scale = 0.5f + 0.5f * Mathf.Sin(time + i * 0.2f);
            spheres[i].transform.localScale = Vector3.one * scale;

            // Color change in a sine wave pattern
            Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();

            // Use sine wave to control color transitions- cyles between red pink and white 
            float colorSin = Mathf.Sin(time + i * 0.3f); // Color cycle speed
            colorSin = (colorSin + 1f) / 2f; // 0-1 range

            // Change between red, pink, and white based on the sine wave
            Color color = Color.Lerp(Color.red, new Color(1f, 0.4f, 0.7f), colorSin); // Interpolate between Red and Pink
            color = Color.Lerp(color, Color.white, Mathf.Abs(colorSin - 0.5f) * 2f); // Add transition to white

            sphereRenderer.material.color = color;
            sphereRenderer.material.SetColor("_EmissionColor", color * 2f); // Apply emission color
        }
    }
}
