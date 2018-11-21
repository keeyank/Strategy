using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public TileMap tileMap;

    public int speed = 5;
    public int pushRange = 1;
    public int x = -1;
    public int y = -1;

    public void setCoords(int x, int y) {
        this.x = x;
        this.y = y;
    }
}
