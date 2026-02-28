using System.Collections.Generic;
using System.Threading;
using System;
using Unity.Mathematics;
using UnityEditor.AssetImporters;
using UnityEngine;
using Unity.AI.Navigation;

public class WorldGen_Terrain : MonoBehaviour
{

    public enum DrawMode { NoiseMap, ColourMap, Mesh };
    public DrawMode drawMode;

    public Material mats;
    [SerializeField] GameObject objYes;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;

    public TerrainType[] regions;

    public void GenerateMap()
    {
        float[,] noiseMap = NoiseGen.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colourMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapWidth + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        Mapdisp display = UnityEngine.Object.FindAnyObjectByType<Mapdisp>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextrGen.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextrGen.TextureFromColourMap(colourMap, mapWidth, mapHeight));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MesGen.GenerateTerrainMesh(noiseMap), TextrGen.TextureFromColourMap(colourMap, mapWidth, mapHeight));
        }
    }

    void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }

    private void Start()
    {
        GenerateMap();
        GetComponent<Mapdisp>().meshRenderer.material = mats;
        objYes.GetComponent<MeshCollider>().sharedMesh = null;
        objYes.GetComponent<MeshCollider>().sharedMesh = GetComponent<Mapdisp>().meshFilter.mesh;
        objYes.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

