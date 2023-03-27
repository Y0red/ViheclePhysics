using System;
using UnityEditor;
using UnityEngine;

public class EditorScreenCapture : EditorWindow
{
    #region Menu Window
    [MenuItem("Tools/Capture Screen")]
    static void OpenWindow()
    {
        EditorScreenCapture window = (EditorScreenCapture)GetWindow(typeof(EditorScreenCapture));
        window.minSize = new Vector2(400, 400);
        window.Show();
    }
    #endregion
    int x = 1024; int  y = 1024;
    Texture2D headerSectionTexture;
    Texture2D oneSectionTexture;

    Texture2D captured;

    Color headerSectionColor = new Color(13f / 255f, 32f / 255f, 44f / 255f, 1f);

    Rect headerSection;
    Rect oneSection;
    Rect oneSection2;

    private void OnEnable()
    {
        InitTextures();
    }
    private void OnGUI()
    {
        DrawLayouts();
        DrawHeader();
        DrawSectionOne();
    }

    void InitTextures()
    {
        headerSectionTexture = new Texture2D(1, 1);
        headerSectionTexture.SetPixel(0, 0, headerSectionColor);
        headerSectionTexture.Apply();

        //oneSectionTexture = Resources.Load<Texture2D>("icons/StoreIcon");
        oneSectionTexture = new Texture2D(1, 1);
        oneSectionTexture.SetPixel(0, 0, Color.cyan);
        oneSectionTexture.Apply();

        captured = new Texture2D(1, 1);
        captured.SetPixel(0, 0, Color.black);
        captured.Apply();

    }
    void DrawLayouts()
    {
        headerSection.x = 0;
        headerSection.y = 0;
        headerSection.width = Screen.width;
        headerSection.height = 50;

        oneSection.x = 0;
        oneSection.y = 50;
        oneSection.width = Screen.width / 3;
        oneSection.height = 100;

        oneSection2.x = 0;
        oneSection2.y = 150;
        oneSection2.width = Screen.width;
        oneSection2.height = Screen.height;

        GUI.DrawTexture(headerSection, headerSectionTexture);
       // GUI.DrawTexture(oneSection, oneSectionTexture);
        GUI.DrawTexture(oneSection2, captured);
    }
    void DrawHeader()
    {
        GUILayout.BeginArea(headerSection);
        GUILayout.Label("Section Title");
        GUILayout.EndArea();
    }
    void DrawSectionOne()
    {
        GUILayout.BeginArea(oneSection);
        GUILayout.BeginHorizontal();
        GUILayout.Label("X");
        x = EditorGUILayout.IntField(x);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Y");
        y = EditorGUILayout.IntField(y);
        GUILayout.EndHorizontal();


        if (GUILayout.Button("Capture"))
        {
            CaptureScreen();
        }

        GUILayout.EndArea();
    }

    private void CaptureScreen()
    {
        Camera cam = Camera.main;

        RenderTexture rt = new RenderTexture(x, y, 24);
        cam.targetTexture = rt;

        Texture2D screenShot = new Texture2D(x, y, TextureFormat.RGBA32, false);
        cam.Render();
        RenderTexture.active = rt;

        screenShot.ReadPixels(new Rect(0, 0, x, y), 0, 0);
        cam.targetTexture = null;
        RenderTexture.active = null;

        DestroyImmediate(rt);

        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes("D:/Projects/ViheclePhysics/Assets/Resources/icons/New Screen Capture.png", bytes);

        AssetDatabase.Refresh();

        captured = Resources.Load<Texture2D>("icons/New Screen Capture");
    }
}
