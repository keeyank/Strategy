using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructer : Unit {

    Constructer() : base(7, 7, 4, 1, 0, 5, 1, 1, 0) { }

    public override void attack2(Vector2Int targetPos, GameObject unitTarget) {
        tileMap.spawnCluster(targetPos, 1);
        tileMap.setTile(Tile.ROCK, targetPos.x, targetPos.y);
    }
}