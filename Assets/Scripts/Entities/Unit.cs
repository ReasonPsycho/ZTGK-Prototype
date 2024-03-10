using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, ISelectable
{
    public UnitState state = UnitState.IDLE;
<<<<<<< Updated upstream
=======
    public UnitType type;

    public bool IsSelected = false;
>>>>>>> Stashed changes

    public Grid grid;
    public Animator animator;
    public Vector2Int gridPosition;
    public Tile currentTile;
    private Tile prevTile;
    public float facingAngle;

    #region ISelectable

    public SELECTION_TYPE SelectionType
    {
        get { return SELECTION_TYPE.UNIT; }
    }

    private Color orgColor;
    private Material material;
    private bool isHovered = false;
    private bool isSelected = false;

    #endregion


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
        material = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        orgColor = material.color;
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

    #region ISelectable

    public void OnHoverEnter()
    {
        if (!isHovered)
        {
            material.color = Color.cyan;
            isHovered = true;
        }
    }

    public void OnHoverExit()
    {
        if (!isSelected)
        {
            material.color = orgColor;
        }

        isHovered = false;
    }

    public virtual void OnSelect()
    {
        if (!isSelected)
        {
            material.color = Color.blue;
        }

        isSelected = true;
        
    }

    public virtual void OnDeselect()
    {
        material.color = orgColor;
        isHovered = false;
        isSelected = false;
    }

    #endregion
}