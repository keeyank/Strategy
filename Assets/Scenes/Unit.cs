using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Unit : MonoBehaviour {

    public TileMap tileMap;

    public bool hasAttacked = false;
    public bool hasMoved = false;

    protected int maxHP;
    protected int currentHP;
    protected int speed;
    protected int attack1Range;
    protected int attack1Buffer;
    protected int attack2Range;
    protected int attack2Buffer;
    protected int attack3Range;
    protected int attack3Buffer;

    public Unit(int maxHP, int speed, int attack1Range, int attack1Buffer, 
        int attack2Range, int attack2Buffer, int attack3Range, int attack3Buffer) {
        this.maxHP = maxHP;
        this.currentHP = maxHP;
        this.speed = speed;
        this.attack1Range = attack1Range;
        this.attack1Buffer = attack1Buffer;
        this.attack2Range = attack2Range;
        this.attack2Buffer = attack2Buffer;
        this.attack3Range = attack3Range;
        this.attack3Buffer = attack3Buffer;
    }

    public int MaxHP { get { return maxHP; } }
    public int CurrentHP {
        get { return currentHP; }
        set { 
            currentHP = value;
            // Destroy unit if below 0 HP
            if (currentHP <= 0) {
                Destroy(gameObject);
            }
            // Ensure HP is capped at maxHP if unit was healed
            if (currentHP > maxHP) {
                currentHP = maxHP;
            }

            Debug.Log(gameObject.name + "now has " + currentHP + "HP.");    
        }
    }
    public int Speed { get { return speed; } }
    public int Attack1Range { get { return attack1Range; } }
    public int Attack1Buffer { get { return attack1Buffer; } }
    public int Attack2Range { get { return attack2Range; } }
    public int Attack2Buffer { get { return attack2Buffer; } }
    public int Attack3Range { get { return attack3Range; } }
    public int Attack3Buffer { get { return attack3Buffer; } }

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
    public abstract void attack3(Vector2Int targetPos, GameObject unitTarget);

}
