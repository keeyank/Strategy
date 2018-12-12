using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public const int GRASS = 0;
    public const int ROCK = 1;

    public TileMap tileMap;

    public bool isValid = false;

    public int type; 
    public int x;
    public int y;

    public const int MAT_GRASS_INVALID = 0;
    public const int MAT_GRASS_VALID_MOVEMENT = 1;
    public const int MAT_ROCK = 2;
    public const int MAT_GRASS_VALID_ATTACK = 3;

    [SerializeField] // So we can add in the mats in the inspector
    public Material[] mats;

    public void setTile(int type, int x, int y, TileMap tileMap) {
        this.type = type;
        this.x = x;
        this.y = y;
        this.tileMap = tileMap;

        if (type == GRASS) {
            gameObject.GetComponent<Renderer>().material = mats[MAT_GRASS_INVALID];
        }
        else if (type == ROCK) {
            gameObject.GetComponent<Renderer>().material = mats[MAT_ROCK];
        }
    }

    public void setAsValid(int validType) {
        if (type != GRASS) {
            return; // only grass can be a valid tile
        }
        isValid = true;
        gameObject.GetComponent<Renderer>().material = mats[validType];
    }

    public void setAsInvalid() {
        if (type != GRASS) {
            return;
        }
        isValid = false;
        gameObject.GetComponent<Renderer>().material = mats[MAT_GRASS_INVALID];
    }

    void OnMouseUp() {
        tileMap.processTilePress(x, y);
    }
}
