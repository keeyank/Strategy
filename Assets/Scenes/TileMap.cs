using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TileMap : MonoBehaviour {

    public class Cell {
        public GameObject tile; // The tile at this cell
        public GameObject unit; // The unit at this cell (if there is one)

        public Cell(GameObject tile, GameObject unit) {
            this.tile = tile;
            this.unit = unit;
        }
    }

    private const int MAP_WIDTH = 12;
    private const int MAP_HEIGHT = 12;
    private const int SCALE_FACTOR = 1;

    private const int STATE_SELECT = 0;
    private const int STATE_COMMAND = 1;
    private const int STATE_ATTACK = 2;
    private int currentState;

    // To be associated with objects in the inspector
    public GameObject tilePrefab;
    public GameObject[] unitPrefabs;

    private GameObject selectedUnit;
    private GameObject[,] map = new GameObject[MAP_WIDTH, MAP_HEIGHT];

    public static Vector2Int worldCoordToGameCoord(Vector3 position) {
        float x = position.x;
        float y = position.y;
        return new Vector2Int((int)x / SCALE_FACTOR, (int)y / SCALE_FACTOR);
    }

    public static Vector3 gameCoordToWorldCoord(int x, int y, int z) {
        return new Vector3(x * SCALE_FACTOR, y * SCALE_FACTOR, z);
    }


    void Start() {
        generateMap();
        currentState = STATE_SELECT;

        // DEBUG
        setTile(Tile.ROCK, 4, 4);
        setTile(Tile.ROCK, 4, 5);
        setTile(Tile.ROCK, 5, 4);
        setTile(Tile.ROCK, 5, 5);
        setTile(Tile.ROCK, 6, 4);
        setTile(Tile.ROCK, 8, 10);
        setTile(Tile.ROCK, 0, 6);
    }


    // Generate the map as all grass tiles
    private void generateMap() {
        Debug.Log("Generating map.");
        for (int x = 0; x < MAP_WIDTH; x++) {
            for (int y = 0; y < MAP_HEIGHT; y++) {
                setTile(Tile.GRASS, x, y);
            }
        }
        return;
    }

    // Instantiate each individual tile at point x, y
    private void setTile(int type, int x, int y) {

        // Delete previous tile if it exists
        if (map[x, y] != null) {
            Destroy(map[x, y]);
            map[x, y] = null;
            Assert.AreEqual(map[x, y], null);
        }

        // Instantiate new tile instance
        GameObject tileGO = Instantiate(tilePrefab, gameCoordToWorldCoord(x, y, 0), Quaternion.identity);

        // Give access to coords and type for new tile instance
        // Map is set to tileGO (tile type and coords can be accessed via map at any time)
        Tile tile = tileGO.GetComponent<Tile>();
        tile.setTile(type, x, y, this);
        map[x, y] = tileGO;
        return;
    }

    /* -------------- Public Functions -------------- */

    // Set selectedUnit and change state to command state
    public void selectUnit(GameObject unit) {
        Debug.Log("Selecting Unit " + unit.name);
        selectedUnit = unit;
        currentState = STATE_COMMAND;
    }

    // Remove selectedUnit and change the state
    public void deselectUnit() {
        if (selectedUnit != null) {
            Debug.Log("Deselecting Unit " + selectedUnit.name);
            selectedUnit = null;
            currentState = STATE_SELECT;
        }
    }

    // Set all tiles within distance, specifying valid material
    public void setValidTiles(int validType, int distance) {
        Debug.Assert(currentState == STATE_COMMAND || currentState == STATE_ATTACK);
        Unit unit = selectedUnit.GetComponent<Unit>();

        int minX = Mathf.Clamp(unit.x - distance, 0, MAP_WIDTH - 1);
        int maxX = Mathf.Clamp(unit.x + distance, 0, MAP_WIDTH - 1);
        int minY = Mathf.Clamp(unit.y - distance, 0, MAP_HEIGHT - 1);
        int maxY = Mathf.Clamp(unit.y + distance, 0, MAP_HEIGHT - 1);

        for (int x = minX; x <= maxX; x++) {
            for (int y = minY; y <= maxY; y++) {
                if (Mathf.Abs(unit.x-x) + Mathf.Abs(unit.y-y) <= distance) {
                    Tile tile = map[x, y].GetComponent<Tile>();
                    tile.setAsValid(validType);
                }
            }
        }
    }

    // Resets all walkable tiles as valid
    public void resetValidTiles() {
        for (int x = 0; x < MAP_WIDTH; x++) {
            for (int y = 0; y < MAP_HEIGHT; y++) {
                Tile tile = map[x, y].GetComponent<Tile>();
                tile.setAsInvalid();
            }
        }
    }

    // Move unit to (x, y)
    private void moveUnit(GameObject unitGO, int x, int y) {
        // Move in world
        unitGO.transform.position = gameCoordToWorldCoord(x, y, -1);
        // Move in game
        Unit unit = unitGO.GetComponent<Unit>();
        unit.x = x;
        unit.y = y;
        return;
    }

    // Move selected unit to coordinates if it exists
    public void moveSelectedUnit(int x, int y) {
        if (selectedUnit != null && currentState == STATE_COMMAND) {
            Assert.AreEqual(currentState, STATE_COMMAND);

            Tile selectedTile = map[x, y].GetComponent<Tile>();
            if (selectedTile.isValid) {
                Debug.Log("Moving selected unit " + selectedUnit.name
                    + " to coordinates (" + x + "," + y + ").");
                moveUnit(selectedUnit, x, y);
            } 
            else {
                Debug.Log("Coordinates (" + x + "," + y + ") are invalid!");
            }
        }
        else if (currentState == STATE_ATTACK) {
            Debug.Log("Cannot move while in attack state!");
        }
        else {
            Debug.Log("No unit selected!");
        }
        return;
    }

    // Update is called once per frame
    void Update () {
        
        // Change to attack state when 1 is clicked
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            if (selectedUnit == null) {
                Debug.Log("No unit selected!");
            }
            else {
                resetValidTiles();
                currentState = STATE_ATTACK;
                int distance = selectedUnit.GetComponent<Unit>().pushRange;
                setValidTiles(Tile.MAT_GRASS_VALID_ATTACK, distance);
                Debug.Log("Unit " + selectedUnit.name + "is preparing to attack!");
            }
        }
    }
}
