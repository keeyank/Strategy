using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructer : Unit {

    public override void attack2(Vector2Int targetPos) {
        tileMap.setTile(Tile.ROCK, targetPos.x, targetPos.y);
    }
}
