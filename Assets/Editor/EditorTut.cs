using UnityEditor;
using UnityEngine;

public class EditorTut : EditorWindow
{
    public enum Types
    {
        One,
        Two,
    }
    Types currentType;

    Texture2D headerSectionTexture;
    Texture2D oneSectionTexture;
    Texture2D twoSectionTexture;
    Texture2D threeSectionTexture;

    Color headerSectionColor = new Color(13f / 255f, 32f / 255f, 44f / 255f, 1f);

    Rect headerSection;
    Rect oneSection;
    Rect twoSection;
    Rect threeSection;

    #region Menu Window
    [MenuItem("Tut/UiDesign")]
    static void OpenWindow()
    {
        EditorTut window = (EditorTut)GetWindow(typeof(EditorTut));
        window.minSize = new Vector2(400, 400);
        window.Show();
    }
    #endregion
    #region Editor Events

    private void OnEnable()
    {
        InitTextures();
    }
    private void OnGUI()
    {
        DrawLayouts();
        DrawHeader();
        DrawSectionOne();
        DrawSectionTwo();
        DrawSectionThree();
    }
    #endregion
    void InitTextures()
    {
        headerSectionTexture = new Texture2D(1, 1);
        headerSectionTexture.SetPixel(0, 0, headerSectionColor);
        headerSectionTexture.Apply();

        oneSectionTexture = Resources.Load<Texture2D>("icons/StoreIcon");
        twoSectionTexture = Resources.Load<Texture2D>("icons/StoreIcon");
        threeSectionTexture = Resources.Load<Texture2D>("icons/StoreIcon");
    }
    void DrawLayouts()
    {
        headerSection.x = 0;
        headerSection.y = 0;
        headerSection.width = Screen.width;
        headerSection.height = 50;

        oneSection.x = 0;
        oneSection.y = 50;
        oneSection.width = Screen.width/ 4f;
        oneSection.height = Screen.width - 50;

        twoSection.x = Screen.width / 3f;
        twoSection.y = 50;
        twoSection.width = Screen.width / 3f;
        twoSection.height = Screen.width - 50;

        threeSection.x = (Screen.width / 3) * 2;
        threeSection.y = 50;
        threeSection.width = Screen.width / 3f;
        threeSection.height = Screen.width - 50;

        GUI.DrawTexture(headerSection, headerSectionTexture);
        GUI.DrawTexture(oneSection, oneSectionTexture);
        GUI.DrawTexture(twoSection, twoSectionTexture);
        GUI.DrawTexture(threeSection, threeSectionTexture);
    }
    void DrawHeader()
    {
        GUILayout.BeginArea(headerSection);

        GUILayout.EndArea();
    }
    void DrawSectionOne()
    {
        GUILayout.BeginArea(oneSection);
        GUILayout.Label("Section One");

        currentType = (Types)EditorGUILayout.EnumPopup(currentType);

        GUILayout.EndArea();
    }
    void DrawSectionTwo()
    {
        GUILayout.BeginArea(twoSection);
        GUILayout.Label("Section Two");

        currentType = (Types)EditorGUILayout.EnumPopup(currentType);
        GUILayout.EndArea();
    }
    void DrawSectionThree()
    {
        GUILayout.BeginArea(threeSection);
        GUILayout.Label("Section Three");

        currentType = (Types)EditorGUILayout.EnumPopup(currentType);
        GUILayout.EndArea();
    }
}
