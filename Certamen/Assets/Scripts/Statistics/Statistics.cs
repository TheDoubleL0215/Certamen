using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Statistics : MonoBehaviour
{
    public Text grassCountText;
    public Text rabbitCountText;

    public RectTransform chartPanel;
    public GameObject chartPointPrefab;

    private int grassObjectCount = 0;
    private int rabbitObjectCount = 0;
    private List<int> rabbitEvolution = new List<int>();

    private float interval = 0.5f; // Store data interval
    private float timer = 0.0f;

    private List<GameObject> chartPoints = new List<GameObject>(); // Store chart points

    private bool chartNeedsUpdate = false; // Flag to determine if the chart needs updating

    private const int maxDataPoints = 1000; // Maximum number of data points to keep

    private void Update()
    {
        UpdateStatistics();

        // Update data at intervals
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0.0f;
            StoreRabbitCount();
        }

        // Update chart less frequently
        if (chartNeedsUpdate)
        {
            UpdateChart();
            chartNeedsUpdate = false;
        }
    }

    private void UpdateStatistics()
    {
        // Only update the counts if necessary (when objects are added/removed)
        if (grassObjectCount != GameObject.FindGameObjectsWithTag("Grass").Length ||
            rabbitObjectCount != GameObject.FindGameObjectsWithTag("Rabbit").Length)
        {
            grassObjectCount = GameObject.FindGameObjectsWithTag("Grass").Length;
            rabbitObjectCount = GameObject.FindGameObjectsWithTag("Rabbit").Length;

            grassCountText.text = "Fûcsomók száma: " + grassObjectCount;
            rabbitCountText.text = "Nyulak száma: " + rabbitObjectCount;

            chartNeedsUpdate = true; // Update chart if counts changed
        }
    }

    private void StoreRabbitCount()
    {
        // Only store if the count has changed
        if (rabbitEvolution.Count == 0 || rabbitEvolution[rabbitEvolution.Count - 1] != rabbitObjectCount)
        {
            rabbitEvolution.Add(rabbitObjectCount);

            // Keep data points within limit
            if (rabbitEvolution.Count > maxDataPoints)
            {
                rabbitEvolution.RemoveAt(0); // Remove oldest data point
            }

            chartNeedsUpdate = true; // Update chart with new data
        }
    }

    void UpdateChart()
    {
        // Clear all previous chart points and lines
        foreach (var chartPoint in chartPoints)
        {
            Destroy(chartPoint);
        }
        chartPoints.Clear();

        // Clear all previous lines
        var lines = chartPanel.GetComponentsInChildren<Image>();
        foreach (var line in lines)
        {
            if (line.gameObject.name == "Line")
            {
                Destroy(line.gameObject);
            }
        }

        int totalPoints = rabbitEvolution.Count;
        float widthPerPoint = chartPanel.rect.width / Mathf.Max(totalPoints - 1, 1); // Calculate the width per data point

        // Reduce the distance between points by a factor (for example, dividing by 2)
        widthPerPoint /= 1.07f;

        for (int i = 0; i < totalPoints; i++)
        {
            float xPosition = i * widthPerPoint;
            float yPosition = rabbitEvolution[i] * 10 - 500;


            // Draw lines between points (except for the first point)
            if (i > 0)
            {
                GameObject line = new GameObject("Line");
                line.transform.SetParent(chartPanel, false);

                RectTransform lineRectTransform = line.AddComponent<RectTransform>();
                lineRectTransform.sizeDelta = new Vector2(widthPerPoint * 2, 10f); // Set line length and thickness

                Image lineImage = line.AddComponent<Image>();
                lineImage.color = Color.white; // Set line color

                // Position the line between two points
                Vector2 startPoint = new Vector2((i - 1) * widthPerPoint - 900, rabbitEvolution[i - 1] * 10 - 500);
                Vector2 endPoint = new Vector2(xPosition - 900, yPosition);

                lineRectTransform.anchoredPosition = Vector2.Lerp(startPoint, endPoint, 0.5f);
                lineRectTransform.sizeDelta = new Vector2(Vector2.Distance(startPoint, endPoint), 2f);

                Vector2 direction = endPoint - startPoint;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                lineRectTransform.rotation = Quaternion.Euler(0, 0, angle);
            }

            // Instantiate a single chart point for each data point
            GameObject chartPoint = Instantiate(chartPointPrefab, chartPanel);
            RectTransform rectTransform = chartPoint.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(xPosition - 900, yPosition);
            chartPoints.Add(chartPoint);

            
        }
    }
}
