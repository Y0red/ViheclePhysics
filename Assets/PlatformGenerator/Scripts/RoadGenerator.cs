using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Directions of Roads
/// </summary>
enum Directions { Forward, Right, Right_UP, Right_Forward, Left, Left_Up, Left_Forward }
//[ExecuteInEditMode]
public class RoadGenerator : MonoBehaviour
{    
    Directions priviousDirection;

    [SerializeField] Platform newActive, first, last;
    GameObject platformPrifab;

     public Vector3 lastPos;

    [SerializeField] bool isFirstTime = true;
    [SerializeField] int forwardSizeLength, PlatformsToSpawn, allSpawnedPlatforms;
    private ObjPool objPool;

    public List<Platform> enabledPlatforms = new List<Platform>();

    private void Start()
    {
        lastPos = this.transform.position;
        objPool = GetComponent<ObjPool>();

        priviousDirection = Directions.Forward;

        GameEvents.current.onRoadWayTriggerEnter += OnIN;
        GameEvents.current.onRoadWayTriggerExit += OnOUT;
        GameEvents.current.OnGeneratePlatform += Starting;
    }
    public void Starting()
    {
        GeneratePlatforms();
        GameEvents.current.OnPlatformGenerated(first.playerStartingPos);
    }
    private void OnIN(Platform obj)
    {
        if(obj != null)
        {
            newActive = obj;
            first = obj;
            if (obj.isLast)
            {
                GeneratePlatforms();
            }
        }
    }
    private void OnOUT(Platform obj)
    {
        if (obj != null)
        {
            newActive = null;
            enabledPlatforms.Remove(obj);
            //obj.isEnabled = false;
        }
    }
    public void GeneratePlatforms()
    {
        for (int i = 0; i <= PlatformsToSpawn; i++)
        {
            ControllTileSpawn();   
        }
        for(int i = 0; i<= enabledPlatforms.Count; i++)
        {
            if (i == 0)
            {
               first = enabledPlatforms[i];
                first.isFirst = true;
            }
            else if(i == enabledPlatforms.Count -2)
            {
                last = enabledPlatforms[i];
                last.isLast = true;
            }
        }
    }
    void ControllTileSpawn()
    {
        if (isFirstTime) DoForward(forwardSizeLength);
        SpawnPlatforms();
    }
    private void DoForward(int index)
    {
        for (int x = 0; x < index; x++)
        {
            SpawnForward();
        }

        isFirstTime = false;
    }
    private void SpawnPlatforms()
    {
        int rand = UnityEngine.Random.Range(0, 4);

        if (rand < 2) SpawnRight();
        else if (rand == 2) SpawnForward();

        else if (rand > 2) SpawnLeft();
    }
    private void SpawnRight()
    {
        switch (priviousDirection)
        {
            case Directions.Forward:
                platformPrifab = objPool.GetAvailablePrifabFromDictionary("R_Right");
                platformPrifab.SetActive(true);

                platformPrifab.transform.position = lastPos + new Vector3(0, 0, 100);
                lastPos = platformPrifab.transform.position;

                enabledPlatforms.Add(platformPrifab.GetComponent<Platform>());

                priviousDirection = Directions.Right;
                break;
            case Directions.Right:
                platformPrifab = objPool.GetAvailablePrifabFromDictionary("R_Right_Forward");
                platformPrifab.SetActive(true);

                platformPrifab.transform.position = lastPos + new Vector3(80f, 0, 0);
                lastPos = platformPrifab.transform.position;

                enabledPlatforms.Add(platformPrifab.GetComponent<Platform>());

                priviousDirection = Directions.Right_Forward;
                break;
            case Directions.Right_Forward:
                platformPrifab = objPool.GetAvailablePrifabFromDictionary("R_Right_Up");
                platformPrifab.SetActive(true);

                platformPrifab.transform.position = lastPos + new Vector3(80f, 0, 0);
                lastPos = platformPrifab.transform.position;

                enabledPlatforms.Add(platformPrifab.GetComponent<Platform>());

                priviousDirection = Directions.Right_UP;
                break;
            case Directions.Right_UP:
                priviousDirection = Directions.Right_UP;
                SpawnForward();
                break;
            case Directions.Left:
                priviousDirection = Directions.Left;
                SpawnForward();
                break;
            case Directions.Left_Forward:
                priviousDirection = Directions.Left_Forward;
                SpawnLeft();
                break;
            case Directions.Left_Up:
                priviousDirection = Directions.Left_Up;
                SpawnLeft();
                break;
        }
        allSpawnedPlatforms++;
    }
    private void SpawnLeft()
    {
        switch (priviousDirection)
        {
            case Directions.Forward:
                platformPrifab = objPool.GetAvailablePrifabFromDictionary("R_Left");
                platformPrifab.SetActive(true);

                platformPrifab.transform.position = lastPos + new Vector3(0, 0, 100);
                lastPos = platformPrifab.transform.position;

                enabledPlatforms.Add(platformPrifab.GetComponent<Platform>());

                priviousDirection = Directions.Left;
                break;
            case Directions.Left:
                platformPrifab = objPool.GetAvailablePrifabFromDictionary("R_Left_Forward");
                platformPrifab.SetActive(true);

                platformPrifab.transform.position = lastPos + new Vector3(-80f, 0, 0);
                lastPos = platformPrifab.transform.position;

                enabledPlatforms.Add(platformPrifab.GetComponent<Platform>());

                priviousDirection = Directions.Left_Forward;
                break;
            case Directions.Left_Forward:
                platformPrifab = objPool.GetAvailablePrifabFromDictionary("R_Left_Up");
                platformPrifab.SetActive(true);

                platformPrifab.transform.position = lastPos + new Vector3(-80f, 0, 0);
                lastPos = platformPrifab.transform.position;

                enabledPlatforms.Add(platformPrifab.GetComponent<Platform>());

                priviousDirection = Directions.Left_Up;
                break;
            case Directions.Left_Up:
                priviousDirection = Directions.Left_Up;
                SpawnForward();
                break;
            case Directions.Right:
                priviousDirection = Directions.Right;
                SpawnForward();
                break;
            case Directions.Right_Forward:
                priviousDirection = Directions.Right_Forward;
                SpawnRight();
                break;
            case Directions.Right_UP:
                priviousDirection = Directions.Right_UP;
                SpawnRight();
                break;
        }
        allSpawnedPlatforms++;
    }
    private void SpawnForward()
    {
        switch (priviousDirection)
        {
            case Directions.Forward:
                platformPrifab = objPool.GetAvailablePrifabFromDictionary("R_Forward");
                platformPrifab.SetActive(true);

                platformPrifab.transform.position = lastPos + new Vector3(0, 0, 100);
                lastPos = platformPrifab.transform.position;

                enabledPlatforms.Add(platformPrifab.GetComponent<Platform>());

                priviousDirection = Directions.Forward;
                break;
            case Directions.Right:
                SpawnRight();
                priviousDirection = Directions.Right;
                break;
            case Directions.Left:
                priviousDirection = Directions.Left;
                SpawnLeft();
                break;
            case Directions.Right_Forward:
                priviousDirection = Directions.Right_Forward;
                SpawnRight();
                break;
            case Directions.Right_UP:
                platformPrifab = objPool.GetAvailablePrifabFromDictionary("R_Forward");
                platformPrifab.SetActive(true);

                platformPrifab.transform.position = lastPos + new Vector3(20, 0, 39);
                lastPos = platformPrifab.transform.position;

                enabledPlatforms.Add(platformPrifab.GetComponent<Platform>());

                priviousDirection = Directions.Forward;
                break;
            case Directions.Left_Forward:
                priviousDirection = Directions.Left_Forward;
                SpawnLeft();
                break;
            case Directions.Left_Up:
                platformPrifab = objPool.GetAvailablePrifabFromDictionary("R_Forward");
                platformPrifab.SetActive(true);

                platformPrifab.transform.position = lastPos + new Vector3(-20, 0, 39);
                lastPos = platformPrifab.transform.position;

                enabledPlatforms.Add(platformPrifab.GetComponent<Platform>());

                priviousDirection = Directions.Forward;
                break;
        }

        allSpawnedPlatforms++;
    }
    public void ResetPlatforms()
    {
        lastPos = this.transform.position;
        isFirstTime = true;
        allSpawnedPlatforms = 0;

        platformPrifab = null;

        objPool.ResetAll("R_Forward");
        objPool.ResetAll("R_Right");
        objPool.ResetAll("R_Left");
        objPool.ResetAll("R_Right_Up");
        objPool.ResetAll("R_Right_Forward");
        objPool.ResetAll("R_Left_Up");
        objPool.ResetAll("R_Left_Forward");
    }
    //public void SetActivePlatform(Platform platform)
    //{
       // activePlatform = platform;
    //}
    //public void ReSetActivePlatform()
    //{
        //activePlatform = null;
    //}
}
