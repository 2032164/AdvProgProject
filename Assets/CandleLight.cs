using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;
public class CandleLight : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject candle;
public float maxTestValue = 1.0f;
public float minTestValue = 0.0f;

    private float rand;
    private Light candleLight;

    [Header("Fire Color Flicker")]
    public Color minFireColor = new Color(1.0f, 0.35f, 0.02f);
    public Color maxFireColor = new Color(1.0f, 0.7f, 0.15f);
    public float colorFlickerSpeed = 4.0f;
    public float colorFlickerAmount = 0.08f;
    void Start()
    {
        candleLight = candle.GetComponent<Light>();
        rand = Random.Range(0.8f, 1.8f);
        candleLight.intensity = rand;
        candleLight.color = Color.Lerp(minFireColor, maxFireColor, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        candleLight.intensity = (rand-0.2f) + (Mathf.Sin(Time.time ) + rand*2.0f)*0.25f;

        float baseT = Mathf.PerlinNoise(Time.time * colorFlickerSpeed, rand * 10.0f);
        float jitter = Random.Range(-colorFlickerAmount, colorFlickerAmount);
        float colorT = Mathf.Clamp01(baseT + jitter);
        candleLight.color = Color.Lerp(minFireColor, maxFireColor, colorT);
    }
}
