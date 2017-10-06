using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class MeshSettings : UpdatableData {

	public const int numSupportedLODs = 5;
	public const int numSupportedChunkSizes = 9;
	public const int numSupportedFlatshadedChunkSizes = 3;
	public static readonly int[] supportedChunkSizes = {48,72,96,120,144,168,192,216,240};

    public float maxViewDistance = 500f;
	
	public float meshScale = 2.5f;
	public bool useFlatShading;

	[Range(0,numSupportedChunkSizes-1)]
	public int chunkSizeIndex;
	[Range(0,numSupportedFlatshadedChunkSizes-1)]
	public int flatshadedChunkSizeIndex;


	// num verts per line of mesh rendered at LOD = 0. + 5 as supportedChunk contains 'edges', so +1. +4 as 2 extra vertices both sides as border
	public int numVertsPerLine {
		get {
			return supportedChunkSizes [(useFlatShading) ? flatshadedChunkSizeIndex : chunkSizeIndex] + 5;
		}
	}

	public float meshWorldSize {
		get {
			return (numVertsPerLine - 3) * meshScale;
		}
	}


}
