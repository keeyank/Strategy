using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : Unit {

    Ranger() : base(5, 4, 1, 0, 8, 1, 2, 1) { }

    public override void attack2(Vector2Int targetPos, GameObject unitTarget) {
        tileMap.damageUnit(unitTarget, 2);
    }

    public override void attack3(Vector2Int targetPos, GameObject unitTarget) {
        tileMap.pushUnit(gameObject, unitTarget, 2);
        tileMap.damageUnit(unitTarget, 2);
    }
}
