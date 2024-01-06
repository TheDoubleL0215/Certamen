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
    public RectTransform rPopPanel;
    public RectTransform fPopPanel;
    public RectTransform fAttrPanel;
    public RectTransform rAttrPanel;
    public GameObject rPopChartPoint;
    public GameObject fPopChartPoint;
    public GameObject rAttrChartPoint;
    public GameObject fAttrChartPoint;

    [Header("Attributes")]
    public List<string> attributeNames = new List<string> { "speed", "hungerLevel", "radius", "fertility", "hungerLoss", "hungerMax"};


    private int grassObjectCount = 0;
    private int rabbitObjectCount = 0;
    private int foxObjectCount = 0;

    private List<float> foxAttributeAverages = new List<float>();
    private List<float> rabbitAttributeAverages = new List<float>();
    private List<int> rabbitEvolution = new List<int>();
    private List<int> foxEvolution = new List<int>();

    private float interval = 0.5f; // Store data interval
    private float timer = 0.0f;
    public float attrDivider = 40f;
    public float attrSubtrahend = 750f;
    public float popDivider = 10f;
    public float popSubtrahend = 500f;

    private List<GameObject> rPopPointList = new List<GameObject>(); // Store chart points
    private List<GameObject> fPopPointList = new List<GameObject>(); // Store chart points
    private List<GameObject> rAttrPointList = new List<GameObject>();
    private List<GameObject> fAttrPointList = new List<GameObject>();

    private bool chartNeedsUpdate = false; // Flag to determine if the chart needs updating

    private const int maxDataPoints = 1000; // Maximum number of data points to keep

    private bool showP = false;

    private bool showO = false;

    private void Update()
    {
        // Update data at intervals
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0.0f;
            UpdateStatistics();
        }

        if (Input.GetKey(KeyCode.P) && showP == false){
            showP = true;
            showO = false;
        }
        else{
            showP = false;
        }

        if (Input.GetKey(KeyCode.O) && showO == false){
            showO = true;
            showP = false;
        }
        else{
            showO = false;
        }

        // Update chart less frequently
        if(chartNeedsUpdate){
            if (showP){
                UpdatePopNumChart();
                chartNeedsUpdate = false;
            }
            if(showO){
                UpdateAttrChart();
                chartNeedsUpdate = false;
            }
        }
    }

    private void UpdateStatistics()
    {
        // Only update the counts if necessary (when objects are added/removed)
        if (grassObjectCount != GameObject.FindGameObjectsWithTag("Grass").Length)
        {
            grassObjectCount = GameObject.FindGameObjectsWithTag("Grass").Length;
            grassCountText.text = "Fűcsomók száma: " + grassObjectCount;
        }

        if (foxObjectCount != GameObject.FindGameObjectsWithTag("Fox").Length || rabbitObjectCount != GameObject.FindGameObjectsWithTag("Rabbit").Length)
        {
            foxObjectCount = GameObject.FindGameObjectsWithTag("Fox").Length;
            foxCountText.text = "Rókák száma: " + foxObjectCount;
            UpdateFoxData(attributeNames[0]);

            rabbitObjectCount = GameObject.FindGameObjectsWithTag("Rabbit").Length;
            rabbitCountText.text = "Nyulak száma: " + rabbitObjectCount;
            UpdateRabbitData(attributeNames[0]);
        }
    }

    private void UpdateRabbitData(string attributeName)
    {
        //  stores the current number of rabbits
        rabbitEvolution.Add(rabbitObjectCount);

        if (rabbitEvolution.Count > maxDataPoints)
        {
            rabbitEvolution.RemoveAt(0);
        }

        GameObject[] rabbitObjects = GameObject.FindGameObjectsWithTag("Rabbit");

        float totalXAttributes = 0f;

        // Loop through all the rabbit objects
        foreach (GameObject rabbitObject in rabbitObjects)
        {
            // Access the script containing the X attribute
            rabbitManagerScript rabbitScript = rabbitObject.GetComponent<rabbitManagerScript>();

            if (rabbitScript != null)
            {
                System.Type rabbitType = rabbitScript.GetType();
                System.Reflection.FieldInfo field = rabbitType.GetField(attributeName);

                if (field != null)
                {
                    object attributeValue = field.GetValue(rabbitScript);
                    totalXAttributes += (float)attributeValue;
                }
            }
        }

        float averageXAttribute = rabbitObjectCount > 0 ? totalXAttributes / rabbitObjectCount : 0f;

    
        rabbitAttributeAverages.Add(averageXAttribute);

        if (rabbitAttributeAverages.Count > maxDataPoints)
        {
            rabbitAttributeAverages.RemoveAt(0);
        }

        chartNeedsUpdate = true;
    }

    private void UpdateFoxData(string attributeName)
    {
        //  stores the current number of rabbits
        foxEvolution.Add(foxObjectCount);

        if (foxEvolution.Count > maxDataPoints)
        {
            foxEvolution.RemoveAt(0);
        }

        GameObject[] foxObjects = GameObject.FindGameObjectsWithTag("Fox");

        float totalXAttributes = 0f;

        // Loop through all the fox objects
        foreach (GameObject foxObject in foxObjects)
        {
            // Access the script containing the X attribute
            FoxManager foxScript = foxObject.GetComponent<FoxManager>();

            if (foxScript != null)
            {
                System.Type foxType = foxScript.GetType();
                System.Reflection.FieldInfo field = foxType.GetField(attributeName);

                if (field != null)
                {
                    object attributeValue = field.GetValue(foxScript);
                    totalXAttributes += (float)attributeValue;
                }
            }
        }

        float averageXAttribute = foxObjectCount > 0 ? totalXAttributes / foxObjectCount : 0f;

        foxAttributeAverages.Add(averageXAttribute);

        if (foxAttributeAverages.Count > maxDataPoints)
        {
            foxAttributeAverages.RemoveAt(0);
        }
        chartNeedsUpdate = true;
    }


    void UpdateAttrChart()
    {
        //RABBITS
        // Clear all previous chart points and lines
        foreach (var chartPoint in rAttrPointList)
        {
            Destroy(chartPoint);
        }
        rAttrPointList.Clear();

        // Clear all previous lines
        var lines = rAttrPanel.GetComponentsInChildren<Image>();
        foreach (var line in lines)
        {
            if (line.gameObject.name == "Line")
            {
                Destroy(line.gameObject);
            }
        }

        int totalPoints = rabbitAttributeAverages.Count;
        float widthPerPoint = rAttrPanel.rect.width / Mathf.Max(totalPoints - 1, 1); // Calculate the width per data point

        // Reduce the distance between points by a factor (for example, dividing by 2)
        widthPerPoint /= 1.07f;

        for (int i = 0; i < totalPoints; i++)
        {
            float xPosition = i * widthPerPoint;
            float yPosition = rabbitAttributeAverages[i] * attrDivider - attrSubtrahend;


            // Draw lines between points (except for the first point)
            if (i > 0)
            {
                GameObject line = new GameObject("Line");
                line.transform.SetParent(rAttrPanel, false);

                RectTransform lineRectTransform = line.AddComponent<RectTransform>();
                lineRectTransform.sizeDelta = new Vector2(widthPerPoint * 2, 10f); // Set line length and thickness

                Image lineImage = line.AddComponent<Image>();
                lineImage.color = Color.white; // Set line color

                // Position the line between two points
                Vector2 startPoint = new Vector2((i - 1) * widthPerPoint - 900, rabbitAttributeAverages[i - 1] * attrDivider - attrSubtrahend);
                Vector2 endPoint = new Vector2(xPosition - 900, yPosition);

                lineRectTransform.anchoredPosition = Vector2.Lerp(startPoint, endPoint, 0.5f);
                lineRectTransform.sizeDelta = new Vector2(Vector2.Distance(startPoint, endPoint), 2f);

                Vector2 direction = endPoint - startPoint;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                lineRectTransform.rotation = Quaternion.Euler(0, 0, angle);
            }
            // Instantiate a single chart point for each data point
            GameObject rChartPoint = Instantiate(rAttrChartPoint, rAttrPanel);
            RectTransform rectTransform = rChartPoint.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(xPosition - 900, yPosition);
            rAttrPointList.Add(rChartPoint);
        }



        //FOXES
        // Clear all previous chart points and lines
        foreach (var chartPoint in fAttrPointList)
        {
            Destroy(chartPoint);
        }
        fAttrPointList.Clear();

        // Clear all previous lines
        var foxLines = fAttrPanel.GetComponentsInChildren<Image>();
        foreach (var line in foxLines)
        {
            if (line.gameObject.name == "Line")
            {
                Destroy(line.gameObject);
            }
        }

        for (int i = 0; i < totalPoints; i++)
        {
            float xPosition = i * widthPerPoint;
            float yPosition = foxAttributeAverages[i] * attrDivider - attrSubtrahend;


            // Draw lines between points (except for the first point)
            if (i > 0)
            {
                GameObject line = new GameObject("Line");
                line.transform.SetParent(fAttrPanel, false);

                RectTransform lineRectTransform = line.AddComponent<RectTransform>();
                lineRectTransform.sizeDelta = new Vector2(widthPerPoint * 2, 10f); // Set line length and thickness

                Image lineImage = line.AddComponent<Image>();
                lineImage.color = Color.red; // Set line color

                // Position the line between two points
                Vector2 startPoint = new Vector2((i - 1) * widthPerPoint - 900, foxAttributeAverages[i - 1] * attrDivider - attrSubtrahend);
                Vector2 endPoint = new Vector2(xPosition - 900, yPosition);

                lineRectTransform.anchoredPosition = Vector2.Lerp(startPoint, endPoint, 0.5f);
                lineRectTransform.sizeDelta = new Vector2(Vector2.Distance(startPoint, endPoint), 2f);

                Vector2 direction = endPoint - startPoint;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                lineRectTransform.rotation = Quaternion.Euler(0, 0, angle);
            }
            // Instantiate a single chart point for each data point
            GameObject fChartPoint = Instantiate(fAttrChartPoint, fAttrPanel);
            RectTransform rectTransform = fChartPoint.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(xPosition - 900, yPosition);
            fAttrPointList.Add(fChartPoint);
        }
    }

    void UpdatePopNumChart()
    {
        //RABBIT CHART//
        // Clear all previous chart points and lines
        foreach (var chartPoint in rPopPointList)
        {
            Destroy(chartPoint);
        }
        rPopPointList.Clear();

        // Clear all previous lines
        var rabbitLines = rPopPanel.GetComponentsInChildren<Image>();
        foreach (var line in rabbitLines)
        {
            if (line.gameObject.name == "Line")
            {
                Destroy(line.gameObject);
            }
        }

        int totalPoints = rabbitEvolution.Count;
        float widthPerPoint = rPopPanel.rect.width / Mathf.Max(totalPoints - 1, 1); // Calculate the width per data point

        // Reduce the distance between points by a factor (for example, dividing by 2)
        widthPerPoint /= 1.07f;

        for (int i = 0; i < totalPoints; i++)
        {
            float xPosition = i * widthPerPoint;
            float yPosition = rabbitEvolution[i] * popDivider - popSubtrahend;


            // Draw lines between points (except for the first point)
            if (i > 0)
            {
                GameObject line = new GameObject("Line");
                line.transform.SetParent(rPopPanel, false);

                RectTransform lineRectTransform = line.AddComponent<RectTransform>();
                lineRectTransform.sizeDelta = new Vector2(widthPerPoint * 2, 10f); // Set line length and thickness

                Image lineImage = line.AddComponent<Image>();
                lineImage.color = Color.white; // Set line color

                // Position the line between two points
                Vector2 startPoint = new Vector2((i - 1) * widthPerPoint - 900, rabbitEvolution[i - 1] * popDivider - popSubtrahend);
                Vector2 endPoint = new Vector2(xPosition - 900, yPosition);

                lineRectTransform.anchoredPosition = Vector2.Lerp(startPoint, endPoint, 0.5f);
                lineRectTransform.sizeDelta = new Vector2(Vector2.Distance(startPoint, endPoint), 2f);

                Vector2 direction = endPoint - startPoint;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                lineRectTransform.rotation = Quaternion.Euler(0, 0, angle);
            }
            // Instantiate a single chart point for each data point
            GameObject rChartPoint = Instantiate(rPopChartPoint, rPopPanel);
            RectTransform rectTransform = rChartPoint.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(xPosition - 900, yPosition);
            rPopPointList.Add(rChartPoint);
        }



        //FOX CHART//
        // Clear all previous chart points and lines
        foreach (var chartPoint in fPopPointList)
        {
            Destroy(chartPoint);
        }
        fPopPointList.Clear();

        // Clear all previous lines
        var foxLines = fPopPanel.GetComponentsInChildren<Image>();
        foreach (var line in foxLines)
        {
            if (line.gameObject.name == "Line")
            {
                Destroy(line.gameObject);
            }
        }

        for (int i = 0; i < totalPoints; i++)
        {
            float foxXPosition = i * widthPerPoint;
            float foxYPosition = foxEvolution[i] * popDivider - popSubtrahend;


            // Draw lines between points (except for the first point)
            if (i > 0)
            {
                GameObject foxLine = new GameObject("Line");
                foxLine.transform.SetParent(rPopPanel, false);

                RectTransform foxLineRectTransform = foxLine.AddComponent<RectTransform>();
                foxLineRectTransform.sizeDelta = new Vector2(widthPerPoint * 2, 10f); // Set line length and thickness

                Image foxLineImage = foxLine.AddComponent<Image>();
                foxLineImage.color = Color.red; // Set line color

                // Position the line between two points
                Vector2 foxStartPoint = new Vector2((i - 1) * widthPerPoint - 900, foxEvolution[i - 1] * popDivider - popSubtrahend);
                Vector2 foxEndPoint = new Vector2(foxXPosition - 900, foxYPosition);

                foxLineRectTransform.anchoredPosition = Vector2.Lerp(foxStartPoint, foxEndPoint, 0.5f);
                foxLineRectTransform.sizeDelta = new Vector2(Vector2.Distance(foxStartPoint, foxEndPoint), 2f);

                Vector2 foxDirection = foxEndPoint - foxStartPoint;
                float foxAngle = Mathf.Atan2(foxDirection.y, foxDirection.x) * Mathf.Rad2Deg;
                foxLineRectTransform.rotation = Quaternion.Euler(0, 0, foxAngle);
            }
            // Instantiate a single chart point for each data point
            GameObject fChartPoint = Instantiate(fPopChartPoint, fPopPanel);
            RectTransform rectTransform = fChartPoint.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(foxXPosition - 900, foxYPosition);
            fPopPointList.Add(fChartPoint);
        }
    }
}
