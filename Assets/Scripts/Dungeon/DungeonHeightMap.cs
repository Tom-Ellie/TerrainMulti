using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DungeonHeightMap {

    public static float[,] GenerateFlatMap(int tileWidth) {
        float[,] tileHeights = new float[tileWidth, tileWidth];
        for (int i = 0; i < tileWidth; i++) {
            for (int j = 0; j < tileWidth; j++) {
                tileHeights[i, j] = 1.0f;
            }
        }
        return tileHeights;
    }
}
