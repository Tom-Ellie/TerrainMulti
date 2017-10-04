using UnityEngine;
using System.Collections;

public class MapPreview : MonoBehaviour {

	public Renderer textureRender;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;


	public enum DrawMode {NoiseMap, Mesh, FalloffMap, Coast};
	public DrawMode drawMode;

    public RegionHolder region;

	[Range(0,MeshSettings.numSupportedLODs-1)]
	public int editorPreviewLOD;

	public bool autoUpdate;

    public static int wow = 0;

	public void DrawMapInEditor() {
        region.textureData.ApplyToMaterial (region.terrainMaterial);
        region.textureData.UpdateMeshHeights (region.terrainMaterial, region.heightMapSettings.minHeight, region.heightMapSettings.maxHeight);
        HeightMap heightMap;

        heightMap = HeightMapGenerator.GenerateHeightMap((int)(region.meshSettings.numVertsPerLine), region.heightMapSettings, Vector2.zero);

        if (drawMode == DrawMode.NoiseMap) {
                DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
		} else if (drawMode == DrawMode.Mesh) {
			DrawMesh (MeshGenerator.GenerateTerrainMesh (heightMap.values, region.meshSettings, editorPreviewLOD));
		} else if (drawMode == DrawMode.FalloffMap) {
			DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(region.meshSettings.numVertsPerLine),0,1)));
		} else if (drawMode == DrawMode.Coast) {
//            Agent agent = new Agent();
//            agent.seedPos = new IntVec((region.meshSettings.numVertsPerLine) / 2, (region.meshSettings.numVertsPerLine) / 2);
//            agent.preferredDir = Direction.North;
//            agent.tokens = 1000;
//            CoastMaker.InitLandmass(region.meshSettings.numVertsPerLine);
//            CoastMaker.CoastLineGenerate(agent);
 //           HeightMap hm = new HeightMap(CoastMaker.landmass, 0, 1);
//            DrawTexture(TextureGenerator.TextureFromHeightMap(hm));
        }
	}





	public void DrawTexture(Texture2D texture) {
		textureRender.sharedMaterial.mainTexture = texture;
		textureRender.transform.localScale = new Vector3 (texture.width, 1, texture.height) /10f;

		textureRender.gameObject.SetActive (true);
		meshFilter.gameObject.SetActive (false);
	}

	public void DrawMesh(MeshData meshData) {
		meshFilter.sharedMesh = meshData.CreateMesh ();

		textureRender.gameObject.SetActive (false);
		meshFilter.gameObject.SetActive (true);
	}



	void OnValuesUpdated() {
		if (!Application.isPlaying) {
			DrawMapInEditor ();
		}
	}

	void OnTextureValuesUpdated() {
        region.textureData.ApplyToMaterial (region.terrainMaterial);
	}

    //OnValuesUpdated is a base function which
	void OnValidate() {
        
		if (region.meshSettings != null) {
            region.meshSettings.OnValuesUpdated -= OnValuesUpdated; //Remove last changes
            region.meshSettings.OnValuesUpdated += OnValuesUpdated;
		} else {
            Debug.LogError("No Mesh Settings specified in previewer");
            return;
        }
        if (region.heightMapSettings != null) {
            region.heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            region.heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        } else {
            Debug.LogError("No Height Map Settings specified in previewer");
            return;
        }
        if (region.textureData != null) {
            region.textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            region.textureData.OnValuesUpdated += OnTextureValuesUpdated;
		} else {
            Debug.LogError("No Texture Data specified in previewer");
            return;
        }

    }


}

