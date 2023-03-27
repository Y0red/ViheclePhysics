using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;
    private void Awake()
    {
        current = this;
    }

    public event Action<Platform> onRoadWayTriggerEnter;
    public event Action<Platform> onRoadWayTriggerExit;

    public event Action<Transform> OnPlatformCreated, OnDessable;
    public event Action<CustomVihecle> OnSelectedCar;
    public event Action OnGeneratePlatform;
    public void DoorWayTriggerEnter(Platform id)
    {
        if(onRoadWayTriggerEnter != null)
        {
            onRoadWayTriggerEnter(id);
        }
    }

    public void DoorWayTriggerExit(Platform id)
    {
        if(onRoadWayTriggerExit != null)
        {
            onRoadWayTriggerExit(id); 
        }
    }

    public void OnPlatformGenerated(Transform id)
    {
        if (OnPlatformCreated != null)
        {
            OnPlatformCreated(id);
        }
    }
    public void OnPlatformGenerate()
    {
        if (OnGeneratePlatform != null)
        {
            OnGeneratePlatform();
        }
    }
    public void OnSelected(CustomVihecle id)
    {
        if (OnSelectedCar != null)
        {
            OnSelectedCar(id);
        }
    }
}
