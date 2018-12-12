using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructer : Unit {

    public override int Speed {
        get { return 4; }
    }
    public override int Attack1Range {
        get { return 1; }
    }
    public override int Attack1Buffer {
        get { return 0; }
    }
    public override int Attack2Range {
        get { return 5; }
    }
    public override int Attack2Buffer {
        get { return 1; }
    }
    public override int Attack3Range {
        get { return 1; }
    }
    public override int Attack3Buffer {
        get { return 1; }
    }

    public override void attack2(Vector2Int targetPos) {
        tileMap.setTile(Tile.ROCK, targetPos.x, targetPos.y);
    }
}
