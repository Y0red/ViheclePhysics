using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Vihecle Engine", menuName = "My Vihecles/Engine")]
public class Engine_SO : ScriptableObject
{
    #region Variables
    //public string name =  string.Empty;
    public bool setManually = false;
    public bool saveDataOnClose = false;

    public float antiRoll = 10000.0f;

    [Header("utils")]
    public float motorForce = 0.0f;
    public float brakForce = 0.0f;
    public float topSpeed = 0.0f;
    public float currentdSpeed = 0.0f;
    public float resetTime = 0.0f;

    public float MaxSteeringAngel = 40f;

    [Header("Chassis Fields")]
    //public Transform COM;
    // Script will simulate chassis movement based on vehicle rigidbody situation.
    //public GameObject chassis;
    // Chassis Vertical Lean Sensitivity
    public float chassisVerticalLean = 4.0f;
    // Chassis Horizontal Lean Sensitivity
    public float chassisHorizontalLean = 4.0f;

    public float horizontalLean = 0.0f;
    public float verticalLean = 0.0f;

    [Header("Gears")]
    // Gears.
    public int currentGear;
    public int totalGears = 6;
    public bool changingGear = false;

    public bool engineStarting = false;
    public bool reversing = false;
    public bool IsBraking = false;
    public bool engineRunning = false;
    public bool runEngineAtAwake = true;

    public bool UseTerrainSplatMapForGroundPhysic = false;

    public float currentBreakForce;

    #endregion

    #region Constructers
    public float GetTopSpeed
    {
        get
        {
            return topSpeed;
        }
    }

    public float GetBraking
    {
        get { return brakForce; }

    }

    public float GetMotorForce
    {
        get { return motorForce; }

    }
    public string GetName
    {
        get { return name; }
    }
    #endregion
}
