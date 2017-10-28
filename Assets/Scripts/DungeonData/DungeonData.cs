using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class DungeonData : UpdatableData {

    public int widthOfTile;
    public int numTilesPerRow; //Number of tiles per row

    public int meshTileSize {
        get {
            return widthOfTile;
        }
    }

}