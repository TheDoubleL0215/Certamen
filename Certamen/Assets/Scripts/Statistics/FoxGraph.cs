using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoxGraph : MonoBehaviour
{
    public Text foxCountText;

    public RectTransform chartPanel;
    public GameObject chartPointPrefab;

    private int foxObjectCount = 0;
    private List<int> foxEvolution = new List<int>();

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
            StoreFoxCount();
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

        if (foxObjectCount != GameObject.FindGameObjectsWithTag("Fox").Length)
        {
            foxObjectCount = GameObject.FindGameObjectsWithTag("Fox").Length;
            foxCountText.text = "Rókák száma: " + foxObjectCount;
            chartNeedsUpdate = true; // Update chart if counts changed
        }

    }

    private void StoreFoxCount()
    {
        if (foxEvolution.Count == 0 || foxEvolution[foxEvolution.Count - 1] != foxObjectCount)
        {
            foxEvolution.Add(foxObjectCount);

            if (foxEvolution.Count > maxDataPoints)
            {
                foxEvolution.RemoveAt(0);
            }

            chartNeedsUpdate = true;
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
                print("törölt vonalat");
            }
        }

        int totalPoints = foxEvolution.Count;
        float widthPerPoint = chartPanel.rect.width / Mathf.Max(totalPoints - 1, 1); // Calculate the width per data point

        // Reduce the distance between points by a factor (for example, dividing by 2)
        widthPerPoint /= 1.07f;

        for (int i = 0; i < totalPoints; i++)
        {
            float xPosition = i * widthPerPoint;
            float yPosition = foxEvolution[i] * 10 - 500;
            print("xy");
            

            // Draw lines between points (except for the first point)
            if (i > 0)
            {
                print("rajzol vonalat");
                GameObject line = new GameObject("Line");
                print("létrehoz vonalat");
                line.transform.SetParent(chartPanel, false);

                RectTransform lineRectTransform = line.AddComponent<RectTransform>();
                lineRectTransform.sizeDelta = new Vector2(widthPerPoint * 2, 10f); // Set line length and thickness

                Image lineImage = line.AddComponent<Image>();
                lineImage.color = Color.white; // Set line color
                print("szín is megvan határozva");

                // Position the line between two points
                Vector2 startPoint = new Vector2((i - 1) * widthPerPoint - 900, foxEvolution[i - 1] * 10 - 500);
                Vector2 endPoint = new Vector2(xPosition - 900, yPosition);

                lineRectTransform.anchoredPosition = Vector2.Lerp(startPoint, endPoint, 0.5f);
                lineRectTransform.sizeDelta = new Vector2(Vector2.Distance(startPoint, endPoint), 2f);

                Vector2 direction = endPoint - startPoint;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                lineRectTransform.rotation = Quaternion.Euler(0, 0, angle);
                print("pörög is");
            }
            // Instantiate a single chart point for each data point
            GameObject chartPoint = Instantiate(chartPointPrefab, chartPanel);
            RectTransform rectTransform = chartPoint.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(xPosition - 900, yPosition);
            chartPoints.Add(chartPoint);
        }
    }
}
