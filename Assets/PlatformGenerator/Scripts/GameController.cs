using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameController : MonoBehaviour
{
    public CustomVihecle player;
    public CinemachineVirtualCamera cv;
    public GameObject joyStick;
    void Start()
    {
        GameEvents.current.OnSelectedCar += OnCarSelected;
        GameEvents.current.OnPlatformCreated += OnPlatformCreated;
    }

    private void OnCarSelected(CustomVihecle obj)
    {
        if(obj != null)
        {
            player = obj;
            player.isAiDriver = false;
            player.isON = false;
        }
    }

    private void OnPlatformCreated(Transform obj)
    {
        if(obj != null)
        {
            player.transform.position = obj.position;
            player.gameObject.SetActive(true);

            cv.Follow = player.gameObject.transform;
            cv.LookAt = player.gameObject.transform;
            joyStick.SetActive(true);
        }
    }

    
}
