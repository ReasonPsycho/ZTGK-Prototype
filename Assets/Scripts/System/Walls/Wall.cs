using System;
using System.Collections;
using System.Collections.Generic;
using Buildings;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


public class Wall : Building
{
    public float x;
    public float z;
    public UnityEvent OnStartCompleted;

    //  NORTH -> Z+ | SOUTH -> Z- | EAST -> X+ | WEST -> X- | TOP -> Y+
    
    public GameObject wallSidePrefab;

    public GameObject northSide; // rot. X 90
    public GameObject southSide; // rot. X -90
    public GameObject eastSide; // rot. Z -90
    public GameObject westSide; // rot. Z 90
    public GameObject topSide; // rot.  0


    public override void Start()
    {
        base.Start();
        buildingType = BuildingType.WALL;
        wallSidePrefab = GameObject.Find("ConstructionManager").GetComponent<ConstructionManager>().wallSide;
    }


    public GameObject SetWall() {
        var tile = tiles[ 0 ];
        
        topSide = Instantiate(wallSidePrefab,
            new Vector3(transform.position.x, tile.grid.cellSize, transform.position.z), Quaternion.identity,
            this.transform); //instantiates the top side of the wall as a child of the wall object

        Tile northNeighbour = tile.grid.GetTile(new Vector2Int(tile.Index.x + 1, tile.Index.y));
        Tile southNeighbour = tile.grid.GetTile(new Vector2Int(tile.Index.x - 1, tile.Index.y));
        Tile eastNeighbour = tile.grid.GetTile(new Vector2Int(tile.Index.x, tile.Index.y + 1));
        Tile westNeighbour = tile.grid.GetTile(new Vector2Int(tile.Index.x, tile.Index.y - 1));

        if (northNeighbour == null || northNeighbour.Vacant || northNeighbour.BuildingHandler == null ||
            northNeighbour.BuildingHandler.buildingType != BuildingType.WALL)
        {
            northSide = Instantiate(wallSidePrefab,
                new Vector3(transform.position.x + tile.grid.cellSize / 2.0f, tile.grid.cellSize / 2.0f,
                    transform.position.z), Quaternion.Euler(0, 0, -90), this.transform);
        }

        if (southNeighbour == null || southNeighbour.Vacant || southNeighbour.BuildingHandler == null ||
            southNeighbour.BuildingHandler.buildingType != BuildingType.WALL)
        {
            southSide = Instantiate(wallSidePrefab,
                new Vector3(transform.position.x - tile.grid.cellSize / 2.0f, tile.grid.cellSize / 2.0f,
                    transform.position.z), Quaternion.Euler(0, 0, 90), this.transform);
        }

        if (eastNeighbour == null || eastNeighbour.Vacant || eastNeighbour.BuildingHandler == null ||
            eastNeighbour.BuildingHandler.buildingType != BuildingType.WALL)
        {
            eastSide = Instantiate(wallSidePrefab,
                new Vector3(transform.position.x, tile.grid.cellSize / 2.0f,
                    transform.position.z + tile.grid.cellSize / 2.0f), Quaternion.Euler(90, 0, 0), this.transform);
        }

        if (westNeighbour == null || westNeighbour.Vacant || westNeighbour.BuildingHandler == null ||
            westNeighbour.BuildingHandler.buildingType != BuildingType.WALL)
        {
            westSide = Instantiate(wallSidePrefab,
                new Vector3(transform.position.x, tile.grid.cellSize / 2.0f,
                    transform.position.z - tile.grid.cellSize / 2.0f), Quaternion.Euler(-90, 0, 0), this.transform);
        }

        ApplyNewMaterial();
        return this.gameObject;
    }

    public void ApplyNewMaterial()
    {
        foreach (MeshRenderer childRenderer in transform.GetComponentsInChildren<MeshRenderer>())
        {
            if (childRenderer != null)
            {
                if (material != null)
                {
                    childRenderer.material = this.material;
                }
                else
                {
                    Debug.LogError("newMaterial is not initialized.");
                }
            }
        }
    }


    public override bool DestroyBuilding() {
        var tile = tiles[ 0 ];

        Tile northNeighbour = tile.grid.GetTile(new Vector2Int(tile.Index.x + 1, tile.Index.y));
        Tile southNeighbour = tile.grid.GetTile(new Vector2Int(tile.Index.x - 1, tile.Index.y));
        Tile eastNeighbour = tile.grid.GetTile(new Vector2Int(tile.Index.x, tile.Index.y + 1));
        Tile westNeighbour = tile.grid.GetTile(new Vector2Int(tile.Index.x, tile.Index.y - 1));

        if (northNeighbour != null && northNeighbour.BuildingHandler != null &&
            northNeighbour.BuildingHandler.buildingType == BuildingType.WALL)
        {
            ((Wall)northNeighbour.BuildingHandler).SetWall();
        }

        if (southNeighbour != null && southNeighbour.BuildingHandler != null &&
            southNeighbour.BuildingHandler.buildingType == BuildingType.WALL)
        {
            ((Wall)southNeighbour.BuildingHandler).SetWall();
        }

        if (eastNeighbour != null && eastNeighbour.BuildingHandler != null &&
            eastNeighbour.BuildingHandler.buildingType == BuildingType.WALL)
        {
            ((Wall)eastNeighbour.BuildingHandler).SetWall();
        }

        if (westNeighbour != null && westNeighbour.BuildingHandler != null &&
            westNeighbour.BuildingHandler.buildingType == BuildingType.WALL)
        {
            ((Wall)westNeighbour.BuildingHandler).SetWall();
        }

        tiles[0].Build(GameObject.Find("ConstructionManager").GetComponent<ConstructionManager>().floor,
            BuildingType.FLOOR);
        Destroy(gameObject);
        return true;
    }

    public  override void GetMaterial()
    {

    }

    public void GetWall()
    {
        material = new Material(Shader.Find("Standard"));
        material.name = "Wall side material";
        orgColor = material.color;
    }
}