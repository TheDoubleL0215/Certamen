using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedualTerrain : MonoBehaviour
{
    public int width = 300;
    public int height = 300;
    public TerrainType[] regions;
    public float scale = 20;

    private float xOffset;
    private float yOffset;

    void Start(){
        xOffset = Random.Range(0, 999999);
        yOffset = Random.Range(0, 999999);
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateTexture();
    }

    void Update(){

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

}
