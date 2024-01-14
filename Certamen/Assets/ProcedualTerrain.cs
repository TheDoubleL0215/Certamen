using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]
public class ProcedualTerrain : MonoBehaviour
{
    public int width = 300;
    public int height = 300;
    public TerrainType[] regions;
    public float scale = 20;

    Mesh mesh;
    int[] triangles;
    int[] testTriangles;
    private float xOffset;
    private float yOffset;
    Vector3[] vertices;
    Vector3[] testVertices;

    [Header("Gizmos")]
    [SerializeField] private bool showGizmosSphere = true;
    public int verticesScale = 20;

    void Start(){

        xOffset = Random.Range(0, 999999);
        yOffset = Random.Range(0, 999999);
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateTexture();
    }

    Texture2D GenerateTexture(){
        Texture2D texture = new Texture2D(width, height);

        //PNM (Perlin Noise Map) generálás
        for (int x = 0; x < width; x++){
            for (int y = 0; y < height; y++){

            float xCoord = (float)x / width * scale + xOffset;
            float yCoord = (float)y / height * scale + yOffset;

            //A Mathf.PerlinNoise(x, y) egy random számot generál, mely aztán egy színné lesz átkonvertálva
            float sample = Mathf.PerlinNoise(xCoord, yCoord);
                for (int s = 0; s < regions.Length; s++){
                    if(sample <= regions[s].height){
                        Color chosenColor = regions[s].color;
                        texture.SetPixel(x, y, chosenColor);
                    }
                }


            }
        }

        texture.Apply();
        return texture;

    }





    //TerrainTypes az Inspectorban
    [System.Serializable]
    public struct TerrainType{
        public string name;
        public float height;
        public Color color;
    }

    private void OnDrawGizmos(){
        if(showGizmosSphere){
            if(vertices == null){
                return;
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(vertices[i], 1f);
            }
        }
    }
    }

