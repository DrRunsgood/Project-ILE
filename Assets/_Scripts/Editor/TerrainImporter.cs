using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(TerrainQuadtreeImporter))]
public class TerrainImporter : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TerrainQuadtreeImporter importer = (TerrainQuadtreeImporter)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Import Heightmaps"))
        {
            string importPath = EditorUtility.OpenFolderPanel("Select Import Folder", "", "");
            if (!string.IsNullOrEmpty(importPath))
            {
                importer.ImportHeightmaps(importPath);
                EditorUtility.DisplayDialog("Import Complete", "Heightmaps have been imported successfully.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Import Cancelled", "No folder selected. Import cancelled.", "OK");
            }
        }
    }
}
