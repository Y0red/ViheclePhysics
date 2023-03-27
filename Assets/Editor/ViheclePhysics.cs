using System;
using UnityEditor;
using UnityEngine;
[ExecuteInEditMode]
public class ViheclePhysics : EditorWindow
{
     public enum Types
    {
        TWD,
        FWD,
    }
    Types currentType;
    #region Menu Item
    [MenuItem("VPCreator/Create New ViheclePhysics")]
    public static void ShowWindow()
    {
        GetWindow<ViheclePhysics>("Example Window");
    }
    #endregion
    #region Variables
    GameObject carMeshPrifab, wheelsMeshPrifabL, wheelsMeshPrifabR, containerObject;
    public GameObject[] carWheelPrifab;
    Color[] wheelsMatColor, bodyMatColors;

    string nameV = "New Vihecle";
    string text, carMeshPrifabErrorText, carWheelsPrifabErrorText;
    int wheelsConunt = 3;
    bool isCreated = false;

    Vector3 frontWheelPos = new Vector3(0f, -0.464f, .99f);
    Vector3 frontLeftWheelPos = new Vector3(0f, -0.464f, 2f);
    Vector3 frontRightWheelPos = new Vector3(0f, -0.464f, 2f);
    Vector3 backLeftWheelPos = new Vector3(-0.506f, -0.464f, -1.64f);
    Vector3 backRightWheelPos = new Vector3(1.157f, -0.464f, -1.61f);

    float wheelsPosMax = -2f;
    float wheelsPosMin = 2f;
    #endregion
    #region GUI
    private void OnGUI()
    {
        GUILayout.Label("Realistic Physics Vihecle Creator Wizard", EditorStyles.whiteBoldLabel);

        if (!isCreated)
        {
            carMeshPrifab = EditorGUILayout.ObjectField("Vihecle Body", carMeshPrifab, typeof(GameObject), true) as GameObject;
            GUILayout.Label(carMeshPrifabErrorText, EditorStyles.whiteBoldLabel);
            if (carMeshPrifab == null) EditorGUILayout.HelpBox($"This field needs to be selected", MessageType.Warning);
            //wheelsConunt = EditorGUILayout.IntSlider(wheelsConunt, 3, 4);
            currentType = (Types)EditorGUILayout.EnumPopup(currentType);
            Debug.Log(currentType);
            if (currentType == Types.FWD) wheelsConunt = 4;
            if (currentType == Types.TWD) wheelsConunt = 3;
            carWheelPrifab = new GameObject[wheelsConunt];
            SetUpWheelBox();
            GUILayout.Label(carWheelsPrifabErrorText, EditorStyles.whiteBoldLabel);
            if (wheelsMeshPrifabL == null) EditorGUILayout.HelpBox($"This field needs to be selected", MessageType.Warning);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Vihecle", GUILayout.Height(20), GUILayout.Width(40)))
            {
                if (carMeshPrifab != null && wheelsMeshPrifabL != null)
                {
                    Create();
                }
            }
            GUILayout.EndHorizontal();
        }
        if (isCreated)
        {
           if(wheelsConunt == 3)
           {
                SetUpThreeWheelsSlider();
           }
           else
           {
                SetUpFourWheelsSlider();
           }
            containerObject = EditorGUILayout.ObjectField("Vihecle Body", containerObject, typeof(GameObject), true) as GameObject;

            GUILayout.Label("Body Material Colors", EditorStyles.whiteBoldLabel);
            for (int i = 0; i < bodyMatColors.Length; i++)
            {
                bodyMatColors[i] = EditorGUILayout.ColorField($"Body Mats{i}", bodyMatColors[i]);
            }

            GUILayout.Label("Wheels Material Colors", EditorStyles.whiteBoldLabel);
            for (int i = 0; i < wheelsMatColor.Length; i++)
            {
                wheelsMatColor[i] = EditorGUILayout.ColorField($"Mats{i}", wheelsMatColor[i]);
            }

        }
    }
    private void SetUpWheelBox()
    {
        GUILayout.BeginHorizontal();
        wheelsMeshPrifabL = EditorGUILayout.ObjectField("Wheel Mesh Left", wheelsMeshPrifabL, typeof(GameObject), true) as GameObject;
        wheelsMeshPrifabR = EditorGUILayout.ObjectField("Wheel Mesh Right", wheelsMeshPrifabR, typeof(GameObject), true) as GameObject;
        GUILayout.EndHorizontal();
        //for (int i = 0; i < wheelsConunt; i++)
        //{
        //    carWheelPrifab[i] = wheelsMeshPrifabL;
       // }
        if(currentType == Types.TWD)
        {
            carWheelPrifab[0] = wheelsMeshPrifabL;
            carWheelPrifab[1] = wheelsMeshPrifabR;
            carWheelPrifab[2] = wheelsMeshPrifabL;
        }
        if(currentType == Types.FWD)
        {
            carWheelPrifab[0] = wheelsMeshPrifabL;
            carWheelPrifab[1] = wheelsMeshPrifabR;
            carWheelPrifab[2] = wheelsMeshPrifabR;
            carWheelPrifab[3] = wheelsMeshPrifabL;
        }
    }
    private void Create()
    {
        #region Validation
        if (carMeshPrifab == null)
        {
            carMeshPrifabErrorText = "Select Vihecl Body Mesh";
            return;
        }
        else
        {
            carMeshPrifabErrorText = String.Empty;
        }
        if (wheelsConunt == 3)
        {
            if (carWheelPrifab[0] == null && carWheelPrifab[1] == null && carWheelPrifab[2] == null)
            {
                carWheelsPrifabErrorText = "Select Wheel Meshs";
                return;
            }
        }
        else
        {
            if (carWheelPrifab[0] == null && carWheelPrifab[1] == null && carWheelPrifab[2] == null && carWheelPrifab[2] == null)
            {
                carWheelsPrifabErrorText = "Select Wheel Meshs";
                return;
            }
        }
        carWheelsPrifabErrorText = String.Empty;
        carMeshPrifabErrorText = String.Empty;
        #endregion
        #region create containers
        GameObject container = new GameObject(nameV);
        container.transform.position = new Vector3(0f, 0f , 0f);

        container.AddComponent<Rigidbody>().mass = 800f;
        container.AddComponent<BoxCollider>();
        container.AddComponent<LocalInputManager>();
        container.AddComponent<CustomVihecle>();
        var customViheclec = container.GetComponent<CustomVihecle>();
        
        GameObject newMesh = Instantiate(carMeshPrifab, container.transform);
       // newMesh.AddComponent<MeshCollider>();newMesh.GetComponent<MeshCollider>().convex = true;

        GameObject COM = new GameObject("COM");
        COM.transform.SetParent(container.transform);
        COM.transform.position = new Vector3(0f, 0f, 0f);
        COM.AddComponent<COM>();
        COM.AddComponent<Chassis>();

        GameObject containerColider = new GameObject("Wheels Colider Container");
        containerColider.transform.SetParent(container.transform);
        containerColider.transform.position = new Vector3(0f, 0f, 0f);
        GameObject containerWheelMesh = new GameObject("Mesh Wheels Container");
        containerWheelMesh.transform.SetParent(container.transform);
        containerWheelMesh.transform.position = new Vector3(0f, 0f, 0f);
        containerObject = container;
        #endregion
        #region Sound Manager Stuf
        GameObject containerSoundManager = new GameObject("Sound Manager");
        containerSoundManager.transform.position = new Vector3(0f, 0f, 0f);
        containerSoundManager.transform.SetParent(container.transform);

        GameObject SoundManager1 = new GameObject("Engine Start AudioSource");
        SoundManager1.transform.SetParent(containerSoundManager.transform);
        SoundManager1.AddComponent<AudioSource>();
        GameObject SoundManager2 = new GameObject("Engine Sound On AudioSource"); 
        SoundManager2.transform.SetParent(containerSoundManager.transform);
        SoundManager2.AddComponent<AudioSource>();
        GameObject SoundManager3 = new GameObject("Engine Sound Off AudioSource"); 
        SoundManager3.transform.SetParent(containerSoundManager.transform);
        SoundManager3.AddComponent<AudioSource>();
        GameObject SoundManager4 = new GameObject("Skid Sound AudioSource"); 
        SoundManager4.transform.SetParent(containerSoundManager.transform);
        SoundManager4.AddComponent<AudioSource>();
        GameObject SoundManager5 = new GameObject("Wind Sound AudioSource"); 
        SoundManager5.transform.SetParent(containerSoundManager.transform);
        SoundManager5.AddComponent<AudioSource>();
        GameObject SoundManager6 = new GameObject("Brake Sound AudioSource"); 
        SoundManager6.transform.SetParent(containerSoundManager.transform);
        SoundManager6.AddComponent<AudioSource>();
        GameObject SoundManager7 = new GameObject("Crash Sound AudioSource"); 
        SoundManager7.transform.SetParent(containerSoundManager.transform);
        SoundManager7.AddComponent<AudioSource>();
        #endregion
        #region assign stufs for custom Vihecle clsas
        customViheclec.inputManager = container.GetComponent<LocalInputManager>();
        customViheclec.engine_SO = ScriptableObject.CreateInstance<Engine_SO>();//new Engine_SO();
        customViheclec.audio_SO = new VihecleAudio_SO();
        customViheclec.rigid = container.GetComponent<Rigidbody>();
        customViheclec.massCenter = COM.transform;
        customViheclec.chassis = COM;
        customViheclec.allAudioSources = containerSoundManager;
        customViheclec.allWheelsContainer = containerColider;
        customViheclec.allWheelsMeshContainer = containerWheelMesh;
        #endregion
        #region create wheel coliders and wheel meshes
        if (carWheelPrifab.Length == 3)
        {
            carWheelPrifab[0] = Instantiate(carWheelPrifab[0], containerWheelMesh.transform);
            carWheelPrifab[1] = Instantiate(carWheelPrifab[1], containerWheelMesh.transform);
            carWheelPrifab[2] = Instantiate(carWheelPrifab[2], containerWheelMesh.transform);

            GameObject frontColl = new GameObject("Front Collider");
            frontColl.transform.SetParent(containerColider.transform);
            GameObject backLColl = new GameObject("Back Left Collider");
            backLColl.transform.SetParent(containerColider.transform);
            GameObject backRColl = new GameObject("Back Right Collider");
            backRColl.transform.SetParent(containerColider.transform);

            frontColl.AddComponent<WheelCollider>().radius = .35f;
            backLColl.AddComponent<WheelCollider>().radius = .35f;
            backRColl.AddComponent<WheelCollider>().radius = .35f; 

            carWheelPrifab[0].AddComponent<WheelMeshs>();
            carWheelPrifab[1].AddComponent<WheelMeshs>();
            carWheelPrifab[2].AddComponent<WheelMeshs>();

            carWheelPrifab[0].name = "front"; carWheelPrifab[2].name = "backR"; carWheelPrifab[1].name = "backL";

            Renderer rendererWheels = carWheelPrifab[0].GetComponent<Renderer>();
            Renderer rendererBody = containerObject.transform.GetChild(0).GetComponentInChildren<Renderer>();

            if (rendererWheels != null)
            {
                wheelsMatColor = new Color[rendererWheels.sharedMaterials.Length];

                for (int i = 0; i < rendererWheels.sharedMaterials.Length; i++)
                {
                    wheelsMatColor[i] = rendererWheels.sharedMaterials[i].color;
                }
            }
            if (rendererBody != null)
            {
                bodyMatColors = new Color[rendererBody.sharedMaterials.Length];
                for (int i = 0; i < rendererBody.sharedMaterials.Length; i++)
                {
                    bodyMatColors[i] = rendererBody.sharedMaterials[i].color;
                }
            }

        }
        else
        {
            carWheelPrifab[0] = Instantiate(carWheelPrifab[0], containerWheelMesh.transform);
            carWheelPrifab[1] = Instantiate(carWheelPrifab[1], containerWheelMesh.transform);
            carWheelPrifab[2] = Instantiate(carWheelPrifab[2], containerWheelMesh.transform);
            carWheelPrifab[3] = Instantiate(carWheelPrifab[3], containerWheelMesh.transform);

            GameObject frontLColl = new GameObject("Front Left Collider");
            frontLColl.transform.SetParent(containerColider.transform);
            GameObject frontRColl = new GameObject("Front Right Collider");
            frontRColl.transform.SetParent(containerColider.transform);
            GameObject backLColl = new GameObject("Back Left Collider");
            backLColl.transform.SetParent(containerColider.transform);
            GameObject backRColl = new GameObject("Back Right Collider");
            backRColl.transform.SetParent(containerColider.transform);

            frontLColl.AddComponent<WheelCollider>().radius = .35f;
            frontRColl.AddComponent<WheelCollider>().radius = .35f;
            backLColl.AddComponent<WheelCollider>().radius = .35f;
            backRColl.AddComponent<WheelCollider>().radius = .35f;

            carWheelPrifab[0].AddComponent<WheelMeshs>();
            carWheelPrifab[1].AddComponent<WheelMeshs>();
            carWheelPrifab[2].AddComponent<WheelMeshs>();
            carWheelPrifab[3].AddComponent<WheelMeshs>();

            carWheelPrifab[0].name = "frontR"; carWheelPrifab[1].name = "frontL"; carWheelPrifab[3].name = "backR"; carWheelPrifab[2].name = "backL";


            Renderer rr = carWheelPrifab[0].GetComponent<Renderer>();

            Renderer rendererWheels = carWheelPrifab[0].GetComponent<Renderer>();
            Renderer rendererBody = containerObject.transform.GetChild(0).GetComponentInChildren<Renderer>();

            if (rendererWheels != null)
            {
                wheelsMatColor = new Color[rendererWheels.sharedMaterials.Length];

                for (int i = 0; i < rendererWheels.sharedMaterials.Length; i++)
                {
                    wheelsMatColor[i] = rendererWheels.sharedMaterials[i].color;
                }
            }
            if (rendererBody != null)
            {
                bodyMatColors = new Color[rendererBody.sharedMaterials.Length];
                for (int i = 0; i < rendererBody.sharedMaterials.Length; i++)
                {
                    bodyMatColors[i] = rendererBody.sharedMaterials[i].color;
                }
            }

        }
        #endregion
        isCreated = true;
    }
    void SetUpThreeWheelsSlider()
    {
        GUILayout.Label("All Wheel Y Position", EditorStyles.boldLabel);
        frontWheelPos.y = EditorGUILayout.Slider(frontWheelPos.y, wheelsPosMin, wheelsPosMax);
        backLeftWheelPos.y = frontWheelPos.y;
        backRightWheelPos.y = frontWheelPos.y;

        GUILayout.Label("Front Wheel", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Wheel Position X Axis", EditorStyles.whiteBoldLabel);
        GUILayout.Label("Wheel Position Z Axis", EditorStyles.whiteBoldLabel);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        frontWheelPos.x = EditorGUILayout.Slider(frontWheelPos.x, wheelsPosMin, wheelsPosMax);
        frontWheelPos.z = EditorGUILayout.Slider(frontWheelPos.z, wheelsPosMin, wheelsPosMax);
        GUILayout.EndHorizontal();
        GUILayout.Label("Back Wheel", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Wheel Position X Axis", EditorStyles.whiteBoldLabel);
        GUILayout.Label("Wheel Position Z Axis", EditorStyles.whiteBoldLabel);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        backLeftWheelPos.x = EditorGUILayout.Slider(backLeftWheelPos.x, wheelsPosMin, wheelsPosMax);
        backRightWheelPos.x = -backLeftWheelPos.x;

        backLeftWheelPos.z = EditorGUILayout.Slider(backLeftWheelPos.z, wheelsPosMin, wheelsPosMax);
        backRightWheelPos.z = backLeftWheelPos.z;
        GUILayout.EndHorizontal();
    }
    void SetUpFourWheelsSlider()
    {
        GUILayout.Label("All Wheel Y Position", EditorStyles.boldLabel);
        frontLeftWheelPos.y = EditorGUILayout.Slider(frontLeftWheelPos.y, wheelsPosMin, wheelsPosMax);
        frontRightWheelPos.y = frontLeftWheelPos.y;
        backLeftWheelPos.y = frontLeftWheelPos.y;
        backRightWheelPos.y = frontLeftWheelPos.y;

        GUILayout.Label("Front Wheel", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Front Wheels Position X Axis", EditorStyles.whiteBoldLabel);
        GUILayout.Label("Front Wheels Position Z Axis", EditorStyles.whiteBoldLabel);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        frontLeftWheelPos.x = EditorGUILayout.Slider(frontLeftWheelPos.x, wheelsPosMin, wheelsPosMax);
        frontRightWheelPos.x = -frontLeftWheelPos.x;

        frontLeftWheelPos.z = EditorGUILayout.Slider(frontLeftWheelPos.z, wheelsPosMin, wheelsPosMax);
        frontRightWheelPos.z = frontLeftWheelPos.z;
        GUILayout.EndHorizontal();


        GUILayout.Label("Back Wheels", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Front Wheels Position X Axis", EditorStyles.whiteBoldLabel);
        GUILayout.Label("Front Wheels Position Z Axis", EditorStyles.whiteBoldLabel);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        backLeftWheelPos.x = EditorGUILayout.Slider(backLeftWheelPos.x, wheelsPosMin, wheelsPosMax);
        backRightWheelPos.x = -backLeftWheelPos.x;

        backLeftWheelPos.z = EditorGUILayout.Slider(backLeftWheelPos.z, wheelsPosMin, wheelsPosMax);
        backRightWheelPos.z = backLeftWheelPos.z;
        GUILayout.EndHorizontal();

    }
    private Texture GetTexture()
    {
        // var prefab = EditorUtility.CreateEmptyPrefab("Assets/" + go.name + ".prefab");
        // EditorUtility.ReplacePrefab(go, prefab);
        //AssetDatabase.Refresh();

        //var textures = Resources.FindObjectsOfTypeAll(typeof(Texture))
        // .Where(t => t.name.ToLower().Contains("StoreIcon"))
        //  .Cast<Texture>().ToList();
        var textures = Resources.Load<Texture>("StoreIcon");
        return textures;
    }
    #endregion
    private void Update()
    {
        if (isCreated)
        {
            WheelsPositionAdjuster();
        }
    }
    void WheelsPositionAdjuster()
    {
        WheelMeshs[] wm = containerObject.GetComponentsInChildren<WheelMeshs>();
        WheelCollider[] wc = containerObject.GetComponentsInChildren<WheelCollider>();
        Renderer wheelsRenderer = wm[0].GetComponent<Renderer>();
        Renderer bodyRenderer = containerObject.transform.GetChild(0).GetComponentInChildren<Renderer>();

        if (wheelsRenderer != null)
        {
           // color = new Color[rr.sharedMaterials.Length];
            for (int i = 0; i < wheelsRenderer.sharedMaterials.Length; i++)
            {
               // color[i] = rr.sharedMaterials[i].color;
                wheelsRenderer.sharedMaterials[i].color = wheelsMatColor[i];
            }
        }
        if (bodyRenderer != null)
        {
            // color = new Color[rr.sharedMaterials.Length];
            for (int i = 0; i < bodyRenderer.sharedMaterials.Length; i++)
            {
                // color[i] = rr.sharedMaterials[i].color;
                bodyRenderer.sharedMaterials[i].color = bodyMatColors[i];
            }
        }

        if (wheelsConunt == 3)
        {
            Transform frontWheel = wm[0].transform;
            Transform backRightWheel = wm[2].transform;
            Transform backLeftWheel = wm[1].transform;

            frontWheel.position = frontWheelPos;
            backLeftWheel.position = backLeftWheelPos;
            backRightWheel.position = backRightWheelPos;

            Transform frontColl = wc[0].transform; 
            Transform backLColl = wc[2].transform; 
            Transform backRColl = wc[1].transform; 

            frontColl.position = frontWheelPos;
            backLColl.position = backLeftWheelPos;
            backRColl.position = backRightWheelPos;
        }
        else
        {
            Transform frontRightWheel = wm[0].transform;
            Transform frontLeftWheel = wm[1].transform;
            Transform backRightWheel = wm[3].transform;
            Transform backLeftWheel = wm[2].transform;

            frontRightWheel.position = frontRightWheelPos;
            frontLeftWheel.position = frontLeftWheelPos;
            backLeftWheel.position = backLeftWheelPos;
            backRightWheel.position = backRightWheelPos;

            Transform frontLColl = wc[0].transform;
            Transform frontRColl = wc[1].transform;
            Transform backLColl = wc[3].transform;
            Transform backRColl = wc[2].transform;

            frontLColl.position = frontLeftWheelPos;
            frontRColl.position = frontRightWheelPos;
            backLColl.position = backLeftWheelPos;
            backRColl.position = backRightWheelPos;
        }
    }
}

