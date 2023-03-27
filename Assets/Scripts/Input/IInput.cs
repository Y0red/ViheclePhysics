using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInput 
{
    float Acceleration { get;  }

    float Steering { get; }

    bool Braking { get; }
}
