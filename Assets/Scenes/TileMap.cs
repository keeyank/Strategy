﻿using System;
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
    private const int STATE_ATTACK_1 = 2;
    private int currentState;

    private const int UNIT_DEFAULT = 0;
    private const int UNIT_CONSTRUCTER = 1;

    // To be associated with objects in the inspector
    public GameObject tilePrefab;
    public GameObject[] unitPrefabs;

    private GameObject selectedUnit;
    private Cell[,] map = new Cell[MAP_WIDTH, MAP_HEIGHT];

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

        // Spawn Units
        spawnUnit(UNIT_CONSTRUCTER, 0, 0);
        spawnUnit(UNIT_DEFAULT, 4, 3);
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
        GameObject unitOnTile = null;
        if (map[x, y] != null) {
            Destroy(map[x, y].tile);
            map[x, y].tile = null;
            unitOnTile = map[x, y].unit;
        }

        // Instantiate new tile instance
        GameObject tileGO = Instantiate(tilePrefab, gameCoordToWorldCoord(x, y, 0), Quaternion.identity);

        // Give access to coords and type for new tile instance
        // Map is set to tileGO (tile type and coords can be accessed via map at any time)
        Tile tile = tileGO.GetComponent<Tile>();
        tile.setTile(type, x, y, this);
        map[x, y] = new Cell(tileGO, unitOnTile);
        return;
    }

    private void spawnUnit(int type, int x, int y) {

        Tile tile = map[x, y].tile.GetComponent<Tile>();
        if (tile.type  == Tile.ROCK) {
            Debug.Log("Invalid Spawn Point: Attempting to spawn on rock.");
            return;
        }
        if (map[x, y].unit != null) {
            Debug.Log("Invalid Spawn Point: Another unit is on this point.");
            return;
        }

        // Instantiate new unit in world coordinates, and set up it's logical position
        GameObject unitGO = Instantiate(unitPrefabs[type], gameCoordToWorldCoord(x, y, -1), Quaternion.identity);
        Unit unit = unitGO.GetComponent<Unit>();
        unit.setCoords(x, y);
        unit.tileMap = this;
        map[x, y].unit = unitGO;
        return;
    }

    // Move unit to (x, y)
    private void moveUnit(GameObject unitGO, int x, int y) {
        Unit unit = unitGO.GetComponent<Unit>();

        // Move in world
        unitGO.transform.position = gameCoordToWorldCoord(x, y, -1);
        // Delete unit's previous logical coordinate
        map[unit.x, unit.y].unit = null;
        // Move in game logic
        unit.x = x;
        unit.y = y;
        map[x, y].unit = unitGO;
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
        Debug.Assert(currentState == STATE_COMMAND || currentState == STATE_ATTACK_1);
        Unit unit = selectedUnit.GetComponent<Unit>();

        int minX = Mathf.Clamp(unit.x - distance, 0, MAP_WIDTH - 1);
        int maxX = Mathf.Clamp(unit.x + distance, 0, MAP_WIDTH - 1);
        int minY = Mathf.Clamp(unit.y - distance, 0, MAP_HEIGHT - 1);
        int maxY = Mathf.Clamp(unit.y + distance, 0, MAP_HEIGHT - 1);

        for (int x = minX; x <= maxX; x++) {
            for (int y = minY; y <= maxY; y++) {
                if (Mathf.Abs(unit.x-x) + Mathf.Abs(unit.y-y) <= distance) {
                    Tile tile = (map[x, y].tile).GetComponent<Tile>();
                    tile.setAsValid(validType);
                }
            }
        }
    }

    // Resets all walkable tiles as valid
    public void resetValidTiles() {
        for (int x = 0; x < MAP_WIDTH; x++) {
            for (int y = 0; y < MAP_HEIGHT; y++) {
                Tile tile = (map[x, y].tile).GetComponent<Tile>();
                tile.setAsInvalid();
            }
        }
    }

    // PUBLIC ATACK FUNCTIONS //
    // Push the target unit pushDst cells away from the source unit
    public void pushUnit(GameObject unitSource, GameObject unitTarget, int pushDst) {
        Vector2Int srcCoord = new Vector2Int(unitSource.GetComponent<Unit>().x, unitSource.GetComponent<Unit>().y);
        Vector2Int trgCoord = new Vector2Int(unitTarget.GetComponent<Unit>().x, unitTarget.GetComponent<Unit>().y);
        // Assertion: Either the x of unitSource is the same as the x unitTarget,
        // or the y of unitSource is the same as the y of unitTarget when pushing (exclusive or)
        Assert.IsTrue(srcCoord.x == trgCoord.x ^ srcCoord.y == trgCoord.y); // ^ means exclusive or

        // Push the target unit in opposite direction of the source unit
        // Note: We can format the if statements this way due to the exclusive or assertions
        if (srcCoord.x == trgCoord.x) {
            if (srcCoord.y < trgCoord.y) {
                // Attempt to find any rocks via a scan, if any exist, then shorten the push distance
                int maxNewPos_y = Mathf.Clamp(trgCoord.y + pushDst, 0, MAP_HEIGHT - 1);
                int newPos_y;
                for (newPos_y = trgCoord.y; newPos_y < maxNewPos_y; newPos_y++) {
                    Tile newPosTile = map[trgCoord.x, newPos_y + 1].tile.GetComponent<Tile>();
                    if (newPosTile.type == Tile.ROCK) {
                        break; // Rock found, newPos is whatever was previously determined
                        // TODO: Damage unitTarget here
                    }
                }

                moveUnit(unitTarget, trgCoord.x, newPos_y);
            }
            else { // srcCoord.y > trgCoord.y
                // Attempt to find any rocks via a scan, if any exist then shorten the pushDst
                int minNewPos_y = Mathf.Clamp(trgCoord.y - pushDst, 0, MAP_HEIGHT - 1);
                int newPos_y;
                for (newPos_y = trgCoord.y; newPos_y > minNewPos_y; newPos_y--) {
                    Tile newPosTile = map[trgCoord.x, newPos_y - 1].tile.GetComponent<Tile>();
                    if (newPosTile.type == Tile.ROCK) {
                        break; // Rock found, newPos is whatever was previously determined
                        // TODO: Damage unitTarget here
                    }
                }

                moveUnit(unitTarget, trgCoord.x, newPos_y);
            }
        }
        else { // srcCoord.y == trgCoord.y
            if (srcCoord.x < trgCoord.x) {
                // Attempt to find any rocks, if any exist then shorten the pushDst
                int maxNewPos_x = Mathf.Clamp(trgCoord.x + pushDst, 0, MAP_WIDTH - 1);
                int newPos_x;
                for (newPos_x = trgCoord.x; newPos_x < maxNewPos_x; newPos_x++) {
                    Tile newPosTile = map[newPos_x + 1, trgCoord.y].tile.GetComponent<Tile>();
                    if (newPosTile.type == Tile.ROCK) {
                        break; // Rock found, newPos is whatever was previously determined
                        // TODO: Damage unitTarget here
                    }
                }

                moveUnit(unitTarget, newPos_x, trgCoord.y);
            }
            else { // srcCoord.x > trgCoord.y
                // Attempt to find any rocks, if any exist then shorten the pushDst
                int minNewPos_x = Mathf.Clamp(trgCoord.x - pushDst, 0, MAP_WIDTH - 1);
                int newPos_x;
                for (newPos_x = trgCoord.x; newPos_x > minNewPos_x; newPos_x--) {
                    Tile newPosTile = map[newPos_x - 1, trgCoord.y].tile.GetComponent<Tile>();
                    if (newPosTile.type == Tile.ROCK) {
                        break; // Rock found, newPos is whatever was previously determined
                        // TODO: Damage unitTarget here
                    }
                }

                moveUnit(unitTarget, newPos_x, trgCoord.y);
            }
        }
    }

    // Ensures the tile press was valid, and calls the appropriate functions
    public void processTilePress(int x, int y) {
        GameObject newSelectedUnitGO = map[x, y].unit;
        Tile selectedTile = map[x, y].tile.GetComponent<Tile>();

        // CASE 1: State Select
        // Select the unit on the tile
        if (currentState == STATE_SELECT) {
            if (newSelectedUnitGO == null) {
                Debug.Log("No unit selected!");
            }
            else {
                selectUnit(newSelectedUnitGO);
                setValidTiles(Tile.MAT_GRASS_VALID_MOVEMENT, newSelectedUnitGO.GetComponent<Unit>().speed);
            }
        }

        // CASE 2: State Command
        // Move the unit to the specified space
        else if (currentState == STATE_COMMAND) {
            Assert.IsTrue(selectedUnit != null);

            bool changeToSelect = false;

            // Case i: Selected tile has previously selected unit on it
            if (newSelectedUnitGO == selectedUnit) { // User clicked the same unit twice
                changeToSelect = true;
            }
            // Case ii: Selected tile has no units on it and is valid
            else if (selectedTile.isValid && newSelectedUnitGO == null) {
                Debug.Log("Moving selected unit " + selectedUnit.name +
                    " to coordinates (" + x + "," + y + ").");
                moveUnit(selectedUnit, x, y);
                changeToSelect = true;
            }
            // Case iii: Selected tile has no units on it, and is not valid
            else if (!selectedTile.isValid) {
                Debug.Log("Coordinates (" + x + "," + y + ") are invalid!");
            }
            // Case iv: Selected tile has a non-selected unit on it
            else if (newSelectedUnitGO != null) { // User clicked any other unit
                Debug.Log("Can't move to a point occupied by another unit!");
            }

            if (changeToSelect == true) {
                resetValidTiles();
                deselectUnit();
            }
        } 

        // CASE 3: State Attack 1
        // Call units attack function
        else if (currentState == STATE_ATTACK_1) {
            bool changeToSelect = false;

            // You can overload the attack function for units that inherit from the basic unit
            // Case i: User selected same unit as before
            if (newSelectedUnitGO == selectedUnit) {
                changeToSelect = true;
            }
            // Case ii: User selected a valid tile with a unit on it
            else if (selectedTile.isValid && newSelectedUnitGO != null) {
                Debug.Log("Attacking tile at (" + x + ", " + y + ").");
                selectedUnit.GetComponent<Unit>().attack1(newSelectedUnitGO);
                changeToSelect = true;
            }
            // Case iii: User did not select a valid tile, or selected a valid tile with no unit on it
            else {
                Debug.Log("Coordinates (" + x + "," + y + ") are invalid!");
            }

            if (changeToSelect == true) {
                resetValidTiles();
                deselectUnit();
            }
        }
      
    }

    // Update is called once per frame
    void Update () {
        
        // Change to attack state when 1 is clicked
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            if (currentState == STATE_SELECT) {
                Assert.IsTrue(selectedUnit == null);
                Debug.Log("No unit selected!");
            }
            else {
                // State can be changed to STATE_ATTACK as long as a unit is selected
                resetValidTiles();
                currentState = STATE_ATTACK_1;
                int distance = selectedUnit.GetComponent<Unit>().attackRange;
                setValidTiles(Tile.MAT_GRASS_VALID_ATTACK, distance);
                Debug.Log("Unit " + selectedUnit.name + "is preparing to attack!");
            }
        }

        // DEBUG // DELETE THIS LATER
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            printLogicalGraph();
        }
    }

    // DEBUG FUNCTIONS
    // DELETE IT LATER
    void printLogicalGraph() {
        Debug.Log("Printing Logical Map of Units");  
        for (int i = MAP_HEIGHT-1; i >= 0; i--) {
            for (int j = 0; j < MAP_WIDTH; j++) {
                if (map[j, i].unit != null) {
                    Debug.Log("Coords " + j + ", " + i + "have a unit on them.");
                }
            }
        }
    }
}
