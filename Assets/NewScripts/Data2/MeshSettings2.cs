using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class MeshSettings2 : UpdatableData {

    public const int numSupportedChunkSizes = 9;
    public static readonly int[] supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

    public float maxViewDistance = 500f;

    public int width = 257;
    public int depth = 40;

    public float meshScale = 2.5f;
    public bool useFlatShading;

    [Range(0, numSupportedChunkSizes - 1)]
    public int chunkSizeIndex;


    public float meshSize {
        get {
            return (width)* meshScale;
         //   return supportedChunkSizes[chunkSizeIndex] * meshScale;
        }
    }


}
