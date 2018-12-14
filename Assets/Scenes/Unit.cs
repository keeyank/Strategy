using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Unit : MonoBehaviour {

    public TileMap tileMap;

    abstract public int Speed { get; }
    abstract public int Attack1Range { get; }
    abstract public int Attack1Buffer { get; }
    abstract public int Attack2Range { get; }
    abstract public int Attack2Buffer { get; }
    abstract public int Attack3Range { get; }
    abstract public int Attack3Buffer { get; }

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
    public abstract void attack2(Vector2Int targetPos, GameObject unitTarget);
    //public abstract void attack3(GameObject unitTarget);

}
