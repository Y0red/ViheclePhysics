using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelColl : MonoBehaviour
{
    private Rigidbody carRigId;

    private float startSlipValue = .25f;
    private WheelCollider wheelCollider;

    private float wheelSlipAmount_SideWays;
    private float wheelSlipAmount_Forward;

    //physics materials
    private PhysicMaterial grassPhysicsMaterial;
    private PhysicMaterial sandPhysicsMaterial;

    //wheel friction curves and stiffness
    private WheelFrictionCurve forwardFrictionCurve;
    private WheelFrictionCurve sidewaysFrictionCurve;

    private float defForwardStiffness = 0f;
    private float defSideWaysStiffness = 0f;

    private Skidmarks skidmarks = null;
    private int lastSkidmark = -1;


    private void Start()
    {
        

        wheelCollider = GetComponent<WheelCollider>();
        carRigId = GetComponentInParent<Rigidbody>();

        if (FindObjectOfType(typeof(Skidmarks)))
           skidmarks = FindObjectOfType(typeof(Skidmarks)) as Skidmarks;
        else
            Debug.Log("No skidmarks object found. Skidmarks will not be drawn. Drag ''RCCSkidmarksManager'' from Prefabs folder, and drop on to your existing scene...");

        grassPhysicsMaterial = Resources.Load("RCCGrassPhysics") as PhysicMaterial;
        sandPhysicsMaterial = Resources.Load("RCCSandPhysics") as PhysicMaterial;

        forwardFrictionCurve = GetComponent<WheelCollider>().forwardFriction;
        sidewaysFrictionCurve = GetComponent<WheelCollider>().sidewaysFriction;

        defForwardStiffness = forwardFrictionCurve.stiffness;
        defSideWaysStiffness = sidewaysFrictionCurve.stiffness;
    }


    private void FixedUpdate()
    {

        if (skidmarks)
        {

            WheelHit GroundHit;
            wheelCollider.GetGroundHit(out GroundHit);

            wheelSlipAmount_SideWays = Mathf.Abs(GroundHit.sidewaysSlip);
            wheelSlipAmount_Forward = Mathf.Abs(GroundHit.forwardSlip);

           if (wheelSlipAmount_SideWays > startSlipValue || wheelSlipAmount_Forward > .5f)
            {

                Vector3 skidPoint = GroundHit.point + 2f * (carRigId.velocity) * Time.deltaTime;

                if (carRigId.velocity.magnitude > 1f)
                    lastSkidmark = skidmarks.AddSkidMark(skidPoint, GroundHit.normal, (wheelSlipAmount_SideWays / 2f) + (wheelSlipAmount_Forward / 2.5f), lastSkidmark);
                else
                    lastSkidmark = -1;

            }

            else
            {

                lastSkidmark = -1;

            }

        }


        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit))
        {

            //if (carController.UseTerrainSplatMapForGroundPhysic && hit.transform.gameObject.GetComponent<TerrainCollider>())
            //{
            //    if (TerrainSurface.GetTextureMix(transform.position)[0] > .5f)
            //        SetWheelStiffnessByGroundPhysic(1f);
            //    else if (TerrainSurface.GetTextureMix(transform.position)[1] > .5f)
            //        SetWheelStiffnessByGroundPhysic(2f);
            //    else if (TerrainSurface.GetTextureMix(transform.position)[2] > .5f)
            //        SetWheelStiffnessByGroundPhysic(3f);
            //    return;
            //}

            //if (hit.collider.material.name == grassPhysicsMaterial.name + " (Instance)")
            //{
            //    SetWheelStiffnessByGroundPhysic(3f);
            //}
            //else if (hit.collider.material.name == sandPhysicsMaterial.name + " (Instance)")
            //{
            //    SetWheelStiffnessByGroundPhysic(2f);
            //}
            
            
                SetWheelStiffnessByGroundPhysic(1f);
            

        }
    }

    public void SetWheelStiffnessByGroundPhysic(float stiffnessDivider)
    {

        forwardFrictionCurve.stiffness = Mathf.Lerp(forwardFrictionCurve.stiffness, defForwardStiffness / stiffnessDivider, Time.deltaTime * 5f);
        sidewaysFrictionCurve.stiffness = Mathf.Lerp(sidewaysFrictionCurve.stiffness, defSideWaysStiffness / stiffnessDivider, Time.deltaTime * 5f);

        wheelCollider.forwardFriction = forwardFrictionCurve;
        wheelCollider.sidewaysFriction = sidewaysFrictionCurve;

    }
}
