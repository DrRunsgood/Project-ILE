using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class TerrainQuadtreeImporter : MonoBehaviour
{
    [Header("Terrain Settings")]
    [Tooltip("Width (X-axis) and Depth (Z-axis) of each Terrain tile in Unity units.")]
    public int terrainTileSize = 4096;

    [Tooltip("Maximum possible height of the terrain.")]
    public float maxTerrainHeight = 600f;

    [Tooltip("Heightmap resolution for each Terrain tile (must match the exported heightmaps).")]
    public int heightmapResolution = 1025;

    [Header("Terrain Material")]
    [Tooltip("Assign a material to apply to all imported terrains.")]
    public Material terrainMaterial;

    /// <summary>
    /// Imports heightmap PNG files and assembles them into terrain chunks.
    /// </summary>
    /// <param name="importDirectory">Directory containing the heightmap PNG files.</param>
    public void ImportHeightmaps(string importDirectory)
    {
        if (!Directory.Exists(importDirectory))
        {
            Debug.LogError($"Import directory does not exist: {importDirectory}");
            return;
        }

        // Get all heightmap files following the naming convention *_Heightmap.png
        string[] heightmapFiles = Directory.GetFiles(importDirectory, "*_Heightmap.png");

        foreach (string filePath in heightmapFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath); // e.g., Terrain_0_0_Heightmap
            string terrainName = fileName.Replace("_Heightmap", ""); // e.g., Terrain_0_0

            // Parse X and Z indices from the terrain name
            string[] parts = terrainName.Split('_');
            if (parts.Length != 3)
            {
                Debug.LogWarning($"Invalid terrain name format: {terrainName}. Skipping.");
                continue;
            }

            int tileX, tileZ;
            if (!int.TryParse(parts[1], out tileX) || !int.TryParse(parts[2], out tileZ))
            {
                Debug.LogWarning($"Invalid tile indices in terrain name: {terrainName}. Skipping.");
                continue;
            }

            // Load the heightmap texture
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D heightmapTexture = new Texture2D(heightmapResolution, heightmapResolution, TextureFormat.RFloat, false);
            heightmapTexture.LoadImage(fileData);

            // Convert texture to heightmap array
            float[,] heights = new float[heightmapResolution, heightmapResolution];
            for (int y = 0; y < heightmapTexture.height; y++)
            {
                for (int x = 0; x < heightmapTexture.width; x++)
                {
                    Color pixel = heightmapTexture.GetPixel(x, y);
                    heights[y, x] = pixel.r; // Assuming grayscale where R=G=B
                }
            }

            // Create a new Terrain GameObject
            GameObject terrainGO = new GameObject(terrainName);
            terrainGO.transform.parent = this.transform;
            terrainGO.transform.position = new Vector3(tileX * terrainTileSize, 0f, tileZ * terrainTileSize);

            // Add Terrain and TerrainCollider components
            Terrain terrain = terrainGO.AddComponent<Terrain>();
            TerrainCollider terrainCollider = terrainGO.AddComponent<TerrainCollider>();

            // Initialize TerrainData
            TerrainData terrainData = new TerrainData
            {
                heightmapResolution = heightmapResolution,
                size = new Vector3(terrainTileSize, maxTerrainHeight, terrainTileSize)
            };

            // Apply heightmap to TerrainData
            terrainData.SetHeights(0, 0, heights);
            terrain.terrainData = terrainData;
            terrainCollider.terrainData = terrainData;

            // Assign Terrain Material
            if (terrainMaterial != null)
            {
                terrain.materialTemplate = terrainMaterial;
            }
            else
            {
                Debug.LogWarning("No terrain material assigned. Using default standard terrain shader.");
                terrain.materialTemplate = new Material(Shader.Find("Nature/Terrain/Standard"));
            }

            Debug.Log($"Imported terrain chunk: {terrainName} from {filePath}");

            // Destroy the temporary texture to free memory
            DestroyImmediate(heightmapTexture);
        }
    }
}
