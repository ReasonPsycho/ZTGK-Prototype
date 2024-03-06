using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    public UnitState state = UnitState.IDLE;


    public bool IsSelected = false;

    public Grid grid;
    public Vector2Int gridPosition;
    public Tile currentTile;
    private Tile prevTile;
    public float facingAngle;


    [Header("General")]
    public float MaxHealth = 100.0f;
    private float health;
    public float reachRange = 1.0f;   // How close to the target we need to be to interact with it - e.g. mine, attack, etc.

    [Header("Movement")]
    public float movementSpeed = 3.0f;
    public float rotationSpeed = 3.0f;


    [Header("Mining")]
    public float miningSpeed = 1.0f;

    [Header("Combat")]
    public float attackSpeed = 1.0f;

    //[Header("Equipment")]
    //TODO 

    public void Start()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        health = MaxHealth;
        prevTile = grid.GetTile(gridPosition);
    }
    private void Update()
    {
        gridPosition = grid.WorldToGridPosition(transform.position);
        currentTile = grid.GetTile(gridPosition);
        grid.GetTile(gridPosition).Vacant = false;

        if(prevTile != currentTile)
        {
            grid.GetTile(prevTile.Index).Vacant = true;
        }

        prevTile = currentTile;
    }

    public void TakeDmg(float dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Die();
        }
    }

    public void Heal(float hp)
    {
        health += hp;
        if (health > MaxHealth)
        {
            health = MaxHealth;
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    //change clr to show selection
    private void OnDrawGizmos()
    {
        if (IsSelected)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1.0f);
        }
    }

    public void Select()
    {
        IsSelected = true;
    }

    public void Deselect()
    {
        IsSelected = false;
    }
}
