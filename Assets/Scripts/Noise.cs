using UnityEngine;
using System.Collections;

public enum NoiseType {Value, Perlin, SecondLevelWarp, Simplex };

public static class Noise {
 //   private static float squaresToTriangles = (3f - Mathf.Sqrt(3f)) / 6f;
//    private static float trianglesToSquares = (Mathf.Sqrt(3f) - 1f) / 2f;

    public enum NormalizeMode { Local, Global };

    public static float[,] GenerateNoiseMap(int mapSize, NoiseSettings settings, Vector2 sampleCentre) {
        float[,] noiseMap = new float[mapSize, mapSize];


        System.Random prng = new System.Random(settings.seed);
        Vector2[] octaveOffsets = new Vector2[settings.octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < settings.octaves; i++) {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCentre.x;
            float offsetY = prng.Next(-100000, 100000) - settings.offset.y - sampleCentre.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.persistence;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapSize / 2f;
        float halfHeight = mapSize / 2f;

        for (int y = 0; y < mapSize; y++) {
            for (int x = 0; x < mapSize; x++) {

                amplitude = 1;
                frequency = 1; //Possibly allow for different freq starts
                float noiseHeight = 0;

                NoiseMethod method = NoiseHash.methods[(int)settings.type][settings.dimensions - 1];

                for (int i = 0; i < settings.octaves; i++) {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;
                    float value = 0;
                    if (settings.type == NoiseMethodType.Perlin) {

                        value = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    } else if (settings.type == NoiseMethodType.SecondLevelWarp) {

                        Vector2 p = new Vector2(sampleX, sampleY);
                        Vector2 q = new Vector2(Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1, Mathf.PerlinNoise(sampleX + 5.2f, sampleY + 1.3f) * 2 - 1);
                        Vector2 r = new Vector2(Mathf.PerlinNoise(p.x + 4.0f * q.x + 1.7f, p.y + 4.0f * q.y + 9.2f) * 2 - 1,
                                                Mathf.PerlinNoise(p.x + 4.0f * q.x + 8.3f, p.y + 4.0f * q.y + 2.8f) * 2 - 1);
                        value = (Mathf.PerlinNoise(sampleX + (4.0f * r.x), sampleY + (4.0f * r.y))) * 2 - 1;

                    } else if (settings.type == NoiseMethodType.Simplex) {
                        //does sampleX/sampleY work?
                        sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale;
                        sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale;
                        NoiseSample sample = NoiseHash.MethodNoOctaves(method, new Vector3(sampleX, sampleY), frequency, amplitude, settings.persistence);
                       // sample = sample * 0.5f;
                        value = sample.value;
                    }

                    noiseHeight += value * amplitude;

                    amplitude *= settings.persistence;
                    frequency *= settings.lacunarity;
                }




                if (noiseHeight > maxLocalNoiseHeight) {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight) {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
                if (settings.normalizeMode == NormalizeMode.Global) {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        if (settings.normalizeMode == NormalizeMode.Local) {
            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
            }
        }
        return noiseMap;
    }




}

[System.Serializable]
public class NoiseSettings {
	public Noise.NormalizeMode normalizeMode;

    [Range(1, 3)]
    public int dimensions = 3;

    public NoiseMethodType type;

	public float scale = 50;

	public int octaves = 6;
	[Range(0.01f,1)]
	public float persistence =.6f;
	public float lacunarity = 2;

	public int seed;
	public Vector2 offset;

	public void ValidateValues() {
		scale = Mathf.Max (scale, 0.01f);
		octaves = Mathf.Max (octaves, 1);
		lacunarity = Mathf.Max (lacunarity, 1);
		persistence = Mathf.Clamp01 (persistence);
	}
}
