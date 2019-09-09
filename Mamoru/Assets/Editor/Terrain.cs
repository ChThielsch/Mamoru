using UnityEngine;
using UnityEditor;
using System.IO;

public class TerrainTools
{
    [MenuItem("Tools/Terrain/Set Terrain Material %u")]
    private static void SetTerrainMaterial()
    {
        GameObject activeObject = Selection.activeGameObject;
        if (activeObject == null)
        {
            Debug.LogError("activeObject is null");
            return;
        }

        Terrain activeTerrain = activeObject.GetComponent<Terrain>();
        if (activeTerrain == null)
        {
            Debug.LogError("activeTerrain is null");
            return;
        }
        activeTerrain.materialType = Terrain.MaterialType.Custom;
        Material terrainMaterial = Resources.Load<Material>("TerrainMaterial");
        if (terrainMaterial == null)
        {
            Debug.LogError("terrainMaterial is null");
            return;
        }

        Material newMaterial = new Material(terrainMaterial);
        int materialIndex;
        if (!File.Exists(Path.Combine(Application.streamingAssetsPath, "MaterialCounter.txt")))
        {
            Debug.LogError("MaterialCounter.txt doesnt exist");
            return;
        }
        using (StreamReader reader = new StreamReader(Path.Combine(Application.streamingAssetsPath, "MaterialCounter.txt")))
        {
           materialIndex = int.Parse(reader.ReadLine());
        }
            AssetDatabase.CreateAsset(newMaterial, "Assets/Resources/Material"+materialIndex+".mat");
        activeTerrain.materialTemplate = Resources.Load<Material>("Material" + materialIndex);
        activeTerrain.materialTemplate.SetTexture("_LayerMaskMap", activeTerrain.terrainData.alphamapTextures[0]);

        materialIndex++;
        using (StreamWriter writer = new StreamWriter(Path.Combine(Application.streamingAssetsPath, "MaterialCounter.txt"),false))
        {
            writer.WriteLine(materialIndex);
        }
    }

    [MenuItem("Tools/Terrain/Enable Splatmaps")]
    private static void EnableSplatmaps()
    {

        Object activeObject = Selection.activeObject;
        if (!(activeObject is TerrainData))
        {
            Debug.LogError("Selected Object isn't an TerrainData Asset");
            return;
        }

        Object[] data = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(activeObject));
        foreach (Object o in data)
        {
            if (o is Texture2D)
            {
                (o as Texture2D).hideFlags = HideFlags.None;
                Debug.Log(o);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(o));
            }
        }
    }


    [MenuItem("Tools/Terrain/Disable Basemap")]
    private static void DisableBasemap()
    {

        Object activeObject = Selection.activeObject;
        Terrain terrain = (activeObject as GameObject)?.GetComponent<Terrain>();
        if (!terrain)
        {
            Debug.LogError("Selected Object doesn't have an Terrain");
            return;
        }

        terrain.basemapDistance = 10000000;
        Debug.Log(terrain.name + "  " + terrain.basemapDistance);
    }
}