using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Platform : MonoBehaviour
{
    public bool isFirst = false;
    public bool isLast = false;
    public bool isActive = false;
    public bool isEnabled = false;
    public Transform playerStartingPos;
    public GameObject EXIT;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            GameEvents.current.DoorWayTriggerEnter(this);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player") isActive = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isActive = false;
            isFirst = false;
            isLast = false;
            isEnabled = false;

            GameEvents.current.DoorWayTriggerExit(this);
        }

        if(!isEnabled && !isActive && !isFirst && !isLast && !isEnabled)
        {
            StartCoroutine(WaitTimer(5f));
        }
    }
    IEnumerator WaitTimer(float time)
    {
        yield return new WaitForSeconds(time);
        if (!isEnabled && !isActive && !isFirst && !isLast && !isEnabled)
        {
            this.gameObject.SetActive(false);
        }
        //StartCoroutine(WaitTimer(5f));
    }

    private void OnEnable()
    {
        isEnabled = true;
    }
    private void OnDisable()
    {
        isEnabled = false;
    }
}