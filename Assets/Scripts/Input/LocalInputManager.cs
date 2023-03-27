using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LocalInputManager : MonoBehaviour, IInput
{
    public float Acceleration { get { return m_Acceleration; }  }
    public float Steering { get { return m_Steering; } }
    public bool Braking { get { return m_Braking; } }

    //[Range(-1.0f, 1.0f)]
    public float m_Acceleration;
   // [Range(-1f, 1f)]
    public float m_Steering;
   private bool m_Braking;

    //[Range(-1f, 1f)]
    public float LocalInput = 0;
   // [Range(-1f, 1f)]
    public float LocalInputSteer = 0;
    public bool LocalInputBrake;

    public bool isMOb = false;

    private CarController carcontroller;
   // public PlayerInput playerInput;

    private void Awake()
    {
        carcontroller = new CarController();
    }

    public void BrakeBtnDown()
    {
        m_Braking = true;
        LocalInputBrake = true;
    }
    public void BrakeBtnUp()
    {
        m_Braking = false;
        LocalInputBrake = false;
    }

    public void AccelerateBtnDown()
    {

        m_Acceleration =+ .1f;
         LocalInput =+ .1f;
       // m_Acceleration = Time.deltaTime * 0.1f;
        
        Debug.Log("Ac");
    }
    public void AccelerateBtnUp()
    {
        m_Acceleration = 0f;
        LocalInput = 0f;
    }

    public void DecelerateBtnDown()
    {
        m_Acceleration -= .1f;
        LocalInput -= .1f;
    }
    public void DecelerateBtnUp()
    {
        m_Acceleration = 0f;
        LocalInput = 0f;
    }

    public void SteerLeftBtnDown()
    {
        m_Steering -= .1f;
        LocalInputSteer -= .1f;
    }
    public void SteerLeftBtnUp()
    {
        m_Steering = 0f;
        LocalInputSteer = 0f;
    }

    public void SteerRightBtnDown()
    {
        m_Steering += .1f;
        LocalInputSteer += .1f;
    }
    public void SteerRightBtnUp()
    {
        m_Steering = 0f;
        LocalInputSteer = 0f;
    }

    private void Update()
    {
        if (isMOb)
        {
            carcontroller.car.Steer.performed += ctx => m_Steering = ctx.ReadValue<Vector2>().x;
            carcontroller.car.Steer.canceled += ctx => m_Steering = ctx.ReadValue<Vector2>().x;

            carcontroller.car.Steer.performed += ctx => m_Acceleration = ctx.ReadValue<Vector2>().x;
            carcontroller.car.Steer.canceled += ctx => m_Acceleration = ctx.ReadValue<Vector2>().x;

            // Vector2 input = carcontroller.car.Steer.ReadValue<Vector2>();
            // m_Steering = input.x;
            // LocalInputSteer = input.x;

            // carcontroller.car.Steer1.performed += AccelerateBtnDow; 
            //m_Acceleration = input2.y;
            // LocalInput = input2.y;

            float keyBoardSteer = carcontroller.car1.SteerKeyboard.ReadValue<float>();
           // m_Steering = keyBoardSteer;
            //LocalInputSteer = keyBoardSteer;

            float keyBoardDrive = carcontroller.car1.DriveKeyboard.ReadValue<float>();
            //m_Acceleration = keyBoardDrive;
            //LocalInput = keyBoardDrive;

            float bbBraking = carcontroller.car1.Brake.ReadValue<float>();


            if (bbBraking >= 1.0f)
            {
                BrakeBtnDown();
            }
            if (bbBraking < 1.0f)
            {
                BrakeBtnUp();
            }
        }

    }

    private void AccelerateBtnDow(InputAction.CallbackContext obj)
    {
        AccelerateBtnDown();
    }

    private void OnEnable()
    {
        carcontroller.Enable();
    }
    private void OnDisable()
    {
        carcontroller.Disable();
    }
}
