using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeGenerator : MonoBehaviour
{
    public Terrain terrain;

    // Terrain Controls
    [Header("Terrain Mesh Controls")]
    public int width = 100;
    public int height = 100;
    public int depth = 20;

    [Header("Noise Controls")]
    [Min(0.0001f)]
    public int scale = 20;
    public int octaves = 3;
    public float amplitudePersistance = 0.5f;
    public float frequencyPersistance = 1f;
    public float offsetX;
    public float offsetY;
    public bool scroll;
    public float scrollSpeed;


    private float[,] heightMap;

    void Update()
    {
        GenerateLandscape();

        // Scroll the landscape
        if (scroll) {
            offsetX += Time.deltaTime * scrollSpeed;
            offsetY += Time.deltaTime * scrollSpeed;
        }
    }

    public void GenerateLandscape() {
        terrain.terrainData.heightmapResolution = width + 1;
        terrain.terrainData.size = new Vector3(width, depth, height);

        heightMap = GenerateHeightMap();
        terrain.terrainData.SetHeights(0, 0, heightMap);
        //SetTexture(30);
    }

    private float[,] GenerateHeightMap() {
        float[,] map = new float[width, height];

        float minNoiseVal = float.MaxValue;
        float maxNoiseVal = float.MinValue;

        for (int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                float frequency = 1f;
                float amplitude = 1f;
                float heightVal = 0f; // Use to compound octaves together

                // Add octaves to heightMap
                for (int i = 0; i < octaves; i++) {
                    // Frequency is added to the x and y values
                    float xCoord = (float)x / scale * frequency + offsetX;
                    float yCoord = (float)y / scale * frequency + offsetY;
                    float perlinValue = Mathf.PerlinNoise(xCoord, yCoord);
                    // Amplitude is added directly to the perlin value
                    heightVal += perlinValue * amplitude;

                    // Each subsequent octave increases affect of the frequency and decreases the amplitude
                    amplitude *= amplitudePersistance; // persistance values should be between 0 and 1
                    frequency *= frequencyPersistance; // persistance values should be above 1

                    if (heightVal < minNoiseVal)
                        minNoiseVal = heightVal;
                    if (heightVal > maxNoiseVal)
                        maxNoiseVal = heightVal;
                }
                map[x, y] = heightVal; // Set heightmap to octaves all added together
            }
        }

        // Unity requires terrain heightMap values to be normalized between 0 and 1.
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                // InverseLerp returns a value between 0 and 1 using a min and max value.
                map[x, y] = Mathf.InverseLerp(minNoiseVal, maxNoiseVal, map[x, y]);
            }
        }

        return map;
    }
    
    // Messing around trying to get textures to work
    private void SetTexture(float percent) {
        float[,,] alphaData = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);

        float percentage = percent / 100f;

        for(int x = 0; x < terrain.terrainData.alphamapWidth; x++) {
            for(int y = 0; y < terrain.terrainData.alphamapHeight; y++) {
                alphaData[x, y, 0] = 1-percentage;
                alphaData[x, y, 1] = percentage;
            }
        }

        terrain.terrainData.SetAlphamaps(0, 0, alphaData);
    }
}
