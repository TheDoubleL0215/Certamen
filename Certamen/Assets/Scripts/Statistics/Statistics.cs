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
    public RectTransform attrHLP;
    public RectTransform popHLP;
    private const int maxDataPoints = 1000; // Maximum number of data points to keep
    private int currentDataPoints = 0;

    [Header("Attributes")]
    float[,] rAttributeMatrix = new float[6, maxDataPoints];
    float[,] fAttributeMatrix = new float[6, maxDataPoints];
    public int choosenAttribute = 0;

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
    public float Subtrahend = 500f;
    public float popDivider = 10f;

    private List<GameObject> rPopPointList = new List<GameObject>(); // Store chart points
    private List<GameObject> fPopPointList = new List<GameObject>(); // Store chart points
    private List<GameObject> rAttrPointList = new List<GameObject>();
    private List<GameObject> fAttrPointList = new List<GameObject>();

    private bool chartNeedsUpdate = false; // Flag to determine if the chart needs updating


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
            UpdateFoxData();

            rabbitObjectCount = GameObject.FindGameObjectsWithTag("Rabbit").Length;
            rabbitCountText.text = "Nyulak száma: " + rabbitObjectCount;
            UpdateRabbitData();
        }
    }

    private void UpdateRabbitData()
    {
        //  stores the current number of rabbits
        rabbitEvolution.Add(rabbitObjectCount);

        if (rabbitEvolution.Count > maxDataPoints)
        {
            rabbitEvolution.RemoveAt(0);
        }

        GameObject[] rabbitObjects = GameObject.FindGameObjectsWithTag("Rabbit");

        float overallSpeed = 0f;
        float overallHungerLevel = 0f;
        float overallRadius = 0f;
        float overallFertility = 0f;
        float overallHungerLoss = 0f;
        float overallHungerMax = 0f;

        // Loop through all the rabbit objects
        foreach (GameObject rabbitObject in rabbitObjects)
        {
            // Access the script containing the X attribute
            rabbitManagerScript rabbitScript = rabbitObject.GetComponent<rabbitManagerScript>();

            if (rabbitScript != null)
            {
                overallSpeed += rabbitScript.baseSpeed;
                overallHungerLevel += rabbitScript.hungerLevel;
                overallRadius += rabbitScript.baseRadius;
                overallFertility += rabbitScript.fertility;
                overallHungerLoss += rabbitScript.hungerLoss;
                overallHungerMax += rabbitScript.baseHungerMax;
            }
        }

        float averageSpeed = rabbitObjectCount > 0 ? overallSpeed / rabbitObjectCount : 0f;
        float averageHungerLevel = rabbitObjectCount > 0 ? overallHungerLevel / rabbitObjectCount : 0f;
        float averageRadius = rabbitObjectCount > 0 ? overallRadius / rabbitObjectCount : 0f;
        float averageFertility = rabbitObjectCount > 0 ? overallFertility / rabbitObjectCount : 0f;
        float averageHungerLoss = rabbitObjectCount > 0 ? overallHungerLoss / rabbitObjectCount : 0f;
        float averageHungerMax = rabbitObjectCount > 0 ? overallHungerMax / rabbitObjectCount : 0f;
    
        rAttributeMatrix[0, currentDataPoints] = averageSpeed;
        rAttributeMatrix[1, currentDataPoints] = averageHungerLevel;
        rAttributeMatrix[2, currentDataPoints] = averageRadius;
        rAttributeMatrix[3, currentDataPoints] = averageFertility;
        rAttributeMatrix[4, currentDataPoints] = averageHungerLoss;
        rAttributeMatrix[5, currentDataPoints] = averageHungerMax;


        if (rAttributeMatrix.GetLength(1) > maxDataPoints)
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < maxDataPoints - 1; j++)
                {
                    rAttributeMatrix[i, j] = rAttributeMatrix[i, j + 1];
                }
            }
        }
        else{
            currentDataPoints += 1;
        }
        chartNeedsUpdate = true;
    }

    private void UpdateFoxData()
    {
        //  stores the current number of rabbits
        foxEvolution.Add(foxObjectCount);

        if (foxEvolution.Count > maxDataPoints)
        {
            foxEvolution.RemoveAt(0);
        }

        GameObject[] foxObjects = GameObject.FindGameObjectsWithTag("Fox");

        float overallSpeed = 0f;
        float overallHungerLevel = 0f;
        float overallRadius = 0f;
        float overallFertility = 0f;
        float overallHungerLoss = 0f;
        float overallHungerMax = 0f;

        // Loop through all the fox objects
        foreach (GameObject foxObject in foxObjects)
        {
            // Access the script containing the X attribute
            FoxManager foxScript = foxObject.GetComponent<FoxManager>();

            if (foxScript != null)
            {
                overallSpeed += foxScript.baseSpeed;
                overallHungerLevel += foxScript.hungerLevel;
                overallRadius += foxScript.baseRadius;
                overallFertility += foxScript.fertility;
                overallHungerLoss += foxScript.hungerLoss;
                overallHungerMax += foxScript.baseHungerMax;
            }
        }

        float averageSpeed = foxObjectCount > 0 ? overallSpeed / foxObjectCount : 0f;
        float averageHungerLevel = foxObjectCount > 0 ? overallHungerLevel / foxObjectCount : 0f;
        float averageRadius = foxObjectCount > 0 ? overallRadius / foxObjectCount : 0f;
        float averageFertility = foxObjectCount > 0 ? overallFertility / foxObjectCount : 0f;
        float averageHungerLoss = foxObjectCount > 0 ? overallHungerLoss / foxObjectCount : 0f;
        float averageHungerMax = foxObjectCount > 0 ? overallHungerMax / foxObjectCount : 0f;

        fAttributeMatrix[0, currentDataPoints] = averageSpeed;
        fAttributeMatrix[1, currentDataPoints] = averageHungerLevel;
        fAttributeMatrix[2, currentDataPoints] = averageRadius;
        fAttributeMatrix[3, currentDataPoints] = averageFertility;
        fAttributeMatrix[4, currentDataPoints] = averageHungerLoss;
        fAttributeMatrix[5, currentDataPoints] = averageHungerMax;

        if (fAttributeMatrix.GetLength(1) > maxDataPoints)
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < maxDataPoints - 1; j++)
                {
                    fAttributeMatrix[i, j] = fAttributeMatrix[i, j + 1];
                }
            }
        }
        chartNeedsUpdate = true;
    }


    void UpdateAttrChart()
    {
        float highestValue = 0f;

        for (int i = 0; i < currentDataPoints; i++){
            if(rAttributeMatrix[choosenAttribute, i] > highestValue){
                highestValue = rAttributeMatrix[choosenAttribute, i];
            }
            if(fAttributeMatrix[choosenAttribute, i] > highestValue){
                highestValue = fAttributeMatrix[choosenAttribute, i];
            }
        }
        
        attrDivider =  750f/(highestValue + (10 - highestValue % 10));
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

        float widthPerPoint = rAttrPanel.rect.width / Mathf.Max(currentDataPoints - 1, 1); // Calculate the width per data point

        // Reduce the distance between points by a factor (for example, dividing by 2)
        widthPerPoint /= 1.07f;

        for (int i = 0; i < currentDataPoints; i++)
        {
            float xPosition = i * widthPerPoint;
            float yPosition = rAttributeMatrix[choosenAttribute, i] * attrDivider - Subtrahend;


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
                Vector2 startPoint = new Vector2((i - 1) * widthPerPoint - 900, rAttributeMatrix[choosenAttribute, i-1] * attrDivider - Subtrahend);
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

        for (int i = 0; i < currentDataPoints; i++)
        {
            float xPosition = i * widthPerPoint;
            float yPosition = fAttributeMatrix[choosenAttribute, i] * attrDivider - Subtrahend;


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
                Vector2 startPoint = new Vector2((i - 1) * widthPerPoint - 900, fAttributeMatrix[choosenAttribute, i-1] * attrDivider - Subtrahend);
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

        RectTransform graphPanel = fAttrPanel;

        var helpingLines = attrHLP.GetComponentsInChildren<Image>();
        foreach (var line in helpingLines)
        {
            if (line.gameObject.name == "HelpingLine")
            {
                Destroy(line.gameObject);
            }
        }


        // Set the spacing between lines
        float lineSpacing = 10f;

        // Calculate the number of lines needed
        int numLines = Mathf.FloorToInt(highestValue / lineSpacing) + 1;

        // Loop through and add lines at the specified intervals
        for (int i = 0; i < numLines + 1; i++)
        {
            // Calculate the y position for the line
            float yPosition = (i * lineSpacing) * attrDivider - Subtrahend;

            // Create a GameObject for the line
            GameObject line = new GameObject("HelpingLine");
            line.transform.SetParent(attrHLP, false);

            RectTransform lineRectTransform = line.AddComponent<RectTransform>();
            lineRectTransform.sizeDelta = new Vector2(graphPanel.rect.width-75, 2f); // Set line length and thickness

            // Position the line
            lineRectTransform.anchoredPosition = new Vector2(0, yPosition);

            Image lineImage = line.AddComponent<Image>();
            lineImage.color = Color.grey; // Set line color
        }


    }

    void UpdatePopNumChart()
    {
        float highestValue = 0f;

        for (int i = 0; i < currentDataPoints; i++){
            if(rabbitEvolution[i] > highestValue){
                highestValue = rabbitEvolution[i];
            }
            if(foxEvolution[i] > highestValue){
                highestValue = foxEvolution[i];
            }
        }
        
        popDivider = 750f/(highestValue + (10 - highestValue % 10));

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

        float widthPerPoint = rPopPanel.rect.width / Mathf.Max(currentDataPoints - 1, 1); // Calculate the width per data point

        // Reduce the distance between points by a factor (for example, dividing by 2)
        widthPerPoint /= 1.07f;

        for (int i = 0; i < currentDataPoints; i++)
        {
            float xPosition = i * widthPerPoint;
            float yPosition = rabbitEvolution[i] * popDivider - Subtrahend;


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
                Vector2 startPoint = new Vector2((i - 1) * widthPerPoint - 900, rabbitEvolution[i - 1] * popDivider - Subtrahend);
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

        for (int i = 0; i < currentDataPoints; i++)
        {
            float foxXPosition = i * widthPerPoint;
            float foxYPosition = foxEvolution[i] * popDivider - Subtrahend;


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
                Vector2 foxStartPoint = new Vector2((i - 1) * widthPerPoint - 900, foxEvolution[i - 1] * popDivider - Subtrahend);
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

        RectTransform graphPanel = fPopPanel;

        var helpingLines = popHLP.GetComponentsInChildren<Image>();
        foreach (var line in helpingLines)
        {
            if (line.gameObject.name == "HelpingLine")
            {
                Destroy(line.gameObject);
            }
        }


        // Set the spacing between lines
        float lineSpacing = 10f;

        // Calculate the number of lines needed
        int numLines = Mathf.FloorToInt(highestValue / lineSpacing) + 1;

        // Loop through and add lines at the specified intervals
        for (int i = 0; i < numLines + 1; i++)
        {
            // Calculate the y position for the line
            float yPosition = (i * lineSpacing) * popDivider - Subtrahend;

            // Create a GameObject for the line
            GameObject line = new GameObject("HelpingLine");
            line.transform.SetParent(popHLP, false);

            RectTransform lineRectTransform = line.AddComponent<RectTransform>();
            lineRectTransform.sizeDelta = new Vector2(graphPanel.rect.width-75, 2f); // Set line length and thickness

            // Position the line
            lineRectTransform.anchoredPosition = new Vector2(0, yPosition);

            Image lineImage = line.AddComponent<Image>();
            lineImage.color = Color.grey; // Set line color
        }
    }
}
