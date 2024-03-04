using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Wall : Building
{
    float cellSize;
    public float x;
    public float z;


    //  NORTH -> Z+ | SOUTH -> Z- | EAST -> X+ | WEST -> X- | TOP -> Y+

    public GameObject wallSidePrefab;

    public GameObject northSide;     // rot. X 90
    public GameObject southSide;     // rot. X -90
    public GameObject eastSide;      // rot. Z -90
    public GameObject westSide;      // rot. Z 90
    public GameObject topSide;       // rot.  0


    private void Start()
    {
        cellSize = GameObject.Find("Ground").GetComponent<Grid>().cellSize;

        //wallSidePrefab = Resources.Load<GameObject>("Assets/Prefabs/Wall/WallSide.prefab");
        //if (wallSidePrefab == null)
        //{
        //    Debug.LogError("Wall side prefab not found!");
        //}


        SetWall();

    }


    public GameObject SetWall() 
    {
        wallSidePrefab = PrefabUtility.LoadPrefabContents("Assets/Prefabs/Wall/WallSide.prefab") as GameObject;

        //set scale so that the wall is the same size as the cell
        Vector3 prfbSize = wallSidePrefab.GetComponent<MeshRenderer>().bounds.size;
        wallSidePrefab.transform.localScale = new Vector3(cellSize / prfbSize.x, 1, cellSize / prfbSize.z);

        if (wallSidePrefab == null)
        {
            Debug.LogError("Wall side prefab not loaded!");
            return null;
        }


        topSide = Instantiate(wallSidePrefab, new Vector3(transform.position.x, cellSize, transform.position.z), Quaternion.identity);     //instantiates the top side of the wall as a child of the wall object

        northSide = Instantiate(wallSidePrefab, new Vector3(transform.position.x, cellSize/2.0f, transform.position.z + cellSize / 2.0f), Quaternion.Euler(90, 0, 0));

        southSide = Instantiate(wallSidePrefab, new Vector3(transform.position.x, cellSize/2.0f, transform.position.z - cellSize / 2.0f), Quaternion.Euler(-90, 0, 0));

        eastSide = Instantiate(wallSidePrefab, new Vector3(transform.position.x + cellSize / 2.0f, cellSize / 2.0f, transform.position.z), Quaternion.Euler(0, 0, -90));

        westSide = Instantiate(wallSidePrefab, new Vector3(transform.position.x - cellSize / 2.0f, cellSize / 2.0f, transform.position.z), Quaternion.Euler(0, 0, 90));

        return this.gameObject;
    }

    public void UpdateWall(Wall wallToUpdate)
    {
        
        RaycastHit hitNorth;
        if (Physics.Raycast(new Vector3(x, 0, z + cellSize/2.0f), Vector3.up, out hitNorth, 1))
        {
            if (hitNorth.collider.gameObject.layer == 6)     //layer 6 is wall layer
            {
                //wallToUpdate.northSide.SetActive(false);
                print("NORTH HIT");
            }
        }
        else
        {
            //wallToUpdate.northSide.SetActive(true);
            print("NoHitNorth");

        }
    }
}
