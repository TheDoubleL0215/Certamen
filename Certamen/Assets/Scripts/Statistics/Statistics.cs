using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Statistics : MonoBehaviour
{
    [Header("Stat fields")]
    // These texts will display the count of objedts
    public Text grassCountText;
    public Text rabbitCountText;
    public Text foxCountText;

    [Header("Chart Components")]
    public RectTransform chartPanel;
    public RectTransform foxChartPanel;
    public GameObject rabbitChartPointPrefab;
    public GameObject foxChartPointPrefab;
    public GameObject spiralPointPrefab;
    public RectTransform spiralPanel;

    private int grassObjectCount = 0;
    private int rabbitObjectCount = 0;
    private int foxObjectCount = 0;
    private List<int> rabbitEvolution = new List<int>();
    private List<int> foxEvolution = new List<int>();

    private float interval = 0.5f; // Store data interval
    private float timer = 0.0f;

    private List<GameObject> chartPoints = new List<GameObject>(); // Store chart points
    private List<GameObject> foxChartPoints = new List<GameObject>(); // Store chart points
    private List<RectTransform> spiralPointsList = new List<RectTransform>();

    private bool chartNeedsUpdate = false; // Flag to determine if the chart needs updating

    private const int maxDataPoints = 1000; // Maximum number of data points to keep

    private void Update()
    {
        // Update data at intervals
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0.0f;
            UpdateStatistics();
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
        if (grassObjectCount != GameObject.FindGameObjectsWithTag("Grass").Length)
        {
            grassObjectCount = GameObject.FindGameObjectsWithTag("Grass").Length;
            grassCountText.text = "Fûcsomók száma: " + grassObjectCount;
            chartNeedsUpdate = true; // Update chart if counts changed
        }

        if (foxObjectCount != GameObject.FindGameObjectsWithTag("Fox").Length || rabbitObjectCount != GameObject.FindGameObjectsWithTag("Rabbit").Length)
        {
            foxObjectCount = GameObject.FindGameObjectsWithTag("Fox").Length;
            foxCountText.text = "Rókák száma: " + foxObjectCount;
            StoreFoxCount();

            rabbitObjectCount = GameObject.FindGameObjectsWithTag("Rabbit").Length;
            rabbitCountText.text = "Nyulak száma: " + rabbitObjectCount;
            StoreRabbitCount();
        }
    }

    private void StoreRabbitCount()
    {
        //  stores the current number of rabbits
        rabbitEvolution.Add(rabbitObjectCount);

        if (rabbitEvolution.Count > maxDataPoints)
        {
            rabbitEvolution.RemoveAt(0);
        }

        chartNeedsUpdate = true;
    }

    private void StoreFoxCount()
    {
        //  stores the current number of rabbits
        foxEvolution.Add(foxObjectCount);

        if (foxEvolution.Count > maxDataPoints)
        {
            foxEvolution.RemoveAt(0);
        }

        chartNeedsUpdate = true;
    }

    void UpdateSpiralDiagram()
    {
        float x, y;
        y = foxObjectCount; // Adjust as needed
        x = rabbitObjectCount; // Adjust as needed

        if (x != 0)
        {
            x *= 15f;
        }
        if (y != 0)
        {
            y *= 30f;
        }

        // Create and position a point at the calculated position on the spiral
        GameObject spiralPoint = Instantiate(spiralPointPrefab, spiralPanel);

        RectTransform rectTransform = spiralPoint.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(x - 900, y - 500);
        spiralPointsList.Add(rectTransform);

        // Draw lines between points (except for the first point)
        if (spiralPanel.childCount > 1)
        {
            GameObject line = new GameObject("SpiralLine");
            line.transform.SetParent(spiralPanel, false);

            RectTransform lineRectTransform = line.AddComponent<RectTransform>();
            lineRectTransform.sizeDelta = new Vector2(2f, 2f); // Set line thickness

            Image lineImage = line.AddComponent<Image>();
            lineImage.color = Color.green; // Set line color

            // Get the current and previous points
            RectTransform prevPoint = spiralPointsList[spiralPointsList.Count - 2];
            RectTransform currentPoint = spiralPoint.GetComponent<RectTransform>();

            // Set line position
            Vector2 startPoint = prevPoint.anchoredPosition;
            Vector2 endPoint = currentPoint.anchoredPosition;

            // Calculate midpoint and distance
            Vector2 midPoint = (startPoint + endPoint) / 2f;
            float distance = Vector2.Distance(startPoint, endPoint);

            // Set line position and length
            lineRectTransform.anchoredPosition = midPoint;
            lineRectTransform.sizeDelta = new Vector2(distance, 2f);

            // Set line rotation
            Vector2 direction = (endPoint - startPoint).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            lineRectTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void UpdateChart()
    {
        UpdateSpiralDiagram();
        //RABBIT CHART//
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
            GameObject chartPoint = Instantiate(rabbitChartPointPrefab, chartPanel);
            RectTransform rectTransform = chartPoint.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(xPosition - 900, yPosition);
            chartPoints.Add(chartPoint);
        }



        //FOX CHART//
        // Clear all previous chart points and lines
        foreach (var foxChartPoint in foxChartPoints)
        {
            Destroy(foxChartPoint);
        }
        foxChartPoints.Clear();

        // Clear all previous lines
        var foxLines = foxChartPanel.GetComponentsInChildren<Image>();
        foreach (var foxLine in foxLines)
        {
            if (foxLine.gameObject.name == "FoxLine")
            {
                Destroy(foxLine.gameObject);
            }
        }

        int foxTotalPoints = foxEvolution.Count;
        float foxWidthPerPoint = chartPanel.rect.width / Mathf.Max(totalPoints - 1, 1); // Calculate the width per data point

        // Reduce the distance between points by a factor (for example, dividing by 2)
        foxWidthPerPoint /= 1.07f;

        for (int i = 0; i < foxTotalPoints; i++)
        {
            float foxXPosition = i * foxWidthPerPoint;
            float foxYPosition = foxEvolution[i] * 10 - 500;


            // Draw lines between points (except for the first point)
            if (i > 0)
            {
                GameObject foxLine = new GameObject("FoxLine");
                foxLine.transform.SetParent(foxChartPanel, false);

                RectTransform foxLineRectTransform = foxLine.AddComponent<RectTransform>();
                foxLineRectTransform.sizeDelta = new Vector2(foxWidthPerPoint * 2, 10f); // Set line length and thickness

                Image foxLineImage = foxLine.AddComponent<Image>();
                foxLineImage.color = Color.red; // Set line color

                // Position the line between two points
                Vector2 foxStartPoint = new Vector2((i - 1) * foxWidthPerPoint - 900, foxEvolution[i - 1] * 10 - 500);
                Vector2 foxEndPoint = new Vector2(foxXPosition - 900, foxYPosition);

                foxLineRectTransform.anchoredPosition = Vector2.Lerp(foxStartPoint, foxEndPoint, 0.5f);
                foxLineRectTransform.sizeDelta = new Vector2(Vector2.Distance(foxStartPoint, foxEndPoint), 2f);

                Vector2 foxDirection = foxEndPoint - foxStartPoint;
                float foxAngle = Mathf.Atan2(foxDirection.y, foxDirection.x) * Mathf.Rad2Deg;
                foxLineRectTransform.rotation = Quaternion.Euler(0, 0, foxAngle);
            }
            // Instantiate a single chart point for each data point
            GameObject foxChartPoint = Instantiate(foxChartPointPrefab, foxChartPanel);
            RectTransform rectTransform = foxChartPoint.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(foxXPosition - 900, foxYPosition);
            foxChartPoints.Add(foxChartPoint);
        }
    }
}
