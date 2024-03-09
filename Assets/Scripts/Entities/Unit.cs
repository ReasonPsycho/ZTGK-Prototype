using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitState state = UnitState.IDLE;


    public bool IsSelected = false;

    public Grid grid;
    public Animator animator;
    public Vector2Int gridPosition;
    public Tile currentTile;
    private Tile prevTile;
    public float facingAngle;


    [Header("General")] public float MaxHealth = 100.0f;
    private float health;

    public float
        reachRange = 1.0f; // How close to the target we need to be to interact with it - e.g. mine, attack, etc.

    [Header("Movement")] public float tilesPerSecond = 3.0f;
    public float rotationSpeed = 3.0f;


    [Header("Mining")] public float miningSpeed = 1.0f;

    [Header("Combat")] public float attackSpeed = 1.0f;

    //[Header("Equipment")]
    //TODO 

    private bool firstUpdate = true;

    public void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        health = MaxHealth;
        prevTile = grid.GetTile(gridPosition);
    }

    private void Update()
    {
        if (firstUpdate)
        {
            grid = GameObject.Find("Grid").GetComponent<Grid>();
            firstUpdate = false;
        }

        gridPosition = grid.WorldToGridPosition(transform.position);
        currentTile = grid.GetTile(gridPosition);
        grid.GetTile(gridPosition).Vacant = false;

        if (prevTile != currentTile)
        {
            if (prevTile.Building == null)
            {
                prevTile.Vacant = true;
                //grid.GetTile(prevTile.Index).Vacant = true;
            }
        }

        prevTile = currentTile;


        if (state == UnitState.MOVING)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsWorking", false);
        }
        else if (state == UnitState.MINING)
        {
            animator.SetBool("IsWorking", true);
            animator.SetBool("IsWalking", false);
        }
        else
        {
            animator.SetBool("IsWorking", false);
            animator.SetBool("IsWalking", false);
        }
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