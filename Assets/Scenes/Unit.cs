using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Unit : MonoBehaviour {

    public TileMap tileMap;

    public int speed = 5;
    public int attack1Range = 1;
    public int attack1Buffer = 0;
    public int attack2Range;
    public int attack2Buffer;
    public int attack3Range;
    public int attack3Buffer;

    public int x = -1;
    public int y = -1;

    public void setCoords(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public void attack1(GameObject unitTarget) { 
        Assert.IsTrue(unitTarget != this);      
        tileMap.pushUnit(this.gameObject, unitTarget, 2);
    }

    // Abstract functions (need to be overwritten in children scripts)
    public abstract void attack2(Vector2Int targetPos);
    //public abstract void attack3(GameObject unitTarget);

}
