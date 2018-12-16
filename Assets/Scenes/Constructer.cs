using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructer : Unit {

    Constructer() : base(7, 4, 1, 0, 5, 1, 5, 4) { }

    public override void attack2(Vector2Int targetPos, GameObject unitTarget) {
        tileMap.spawnCluster(targetPos, 1, Tile.ROCK);
    }

    public override void attack3(Vector2Int targetPos, GameObject unitTarget) {
        if (unitTarget != null) {
            tileMap.damageUnit(unitTarget, 3);
        }
        tileMap.spawnCluster(targetPos, 2, Tile.ROCK);
        tileMap.spawnCluster(targetPos, 1, Tile.GRASS);
    }
}