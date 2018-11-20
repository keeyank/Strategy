using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public TileMap tileMap;

    public int speed = 5;
    public int pushRange = 1;
    public int x;
    public int y;

    private void Start() {
        // Gain access to Map's TileMap script
        // "Find" finds the GO with the tag "Map" in the same scene as this instance of Unit
        GameObject mapGO = GameObject.Find("Map");
        tileMap = mapGO.GetComponent<TileMap>();

        // Get its current coords based on its position in the world
        Vector2Int gameCoord = TileMap.worldCoordToGameCoord(gameObject.transform.position);
        x = gameCoord.x;
        y = gameCoord.y;
    }

    private void OnMouseUp() {
        tileMap.resetValidTiles();
        // Update over here
        // If statement: if stte is attack state key 1, execute the function that pushes this character yo
        // Otherwise just select unit as usual, and set the valid tiles n stuff
        // Also the if statement should be before the resetValidTiles so we can use a function in TileMap to check if 
        // the tile that this unit is on is valid (AttackCharacter(this) or whatevver it probly checks selectedUnit and this unit
        // and sees if the tiel this guys on is valid and if it is, itll call a function here that pushes this guy back a bit via
        // the moveUnit function in tileMap so attackCharacter or whatever would probly be a boolean and the attacking goes on here idk)
        tileMap.selectUnit(gameObject);
        tileMap.setValidTiles(Tile.MAT_GRASS_VALID_MOVEMENT, speed); 
    }
}
