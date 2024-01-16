using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

#if UNITY_EDITOR
using UnityEditor;
#endif
using Random=UnityEngine.Random;



[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class terrainGeneratorScript : MonoBehaviour
{
    Mesh mesh;
    MeshCollider meshCollider; // Hozzáadott sor

    public int width;
    public int height;
    Color[] colors;
    

    public bool randomSeed = true;
    public int seed = 10;

    private float xOffset;
    private float yOffset;

    [Header("Gizmos")]
    [SerializeField] private bool showGizmosSphere = false;  //VIGYÁZZ!!!!! A TELJESÍTMÉNY DRASZTIKUSAN CSÖKKENHET

    private int verticesScale = 2;

    [Header("Perlin Noise Settings")]
    [Range(1, 10)]
    public float scale = 20;
    [Range(1, 100)]
    public float heightMapping = 50;
    public Gradient gradient;

    public NavMeshSurface surface;
    private float minTerrainHeight;
    private float maxTerrainHeight;

    Vector3[] vertices;
    int[] triangles;

    [Header("Environment")]
    public GameObject treePrefab;
    [Range(1, 30)]
    public float treeDensity ;

    public float lowestX;
    public float highestX;
    public float lowestZ;
    public float highestZ;

    public Transform environmentParentObj;



    void OnLoad()
    {
        seed = Random.Range(1, 100000);
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = GetComponent<MeshCollider>(); // Hozzáadott sor


    }

    void OnValidate(){
        minTerrainHeight = 0f;
        maxTerrainHeight = 0f;


        meshCollider = GetComponent<MeshCollider>(); // Hozzáadott sor

        if (mesh == null) {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        DevideUpMesh();
        UpdateMesh();
        //CreateNavMeshSurface();
    }



    void Start(){
        if(randomSeed){
            seed = Random.Range(1, 100000);
        }
        minTerrainHeight = 0f;
        maxTerrainHeight = 0f;


        meshCollider = GetComponent<MeshCollider>(); // Hozzáadott sor

        if (mesh == null) {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        DevideUpMesh();
        UpdateMesh();
        CreateNavMeshSurface();
        placeTree();
    }


    void CreateNavMeshSurface(){

        surface.BuildNavMesh();
    }

    void placeTree(){

        for(int x = 0; x < treeDensity; x++){

            bool placed = false;

            while (!placed)
            {
                float tryPositionX = Random.Range(lowestX, highestX);
                float tryPositionZ = Random.Range(lowestZ, highestZ);

                Vector3 raycastStart = new Vector3(tryPositionX, 100f, tryPositionZ);

                if (Physics.Raycast(raycastStart, Vector3.down, out RaycastHit hit, Mathf.Infinity))
                {
                    if (hit.point.y > 12.5f)
                    {
                        float randomRotationY = Random.Range(0f, 360f);

                        // Hozz létre egy Quaternion-t a véletlenszerű Euler-szögvektorok alapján
                        Quaternion randomRotation = Quaternion.Euler(0, randomRotationY, 0);

                        Instantiate(treePrefab, new Vector3(tryPositionX, hit.point.y, tryPositionZ), randomRotation, environmentParentObj.transform);
                        placed = true;
                    }
                }
            }
        }


    }



    void DevideUpMesh(){

        vertices = new Vector3[((width/verticesScale)+1) * ((height/verticesScale)+1)];



        System.Random prng = new System.Random(seed);
        float xOffset = prng.Next(-1000, 1000);
        float yOffset = prng.Next(-1000, 1000);

        for (int i = 0, x = 0; x <= width; x+=verticesScale)
        {
            for (int y = 0; y <= height; y+=verticesScale)
            {
                float xCoord = (float)x / width * scale + xOffset;
                float yCoord = (float)y / height * scale + yOffset;
                float z = Mathf.PerlinNoise(xCoord, yCoord) * heightMapping * 2 - 1;
                vertices[i] = new Vector3(y-150, z, x-150);

                if(z > maxTerrainHeight){
                    maxTerrainHeight = z;
                }
                if(z < minTerrainHeight){
                    minTerrainHeight = z;
                }

                i++;
            }

        }


        triangles = new int[(width/verticesScale) * (height/verticesScale) * 6];
        int vert = 0;
        int tris = 0;
        for (int y = 0; y < width; y+=verticesScale){

            for (int x = 0; x < height; x+=verticesScale)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + (height/verticesScale) + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + (width/verticesScale) + 1;
                triangles[tris + 5] = vert + (width/verticesScale) + 2;

                vert++;
                tris += 6;
                
            }
            vert++;
        }

        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= height; z += verticesScale){
            for (int x = 0; x <= width; x += verticesScale){
                
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);



                i++;
            }
        }




    }
    void UpdateMesh(){

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;
    }

    private void OnDrawGizmos(){
        if(showGizmosSphere){
            if(vertices == null){
                return;
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(vertices[i], 0.5f);
            }
        }
    }

}

internal class ValidateAttribute : System.Attribute
{
}