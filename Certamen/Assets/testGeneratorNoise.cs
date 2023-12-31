using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(MeshFilter))]
public class testGeneratorNoise : MonoBehaviour
{
    Mesh mesh;

    public int width = 300;
    public int height = 300;
    
    public float scale = 20;

    private float xOffset;
    private float yOffset;
    public TerrainType[] regions;

    [Header("Gizmos")]
    [SerializeField] private bool showGizmosSphere = true;
    public int verticesScale = 20;

    Vector3[] vertices;
    int[] triangles;
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        DevideUpMesh();
        UpdateMesh();
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

    [System.Serializable]
    public struct TerrainType{
        public string name;
        public float height;
        public Color color;
    }



    void DevideUpMesh(){

        vertices = new Vector3[((width/verticesScale)+1) * ((height/verticesScale)+1)];

        int i = 0;
        for (int x = 0; x <= width; x+=verticesScale)
        {
            for (int y = 0; y <= height; y+=verticesScale)
            {
                vertices[i] = new Vector3(y, 0, x);
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




    }
    void UpdateMesh(){

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
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
