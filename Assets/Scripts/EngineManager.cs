using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public abstract class EngineManager : MonoBehaviour
{
    #region Variables
    private WheelCollider[] allWheels;
    private WheelMeshs[] allWheelMeshes;
    private AudioSource[] audioSources;
    private AudioSource engineStartSound, engineSoundOn, engineSoundOff, crashSound, windSound, brakeSound, skidSound;

    [SerializeField]public bool isAiDriver;
    [SerializeField]public bool isON;

    [Header("Input Manager")]
    [SerializeField]public LocalInputManager inputManager;
    [Header("Scriptables Engine_So")]
    [SerializeField]public Engine_SO engine_SO;
    [Header("Scriptables VihecleAudio_So")]
    [SerializeField]public VihecleAudio_SO audio_SO;
    [Header("Engine Parts")]
    [SerializeField]public Rigidbody rigid;
    [SerializeField]public Transform massCenter;
    [Header("Vihecle Utils")]
    [SerializeField]public GameObject chassis, allAudioSources, allWheelsContainer, allWheelsMeshContainer;
    
    #endregion

    #region MonoBehaviours
    private void OnEnable()
    {
        if (inputManager == null)
        {
            inputManager = FindObjectOfType<LocalInputManager>();
        }
        engine_SO.engineRunning = false;
        engine_SO.engineStarting = false;

        if (engine_SO.runEngineAtAwake)
            KillOrStartEngine();
    }
    private void OnDisable()
    {
        engine_SO.engineRunning = false;
        engine_SO.engineStarting = false;
    }
    private void Awake()
    {
       // engine_SO = new Engine_SO();
        //audio_SO = new VihecleAudio_SO();
        if (inputManager == null)
        {
            inputManager = FindObjectOfType<LocalInputManager>();
        }

        AssignWheels();
        AssignAllAudio();

        if (engine_SO.runEngineAtAwake)
            KillOrStartEngine();
    }
    private void Start()
    {
        Time.fixedDeltaTime = .02f;
        rigid.maxAngularVelocity = 5f;
    }
    private void Update()
    {
        if (!isAiDriver)
        {
            UpdateWheelVisuals();
            Chassis();
            PlaySounds();
        }
        if (isON)
        {
            engine_SO.engineRunning = false;
        }
        else
        {
            engine_SO.engineRunning = true;
        }
        ResetCar();
    }
    private void FixedUpdate()
    {
        engine_SO.currentdSpeed = rigid.velocity.magnitude;
        if (engine_SO.engineRunning) 
        { 
            DriveMotor();
            HandleSteering();
        }

        Suspension();

        //Angular Drag Limit Depends On Vehicle Speed.
        rigid.angularDrag = Mathf.Clamp((engine_SO.currentdSpeed / engine_SO.topSpeed), 0f, 1f);

        //Limit front wheels steering.
        engine_SO.MaxSteeringAngel = Mathf.Lerp(40f, 6.0f, (engine_SO.currentdSpeed / 80.0f));
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length < 1 || collision.relativeVelocity.magnitude < 5f)
            return;

        if (audio_SO.crashClips.Length > 0)
        {
            if (collision.contacts[0].thisCollider.gameObject.transform != transform.parent)
            {

                crashSound = CreateAudioSource.NewAudioSource(crashSound, "Crash Sound AudioSource", 5, audio_SO.maxCrashSoundVolume, audio_SO.crashClips[UnityEngine.Random.Range(0, audio_SO.crashClips.Length)], false, true, true);
            }
        }
    }
    #endregion

    #region Engine Utils
    private void AssignWheels()
    {
        allWheels = allWheelsContainer.GetComponentsInChildren<WheelCollider>();
        allWheelMeshes = allWheelsMeshContainer.GetComponentsInChildren<WheelMeshs>();
    }
    private void AssignAllAudio()
    {
        audioSources = allAudioSources.GetComponentsInChildren<AudioSource>();

        engineStartSound = audioSources[0];
        engineSoundOn = audioSources[1];
        engineSoundOff = audioSources[2];
        brakeSound = audioSources[3];
        crashSound = audioSources[4];
        windSound = audioSources[5];
        skidSound = audioSources[6];

        engineSoundOn = CreateAudioSource.NewAudioSource(engineSoundOn, "Engine Sound On AudioSource", 5, 0, audio_SO.engineClipOn, true, true, false);
        engineSoundOff = CreateAudioSource.NewAudioSource(engineSoundOff, "Engine Sound Off AudioSource", 5, 0, audio_SO.engineClipOff, true, true, false);
        skidSound = CreateAudioSource.NewAudioSource(skidSound, "Skid Sound AudioSource", 5, 0, audio_SO.asphaltSkidClip, true, true, false);
        windSound = CreateAudioSource.NewAudioSource(windSound, "Wind Sound AudioSource", 5, 0, audio_SO.windClip, true, true, false);
        brakeSound = CreateAudioSource.NewAudioSource(brakeSound, "Brake Sound AudioSource", 5, 0, audio_SO.brakeClip, true, true, false);
        crashSound = CreateAudioSource.NewAudioSource(crashSound, "Brake Sound AudioSource", 5, 0, audio_SO.crashClips[1], true, true, false);
    }
    #endregion

    #region Utils
    private void ResetCar()
    {
        if (engine_SO.currentdSpeed < 20.0f)
        {
            if (transform.localEulerAngles.z < 300 && transform.localEulerAngles.z > 60)
            {

                engine_SO.resetTime += Time.deltaTime;

                if (engine_SO.resetTime > 3f)
                {
                    transform.rotation = Quaternion.identity;
                    transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                    engine_SO.resetTime = 0f;
                }
            }

            if (transform.localEulerAngles.x < 300 && transform.localEulerAngles.x > 60)
            {

                engine_SO.resetTime += Time.deltaTime;

                if (engine_SO.resetTime > 3f)
                {
                    transform.rotation = Quaternion.identity;
                    transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                    engine_SO.resetTime = 0f;
                }
            }
        }
    }
    private void PlaySounds()
    {
        //Engine Audio Volume.
        if (engineSoundOn)
        {

            if (!engine_SO.reversing)
                engineSoundOn.volume = Mathf.Lerp(engineSoundOn.volume, Mathf.Clamp(inputManager.LocalInput, audio_SO.minEngineSoundVolume, audio_SO.maxEngineSoundVolume), Time.deltaTime * 50f);
            else
                engineSoundOn.volume = Mathf.Lerp(engineSoundOn.volume, Mathf.Clamp((inputManager.LocalInput), audio_SO.minEngineSoundVolume, audio_SO.maxEngineSoundVolume), Time.deltaTime * 50f);

            if (engine_SO.engineRunning)
                engineSoundOn.pitch = Mathf.Lerp(engineSoundOn.pitch, Mathf.Lerp(audio_SO.minEngineSoundPitch, audio_SO.maxEngineSoundPitch, (800f) / (8000f)), Time.deltaTime * 50f);
            else
                engineSoundOn.pitch = Mathf.Lerp(engineSoundOn.pitch, 0, Time.deltaTime * 50f);

        }

        if (engineSoundOff)
        {

            if (!engine_SO.reversing)
                engineSoundOff.volume = Mathf.Lerp(engineSoundOff.volume, Mathf.Clamp((1 + (inputManager.LocalInput)), audio_SO.minEngineSoundVolume, .5f), Time.deltaTime * 50f);
            else
                engineSoundOff.volume = Mathf.Lerp(engineSoundOff.volume, Mathf.Clamp((1 + (inputManager.LocalInput)), audio_SO.minEngineSoundVolume, .5f), Time.deltaTime * 50f);

            if (engine_SO.engineRunning)
                engineSoundOff.pitch = Mathf.Lerp(engineSoundOff.pitch, Mathf.Lerp(audio_SO.minEngineSoundPitch, audio_SO.maxEngineSoundPitch, (800f) / (8000f)), Time.deltaTime * 50f);
            else
                engineSoundOff.pitch = Mathf.Lerp(engineSoundOff.pitch, 0, Time.deltaTime * 50f);

        }

        windSound.volume = Mathf.Lerp(0f, audio_SO.maxWindSoundVolume, engine_SO.currentdSpeed / 220f);
        windSound.pitch = UnityEngine.Random.Range(.9f, 1.1f) * 1f;

        if (engine_SO.IsBraking && engine_SO.currentdSpeed > 2f)
        {
            //if (wheelDifination is FWD)
            //{
            //    wheelDifination.brakeSound.volume = Mathf.Lerp(1f, wheelDifination.maxBrakeSoundVolume, Mathf.Clamp01(wheelDifination.brakForce) * Mathf.Lerp(0f, 1f, ((FWD)wheelDifination).front_R_wheelColliderFWD.rpm / 50f));
            //}
            //else if (wheelDifination is TWD)
            //{
            //    brakeSound.volume = Mathf.Lerp(1f, audio_SO.maxBrakeSoundVolume, Mathf.Clamp01(engine_SO.brakForce) * Mathf.Lerp(0f, 1f, ((TWD)wheelDifination).front_wheelColliderTWD.rpm / 50f));
            //}


        }
        else { brakeSound.volume = 0f; }


    }
    public void Chassis()
    {
        if (allWheels.Length > 3)//4
        {
            rigid.centerOfMass = new Vector3((massCenter.localPosition.x) * transform.localScale.x, (Mathf.Lerp(massCenter.localPosition.y, allWheels[0].transform.localPosition.y, engine_SO.currentdSpeed / 120f)) * transform.localScale.y, (massCenter.localPosition.z) * transform.localScale.z);

            engine_SO.verticalLean = Mathf.Clamp(Mathf.Lerp(engine_SO.verticalLean, rigid.angularVelocity.x * engine_SO.chassisVerticalLean, Time.deltaTime * 3f), -3.0f, 3.0f);

            WheelHit CorrespondingGroundHit;
            allWheels[1].GetGroundHit(out CorrespondingGroundHit);

            float normalizedLeanAngle = Mathf.Clamp(CorrespondingGroundHit.sidewaysSlip, -1f, 1f);

            if (normalizedLeanAngle > 0f)
                normalizedLeanAngle = 1;
            else
                normalizedLeanAngle = -1;

            if (transform.InverseTransformDirection(rigid.velocity).z >= 0)
                engine_SO.horizontalLean = Mathf.Clamp(Mathf.Lerp(engine_SO.horizontalLean, (transform.InverseTransformDirection(rigid.angularVelocity).y) * engine_SO.chassisHorizontalLean, Time.deltaTime * 3f), -3f, 3f);
            else
                engine_SO.horizontalLean = Mathf.Clamp(Mathf.Lerp(engine_SO.horizontalLean, (Mathf.Abs(transform.InverseTransformDirection(rigid.angularVelocity).y) * -normalizedLeanAngle) * engine_SO.chassisHorizontalLean, Time.deltaTime * 3f), -3.0f, 3.0f);

            if (float.IsNaN(engine_SO.verticalLean) || float.IsNaN(engine_SO.horizontalLean) || float.IsInfinity(engine_SO.verticalLean) || float.IsInfinity(engine_SO.horizontalLean) || Mathf.Approximately(engine_SO.verticalLean, 0f) || Mathf.Approximately(engine_SO.horizontalLean, 0f))
                return;

            Quaternion target = Quaternion.Euler(engine_SO.verticalLean, chassis.transform.localRotation.y + (rigid.angularVelocity.z), engine_SO.horizontalLean);
            chassis.transform.localRotation = target;
        }
        else //3
        {
            rigid.centerOfMass = new Vector3((massCenter.localPosition.x) * transform.localScale.x, (Mathf.Lerp(massCenter.localPosition.y, allWheels[0].transform.localPosition.y, engine_SO.currentdSpeed / 120f)) * transform.localScale.y, (massCenter.localPosition.z) * transform.localScale.z);

            engine_SO.verticalLean = Mathf.Clamp(Mathf.Lerp(engine_SO.verticalLean, rigid.angularVelocity.x * engine_SO.chassisVerticalLean, Time.deltaTime * 3f), -3.0f, 3.0f);

            WheelHit CorrespondingGroundHit;
            allWheels[0].GetGroundHit(out CorrespondingGroundHit);

            float normalizedLeanAngle = Mathf.Clamp(CorrespondingGroundHit.sidewaysSlip, -1f, 1f);

            if (normalizedLeanAngle > 0f)
                normalizedLeanAngle = 1;
            else
                normalizedLeanAngle = -1;

            if (transform.InverseTransformDirection(rigid.velocity).z >= 0)
                engine_SO.horizontalLean = Mathf.Clamp(Mathf.Lerp(engine_SO.horizontalLean, (transform.InverseTransformDirection(rigid.angularVelocity).y) * engine_SO.chassisHorizontalLean, Time.deltaTime * 3f), -3f, 3f);
            else
                engine_SO.horizontalLean = Mathf.Clamp(Mathf.Lerp(engine_SO.horizontalLean, (Mathf.Abs(transform.InverseTransformDirection(rigid.angularVelocity).y) * -normalizedLeanAngle) * engine_SO.chassisHorizontalLean, Time.deltaTime * 3f), -3.0f, 3.0f);

            if (float.IsNaN(engine_SO.verticalLean) || float.IsNaN(engine_SO.horizontalLean) || float.IsInfinity(engine_SO.verticalLean) || float.IsInfinity(engine_SO.horizontalLean) || Mathf.Approximately(engine_SO.verticalLean, 0f) || Mathf.Approximately(engine_SO.horizontalLean, 0f))
                return;

            Quaternion target = Quaternion.Euler(engine_SO.verticalLean, chassis.transform.localRotation.y + (rigid.angularVelocity.z), engine_SO.horizontalLean);
            chassis.transform.localRotation = target;
        }
    }
    private void UpdateWheelVisuals()
    {
        if (allWheels.Length > 3)
        {
            updatVisuals(allWheels[0], allWheelMeshes[0].transform);
            updatVisuals(allWheels[1], allWheelMeshes[1].transform);
            updatVisuals(allWheels[2], allWheelMeshes[2].transform);
            updatVisuals(allWheels[3], allWheelMeshes[3].transform);
        }
        else
        {
            updatVisuals(allWheels[0], allWheelMeshes[0].transform);
            updatVisuals(allWheels[1], allWheelMeshes[1].transform);
            updatVisuals(allWheels[2], allWheelMeshes[2].transform);
        }
    }
    private void updatVisuals(WheelCollider wheelCollider, Transform transformwheel)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        transformwheel.rotation = rot;

        transformwheel.position = pos;
    }
    public void Suspension()
    {
        if (allWheels.Length > 3)
        {
            WheelHit FrontWheelHit;

            float travelFL = 1.0f;
            float travelFR = 1.0f;

            bool groundedFR = allWheels[0].GetGroundHit(out FrontWheelHit);

            if (groundedFR)
                travelFR = (-allWheels[0].transform.InverseTransformPoint(FrontWheelHit.point).y - allWheels[0].radius) / allWheels[0].suspensionDistance;

            float antiRollForceFront = (travelFL - travelFR) * engine_SO.antiRoll;


            if (groundedFR)
                rigid.AddForceAtPosition(allWheels[0].transform.up * antiRollForceFront, allWheels[0].transform.position);

            //front left
            bool groundedFL = allWheels[1].GetGroundHit(out FrontWheelHit);

            if (groundedFL)
                travelFR = (-allWheels[1].transform.InverseTransformPoint(FrontWheelHit.point).y - allWheels[1].radius) / allWheels[1].suspensionDistance;

            float antiRollForceFrontLeft = (travelFL - travelFR) * engine_SO.antiRoll;


            if (groundedFR)
                rigid.AddForceAtPosition(allWheels[1].transform.up * antiRollForceFrontLeft, allWheels[1].transform.position);

            WheelHit RearWheelHit;

            float travelRL = 1.0f;
            float travelRR = 1.0f;

            bool groundedRL = allWheels[2].GetGroundHit(out RearWheelHit);

            if (groundedRL)
                travelRL = (-allWheels[2].transform.InverseTransformPoint(RearWheelHit.point).y - allWheels[2].radius) / allWheels[2].suspensionDistance;

            bool groundedRR = allWheels[3].GetGroundHit(out RearWheelHit);

            if (groundedRR)
                travelRR = (-allWheels[3].transform.InverseTransformPoint(RearWheelHit.point).y - allWheels[3].radius) / allWheels[3].suspensionDistance;

            float antiRollForceRear = (travelRL - travelRR) * engine_SO.antiRoll;

            if (groundedRL)
                rigid.AddForceAtPosition(allWheels[2].transform.up * -antiRollForceRear, allWheels[2].transform.position);
            if (groundedRR)
                rigid.AddForceAtPosition(allWheels[3].transform.up * antiRollForceRear, allWheels[3].transform.position);

            if (groundedRR && groundedRL)
            {
                rigid.AddRelativeTorque((Vector3.up * (inputManager.LocalInputSteer * inputManager.LocalInput)) * 2000f);
            }
        }
        else
        {
            WheelHit FrontWheelHit;

            float travelFL = 1.0f;
            float travelFR = 1.0f;

            bool groundedFR = allWheels[0].GetGroundHit(out FrontWheelHit);

            if (groundedFR)
                travelFR = (-allWheels[0].transform.InverseTransformPoint(FrontWheelHit.point).y - allWheels[0].radius) / allWheels[0].suspensionDistance;

            float antiRollForceFront = (travelFL - travelFR) * engine_SO.antiRoll;

            if (groundedFR)
                rigid.AddForceAtPosition(allWheels[0].transform.up * antiRollForceFront, allWheels[0].transform.position);

            WheelHit RearWheelHit;

            float travelRL = 1.0f;
            float travelRR = 1.0f;

            bool groundedRL = allWheels[1].GetGroundHit(out RearWheelHit);

            if (groundedRL)
                travelRL = (-allWheels[1].transform.InverseTransformPoint(RearWheelHit.point).y - allWheels[1].radius) / allWheels[1].suspensionDistance;

            bool groundedRR = allWheels[2].GetGroundHit(out RearWheelHit);

            if (groundedRR)
                travelRR = (-allWheels[2].transform.InverseTransformPoint(RearWheelHit.point).y - allWheels[2].radius) / allWheels[2].suspensionDistance;

            float antiRollForceRear = (travelRL - travelRR) * engine_SO.antiRoll;

            if (groundedRL)
                rigid.AddForceAtPosition(allWheels[1].transform.up * -antiRollForceRear, allWheels[1].transform.position);
            if (groundedRR)
                rigid.AddForceAtPosition(allWheels[2].transform.up * antiRollForceRear, allWheels[2].transform.position);

            if (groundedRR && groundedRL)
            {
                rigid.AddRelativeTorque((Vector3.up * (inputManager.LocalInputSteer * inputManager.LocalInput)) * 2000f);
            }
        }
    }
    #endregion

    #region Engine
    IEnumerator StartEngine()
    {
        engine_SO.engineRunning = false;
        engine_SO.engineStarting = true;
        if (!engine_SO.engineRunning)
            engineStartSound = CreateAudioSource.NewAudioSource(engineStartSound, "Engine Start AudioSource", 5, 1, audio_SO.engineStartClip, false, true, true);
        yield return new WaitForSeconds(1f);
        engine_SO.engineRunning = true;
        yield return new WaitForSeconds(1f);
        engine_SO.engineStarting = false;
    }
    public void KillOrStartEngine()
    {
        if (engine_SO.engineRunning && !engine_SO.engineStarting)
        {
            engine_SO.engineRunning = false;
        }
        else if (!engine_SO.engineStarting)
        {
            StartCoroutine(StartEngine());
        }
    }
    private void HandleSteering()
    {
        if (allWheels.Length > 3)
        {
            allWheels[0].steerAngle = Mathf.Clamp((engine_SO.MaxSteeringAngel * inputManager.LocalInputSteer), -engine_SO.MaxSteeringAngel, engine_SO.MaxSteeringAngel);
            allWheels[1].steerAngle = Mathf.Clamp((engine_SO.MaxSteeringAngel * inputManager.LocalInputSteer), -engine_SO.MaxSteeringAngel, engine_SO.MaxSteeringAngel);
        }
        else
        {
            allWheels[0].steerAngle = Mathf.Clamp((engine_SO.MaxSteeringAngel * inputManager.LocalInputSteer), -engine_SO.MaxSteeringAngel, engine_SO.MaxSteeringAngel);
        }
    }
    void DriveMotor()
    {
        engine_SO.currentBreakForce = inputManager.Braking ? engine_SO.brakForce : 0f;
        engine_SO.IsBraking = inputManager.Braking;

        if (allWheels.Length > 3)
        {
            if (allWheels[2].transform.localPosition.z < 0 && allWheels[3].transform.localPosition.z < 0)
            {
                Brake(engine_SO.currentBreakForce);
            }
            if (allWheels[2].transform.localPosition.z < 0 && allWheels[3].transform.localPosition.z < 0)
            {
                Accelerate();
            }
            if (allWheels[2].transform.localPosition.z >= 0 && allWheels[3].transform.localPosition.z >= 0)
            {
                Accelerate();
            }
            //Reversing Bool.
            if (engine_SO.currentBreakForce > .1f && transform.InverseTransformDirection(rigid.velocity).z < 1f)
                engine_SO.reversing = true;
            else
                engine_SO.reversing = false;
        }
        else
        {
            engine_SO.currentBreakForce = inputManager.Braking ? engine_SO.brakForce : 0f;

            if (allWheels[1].transform.localPosition.z < 0 && allWheels[2].transform.localPosition.z < 0)
            {
                Brake(engine_SO.currentBreakForce);
            }
            if (allWheels[1].transform.localPosition.z < 0 && allWheels[2].transform.localPosition.z < 0)
            {
                Accelerate();
            }
            if (allWheels[1].transform.localPosition.z >= 0 && allWheels[2].transform.localPosition.z >= 0)
            {
                Accelerate();
            }
            //Reversing Bool.
            if (engine_SO.currentBreakForce > .1f && transform.InverseTransformDirection(rigid.velocity).z < 1f)
                engine_SO.reversing = true;
            else
                engine_SO.reversing = false;
        }
    }
    public void Accelerate()
    {
        if (allWheels.Length > 3)
        {
            if (engine_SO.currentdSpeed < engine_SO.topSpeed)
            {
                allWheels[3].motorTorque = engine_SO.motorForce * inputManager.Acceleration;
                allWheels[2].motorTorque = engine_SO.motorForce * inputManager.Acceleration;
            }
            else
            {
                allWheels[0].motorTorque = 0;
                allWheels[1].motorTorque = 0;

                allWheels[2].motorTorque = 0;
                allWheels[3].motorTorque = 0;
            }
        }
        else
        {
            if (engine_SO.currentdSpeed < engine_SO.topSpeed)
            {
                allWheels[2].motorTorque = engine_SO.motorForce * inputManager.Acceleration;
                allWheels[1].motorTorque = engine_SO.motorForce * inputManager.Acceleration;
            }
            else
            {
                allWheels[0].motorTorque = 0;
                allWheels[1].motorTorque = 0;

                allWheels[2].motorTorque = 0;
            }
        }
    }
    private void Brake(float brForce)
    {
        if (allWheels.Length > 3)
        {
            allWheels[0].brakeTorque = brForce;
            allWheels[1].brakeTorque = brForce;
            allWheels[2].brakeTorque = brForce;
            allWheels[3].brakeTorque = brForce;
        }
        else
        {
            allWheels[0].brakeTorque = brForce;
            allWheels[1].brakeTorque = brForce;
            allWheels[2].brakeTorque = brForce;
        }
    }
    #endregion

    #region constructors for VehiclesStats
    public float _GetTopSpeed
    {
        get { return engine_SO.GetTopSpeed; }
    }
    public float _GetHandling
    {
        get { return engine_SO.GetMotorForce; }
    }
    public float _GetBraking
    {
        get { return engine_SO.GetBraking; }
    }
    public string _GetName
    {
        get { return engine_SO.GetName; }
    }
    public float _GetCurrentTopSpeed
    {
        get { return engine_SO.currentdSpeed; }
    }
    #endregion
}
