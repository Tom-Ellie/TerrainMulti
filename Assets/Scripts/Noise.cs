using UnityEngine;
using System.Collections;

public enum NoiseType {Value, Perlin, SecondLevelWarp, Simplex };

public static class Noise {
    private static float squaresToTriangles = (3f - Mathf.Sqrt(3f)) / 6f;
    private static float trianglesToSquares = (Mathf.Sqrt(3f) - 1f) / 2f;

    public enum NormalizeMode { Local, Global };

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCentre) {
        float[,] noiseMap = new float[mapWidth, mapHeight];


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
            amplitude *= settings.persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;
                Vector3 pos3D = new Vector3(x, y, 0.0f);
                NoiseSample sample = new NoiseSample();
                sample.value = 0;
                for (int i = 0; i < settings.octaves; i++) {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;
                    float value = 0;
                    if (settings.noiseType == NoiseType.Perlin) {

                        value = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    } else if (settings.noiseType == NoiseType.SecondLevelWarp) {

                        Vector2 p = new Vector2(sampleX, sampleY);
                        Vector2 q = new Vector2(Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1, Mathf.PerlinNoise(sampleX + 5.2f, sampleY + 1.3f) * 2 - 1);
                        Vector2 r = new Vector2(Mathf.PerlinNoise(p.x + 4.0f * q.x + 1.7f, p.y + 4.0f * q.y + 9.2f) * 2 - 1,
                                                Mathf.PerlinNoise(p.x + 4.0f * q.x + 8.3f, p.y + 4.0f * q.y + 2.8f) * 2 - 1);
                        value = (Mathf.PerlinNoise(sampleX + (4.0f * r.x), sampleY + (4.0f * r.y))) * 2 - 1;

                    } else if (settings.noiseType == NoiseType.Simplex) {
                        sample += NoiseHash.SimplexValue2D(pos3D, frequency);
                        value = sample.value;
                    }

                    noiseHeight += value * amplitude;

                    amplitude *= settings.persistance;
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
            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++) {
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

    public NoiseType noiseType = NoiseType.Perlin;

	public float scale = 50;

	public int octaves = 6;
	[Range(0,1)]
	public float persistance =.6f;
	public float lacunarity = 2;

	public int seed;
	public Vector2 offset;

	public void ValidateValues() {
		scale = Mathf.Max (scale, 0.01f);
		octaves = Mathf.Max (octaves, 1);
		lacunarity = Mathf.Max (lacunarity, 1);
		persistance = Mathf.Clamp01 (persistance);
	}
}
