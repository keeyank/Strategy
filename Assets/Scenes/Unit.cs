using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Unit : MonoBehaviour {

    public TileMap tileMap;

    public int speed = 5;
    public int attackRange = 1;
    public int x = -1;
    public int y = -1;

    public void setCoords(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public virtual void attack1(GameObject unitTarget) { // virtual means it searches children of Unit class to override 
        Assert.IsTrue(unitTarget != this);
        Debug.Log("Unit " + gameObject.name + "is attacking " + unitTarget.name + ".");
        tileMap.pushUnit(this.gameObject, unitTarget, 2);
    }

    public virtual void attack2(GameObject unitTarget) { }
    public virtual void attack3(GameObject unitTarget) { }

}
