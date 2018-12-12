using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructer : Unit {

    public new int speed = 4;
    public new int attack2Range = 4;
    public new int attack2Buffer = 1;
    public new int attack3Range = 1;
    public new int attack3Buffer = 0;

    public override void attack2(Vector2Int targetPos) {
        tileMap.setTile(Tile.ROCK, targetPos.x, targetPos.y);
    }
}
