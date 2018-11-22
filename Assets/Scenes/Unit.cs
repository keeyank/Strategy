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

    public void attack1(GameObject unitTarget) {
        Assert.IsTrue(unitTarget != this);
        Debug.Log("Unit " + gameObject.name + "is attacking " + unitTarget.name + ".");
        tileMap.pushUnit(this.gameObject, unitTarget, 1);
    }
}
