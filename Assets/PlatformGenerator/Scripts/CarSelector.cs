using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarSelector : MonoBehaviour
{
    public CustomVihecle[] cars;
    public Button next, priv, select;
    public GameObject menu;
    int index = 1;
    private void Start()
    {
        next.onClick.AddListener(OnNext);
        priv.onClick.AddListener(OnPriv);
        select.onClick.AddListener(OnSelect);
        UpdateSelection(index);
    }

    private void OnSelect()
    {
        cars[index].gameObject.SetActive(false);
        GameEvents.current.OnSelected(cars[index]);
        GameEvents.current.OnPlatformGenerate();
        menu.SetActive(false);
    }

    private void OnPriv()
    {
        Deactivate(index);
        index = (index - 1) % cars.Length;
        UpdateSelection(index);
    }
    private void OnNext()
    {
        Deactivate(index);
        index = (index + 1) % cars.Length;
        UpdateSelection(index);
    }
    void UpdateSelection(int index)
    {
        Debug.Log(index);

        cars[index].gameObject.SetActive(true);
    }
    private void Deactivate(int index)
    {
        Debug.Log(index);
        cars[index].gameObject.SetActive(false);
    }
}
