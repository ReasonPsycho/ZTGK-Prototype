using System;
using System.Collections;
using System.Collections.Generic;
using GameItems;
using GameItems.ConcreteItems;
using UnityEngine;

public class Unit : MonoBehaviour, ISelectable
{
    public UnitState state = UnitState.IDLE;

    public UnitType type;

    public bool IsSelected = false;

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

    [Header("Equipment")]
    public GameItem Item1;
    public GameItem Item2;
    // Accumulators to avoid checking both items every attack. Modified on Apply/Unapply, Equip/Unequip.
    public float FlatDamageBuff = 0;
    public float PercentDamageBuff = 0;
    public float Armor = 0;

    private bool firstUpdate = true;

    public void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        health = MaxHealth;
        prevTile = grid.GetTile(gridPosition);
        
        material = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        orgColor = material.color;

        Equip(new GIMop(), 1,true);
        Equip(new GILanceMop(), 2,true);
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
        //print(prevTile.x + "  "+ prevTile.y);
       // print(currentTile.x +"  " + prevTile.y);
        grid.GetTile(gridPosition).Vacant = false;

        if (prevTile != currentTile)
        {
            //print("Tile changed");
            if (prevTile.BuildingHandler.buildingType == Buildings.BuildingType.FLOOR || prevTile.Building == null)
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

    #region Equipment

    /// <summary>
    /// Try equip item to specified slot, any by default.
    /// Switch the items if force is on.
    /// Does not use Unequip.
    /// </summary>
    /// <param name="item">Item to equip</param>
    /// <param name="slotMask">Slot to fill. 1 for Item1, 2 for Item2, 3 for any.</param>
    /// <param name="force">If true, remove the previously equipped item. If true and slotMask is Any and both slots are filled, Item1 will be unequipped.</param>
    /// <returns>
    /// Tuple of boolean marking success or failure and GameItem that was unequipped in force mode, null otherwise.
    /// </returns>
    /// <returns></returns>
    public (bool, GameItem) Equip(GameItem item, int slotMask = 0b11, bool force = false) {
        if (item == null) return (false, null);
        
        if ( (slotMask & 0b01) != 0 && Item1 != null ) {
            Item1 = item;
            Apply(Item1);
            return (true, null);
        }

        if ( (slotMask & 0b10) != 0 && Item2 != null ) {
            Item2 = item;
            Apply(Item2);
            return (true, null);
        }

        if ( force ) {
            if ( (slotMask & 0b01) != 0 ) {
                var ret = Item1;
                Item1 = item;
                Unapply(ret);
                Apply(item);
                return (true, ret);
            }

            if ( (slotMask & 0b10) != 0 ) {
                var ret = Item2;
                Item2 = item;
                Unapply(ret);
                Apply(item);
                return (true, ret);
            }
        }

        return (false, null);
    }

    /// <summary>
    /// Unequips specified items and returns them as a list.
    /// </summary>
    /// <param name="slotMask">Slot to free. 1 for Item1, 2 for Item2, 3 for both.</param>
    /// <returns></returns>
    public List<GameItem> Unequip(int slotMask) {
        List<GameItem> ret = new();

        if ( (slotMask & 0b01) != 0 && Item1 != null ) {
            ret.Add(Item1);
            Unapply(Item1);
            Item1 = null;
        }

        if ( (slotMask & 0b10) != 0 && Item2 != null ) {
            ret.Add(Item2);
            Unapply(Item2);
            Item2 = null;
        }

        return ret;
    }

    /// <summary>
    /// Apply item buffs to accumulator stats.
    /// </summary>
    /// <param name="item">Item to apply.</param>
    public void Apply(GameItem item) {
        if (item == null) return;

        MaxHealth += item.HealthBuff;
        FlatDamageBuff += item.FlatDamageBuff;
        PercentDamageBuff += item.PercentDamageBuff;
        Armor += item.ArmorBuff;
    }

    /// <summary>
    /// Remove applied buffs from accumulator stats.
    /// </summary>
    /// <param name="item">Item to unapply.</param>
    public void Unapply(GameItem item) {
        if (item == null) return;

        MaxHealth -= item.HealthBuff;
        FlatDamageBuff -= item.FlatDamageBuff;
        PercentDamageBuff -= item.PercentDamageBuff;
        Armor -= item.ArmorBuff;
    }

    #endregion
}