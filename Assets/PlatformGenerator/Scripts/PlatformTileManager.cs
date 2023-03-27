using System.Collections;
using UnityEngine;
//[ExecuteInEditMode]
public class PlatformTileManager : MonoBehaviour
{
    enum Directions { Forward, Right, Left}
    Directions priviousDirection;

    [SerializeField] Platform lastPlatform;
    GameObject platformPrifab;

    [SerializeField]public Vector3 lastPos;
    [SerializeField]float sizeX , sizeZ;

    [SerializeField] bool isFirstTime = true;
    [SerializeField] int forwardSizeLength;
    [SerializeField] private ObjPool objPool;

    public int allPlatforms, livePlatforms;
    public int spawnedPlatforms;
    public int currentPlatform;

    private void Start()
    {
        lastPos = this.transform.position;
        objPool = GetComponent<ObjPool>();
        //InvokeRepeating("ControllTileSpawn", 1f, .2f);

        GeneratePlatforms();
    }
    public void GeneratePlatforms()
    {
        for(int i = 0; i <= allPlatforms; i++)
        {
            ControllTileSpawn();
        }
        lastPlatform = platformPrifab.GetComponent<Platform>();
        lastPlatform.isLast = true;
        //lastPlatform.plaMan = this;
    }
    void ControllTileSpawn()
    {
       if (isFirstTime) DoForward(forwardSizeLength);
       SpawnPlatforms();
    }
    private void DoForward(int index)
    {

        for (int x = 0; x < index; x++) SpawnForward();

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
        if(priviousDirection == Directions.Right)
        {
            platformPrifab = objPool.GetAvailablePrifabFromDictionary("Right");
            platformPrifab.SetActive(true);

            platformPrifab.transform.position = lastPos + new Vector3(10f * 3, 0, 0);
            lastPos = platformPrifab.transform.position;
        }
        if (priviousDirection == Directions.Left)
        {
            SpawnForward();
            return;
        }
        if (priviousDirection == Directions.Forward)
        {
            platformPrifab = objPool.GetAvailablePrifabFromDictionary("Right");
            platformPrifab.SetActive(true);

            platformPrifab.transform.position = lastPos + new Vector3(0, 0, 14f * 3);
            lastPos = platformPrifab.transform.position;
        }

        priviousDirection = Directions.Right;
        spawnedPlatforms++;
    }
    private void SpawnLeft()
    {
        if (priviousDirection == Directions.Left)
        {
            platformPrifab = objPool.GetAvailablePrifabFromDictionary("Left");
            platformPrifab.SetActive(true);

            platformPrifab.transform.position = lastPos + new Vector3(-10f * 3, 0, 0);
            lastPos = platformPrifab.transform.position;
        }
        if (priviousDirection == Directions.Right)
        {
            SpawnForward();
            return;
        }
        if (priviousDirection == Directions.Forward)
        {
            platformPrifab = objPool.GetAvailablePrifabFromDictionary("Left");
            platformPrifab.SetActive(true);

            platformPrifab.transform.position = lastPos + new Vector3(0, 0, 14f * 3);
            lastPos = platformPrifab.transform.position;
        }

        priviousDirection = Directions.Left;
        spawnedPlatforms++;
    }
    private void SpawnForward()
    {
        if(priviousDirection == Directions.Forward)
        {
            platformPrifab = objPool.GetAvailablePrifabFromDictionary("Forward");
            platformPrifab.SetActive(true);

            sizeZ = 10f;////platformPrifab.transform.localScale.x;
            platformPrifab.transform.position = lastPos + new Vector3(0, 0, sizeZ);
            lastPos = platformPrifab.transform.position;
        }
        if(priviousDirection == Directions.Right)
        {
            platformPrifab = objPool.GetAvailablePrifabFromDictionary("Forward");
            platformPrifab.SetActive(true);

            sizeZ = 0;//platformPrifab.transform.localScale.x;
            platformPrifab.transform.position = lastPos + new Vector3(12.15f * 3, 0, 1.5f * 3);
            lastPos = platformPrifab.transform.position;
        }
        if (priviousDirection == Directions.Left)
        {
            platformPrifab = objPool.GetAvailablePrifabFromDictionary("Forward");
            platformPrifab.SetActive(true);

            sizeZ = 0;//platformPrifab.transform.localScale.x;
            platformPrifab.transform.position = lastPos + new Vector3(-12.93f * 3, 0, 1.5f * 3);
            lastPos = platformPrifab.transform.position;
        }

        priviousDirection = Directions.Forward;
        spawnedPlatforms++;
    }
    public void ResetPlatforms()
    {
        lastPos = this.transform.position;
        isFirstTime = true;
        currentPlatform = 0;
        spawnedPlatforms = 0;

        sizeX = 0;
        sizeZ = 0;

        objPool.ResetAll("Forward");
        objPool.ResetAll("Right");
        objPool.ResetAll("Left");
    }
}
