/*
    1) Resize - X
    2) Rotating - X
    3) RGBA - X
    4) Brightness - X
    5) Grayscale - ✓
    6) Contrast - ✓
    7) Saturation - X
    8) Hue - X
    9) Tint - X
*/

/*
using UnityEngine;
using UnityEditor;
using MadLab.Utilities;
using Sirenix.OdinInspector;

public class TextureEditor : EditorWindow {

    public enum UpdateTypes{
        UpdateOFF,
        NeedUpdate,
        Updated,
    }

    private UpdateTypes update;

    private Texture2D myTexture;
    private Texture2D textureDefaultData;
    private Texture2D textureOnWorking;
    private Texture2D aseGrid => Resources.Load<Texture2D>("Editor Arts/Texture Editor/Grid128");
    private Texture2D updateOff => Resources.Load<Texture2D>("Editor Arts/Texture Editor/UpdateOFF");
    private Texture2D updateUpToDated => Resources.Load<Texture2D>("Editor Arts/Texture Editor/UpdateOutdated");
    private Texture2D updateOutDated => Resources.Load<Texture2D>("Editor Arts/Texture Editor/UpdateUpToDated");

    private Texture2D headerSectionTexture;
    private Texture2D toolSectionTexture;
    private Texture2D previewSectionTexture;

    private Color headerSectionColor = new Color(0f, 0f, 0f, 0.25f);
    private Color toolSectionColor = Utility.GetColorFromString("393E46");
    private Color preivewSectionColor = Utility.GetColorFromString("232931");

    private Rect myTextureRect;
    private Rect headerSection;
    private Rect toolSection;
    private Rect previewSection;

    int importedTextureID = 0;
    private int contrast = 0;
    private int oldContrast = 0;

    bool isGrayScaled = false;
    bool needUpdate = false;

    [MenuItem("ML Framework/Texture Editor")]
    private static void ShowWindow() {
        var window = GetWindow<TextureEditor>();
        window.titleContent = new GUIContent("Texture Editor");
        window.minSize = new Vector2(800f, 500f);
        window.Show();
    }

    /// <summary>
    /// Similar to Start() or Awake()
    /// </summary>
    private void OnEnable() {
        InitTextures();
    }

    /// <summary>
    /// Initialize Texture2D values
    /// </summary>
    private void InitTextures(){

        toolSectionTexture = new Texture2D(1, 1);
        toolSectionTexture.SetPixel(0, 0, toolSectionColor);
        toolSectionTexture.Apply();

        previewSectionTexture = Resources.Load<Texture2D>("Editor Arts/Texture Editor/Grid128");
        previewSectionTexture.Apply();

        headerSectionTexture = new Texture2D(1, 1);
        headerSectionTexture.SetPixel(0, 0, headerSectionColor);
        headerSectionTexture.Apply();
    }

    private void OnGUI() {
        DrawLayouts();
        DrawPreview();
        DrawTools();
        DrawHeader();


/*      // Zoom event
        Event e = Event.current;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        if (e.GetTypeForControl(controlID) == EventType.ScrollWheel) {
            Vector2 mousePos = GUIUtility.GUIToScreenPoint(e.mousePosition) - new Vector2(position.x, position.y);

        }*/
    //}

    /// <summary>
    /// Defines Rect values and paints textures based on Rects
    /// </summary>
    /*
    private void DrawLayouts(){
        toolSection.x = 0;
        toolSection.y = 0;
        toolSection.width = 200;
        toolSection.height = Screen.height;

        GUI.DrawTexture(toolSection, toolSectionTexture);

        previewSection.x = 200;
        previewSection.y = Screen.height;
        previewSection.width = Screen.width - 200;
        previewSection.height = -Screen.height;

        float width = ((float)Screen.width - 200) / 50f;
        float height = (float)Screen.height / 50f;

        GUI.DrawTextureWithTexCoords(previewSection, previewSectionTexture, new Rect(0f, 0f, width, height));

        headerSection.x = 200;
        headerSection.y = 0;
        headerSection.width = Screen.width - 200;
        headerSection.height = 25;

        GUI.DrawTexture(headerSection, headerSectionTexture);
    }

    private void ResetAll(){
        contrast = 0;
        oldContrast = 0;
        isGrayScaled = false;
        textureOnWorking = new Texture2D(0, 0);
        importedTextureID = 0;
    }

    private void DrawTools(){
        myTexture = (Texture2D)EditorGUI.ObjectField(new Rect(5, 35, 190, 15), myTexture, typeof(Texture2D), false);

        if (myTexture) {
            if (myTexture.GetInstanceID() != importedTextureID) {
                if (!myTexture.isReadable)
                    SetTextureImporterFormat(myTexture, true);

                ResetAll();
                update = UpdateTypes.Updated;

                textureOnWorking = new Texture2D(myTexture.width, myTexture.height);
                textureOnWorking.SetPixels(myTexture.GetPixels());
                textureOnWorking.Apply();
                
                textureDefaultData = new Texture2D(myTexture.width, myTexture.height);
                textureDefaultData.SetPixels(myTexture.GetPixels());
                textureDefaultData.Apply();

                importedTextureID = myTexture.GetInstanceID();
            }

            GUI.Label(new Rect(5, 60, 190, 20), "Contrast :");
            contrast = EditorGUI.IntSlider(new Rect(5, 80, 190, 20), contrast, -100, 100);
            if (oldContrast != contrast)
            {
                SetContrast(contrast);
                oldContrast = contrast;
            }

            if (GUI.Button(new Rect(5, 100, 190, 20), "Grayscale")) {
                MakeGrayscale();
            }
        }
        else if (importedTextureID != 0) {
            ResetAll();
            update = UpdateTypes.UpdateOFF;
        }
    }

    float multiplier = 1f;
    private void DrawPreview(){
        if (!textureOnWorking)
            return;

        Vector2 previewArea = new Vector2(Screen.width - 200, Screen.height-20);
        
        Vector2 size = new Vector2(textureOnWorking.width, textureOnWorking.height);


        if (previewArea.x >= size.x && previewArea.y >= size.y){
            myTextureRect.x = 200 + (previewArea.x - size.x) * 0.5f;
            myTextureRect.y = (previewArea.y - size.y) * 0.5f;
            myTextureRect.width = size.x;
            myTextureRect.height = size.y;
        }
        else
        {
            Vector2[] values = { new Vector2(size.x * multiplier, previewArea.x), new Vector2(size.y * multiplier, previewArea.y) };
            Vector2 closest = AdvancedMath.ClosestDisValue(values);

            if (values[0] == closest) {
                // strecth x
                multiplier = previewArea.x / size.x;
                myTextureRect.x = 200;
                myTextureRect.y = (previewArea.y - size.y * multiplier) * 0.5f;
            }
            else
            {
                // strecth y
                multiplier = previewArea.y / size.y;
                myTextureRect.x = 200 + (previewArea.x - size.x * multiplier) * 0.5f;
                myTextureRect.y = 0f;
            }
            myTextureRect.width = size.x * multiplier;
            myTextureRect.height = size.y * multiplier;
        }

        GUI.DrawTexture(myTextureRect, textureOnWorking);
    }

    private void DrawHeader() {
        GUIStyle newStyle = new GUIStyle(GUI.skin.label);
        newStyle.fontSize = 20;
        newStyle.fontStyle = FontStyle.Bold;
        newStyle.alignment = TextAnchor.MiddleCenter;

        GUI.Label(new Rect(200, 0, headerSection.width, headerSection.height), myTexture ? myTexture.name : "None Texture", newStyle);

        Color c = GUI.backgroundColor;
        GUI.backgroundColor = Color.clear;
        newStyle.overflow = new RectOffset(0, 0, 0, 0);
        if (GUI.Button(new Rect(205, 2, 35, 21), update == UpdateTypes.UpdateOFF ? updateOff : update == UpdateTypes.NeedUpdate ? updateUpToDated : updateOutDated, newStyle)){
            update = UpdateTypes.Updated;
        }
        GUI.backgroundColor = c;
    }

#region Operaitons

    private void MakeGrayscale(){
        if (!myTexture)
            return;

        for (int x = 0; x < textureOnWorking.width; x++){
            for(int y = 0; y < textureOnWorking.height; y++){
                Color pixel = textureOnWorking.GetPixel(x, y);
                float gray = (pixel.r + pixel.g + pixel.b) / 3f;
                pixel = new Color(gray, gray, gray, pixel.a);
                textureOnWorking.SetPixel(x, y, pixel);
            }
        }

        update = UpdateTypes.NeedUpdate;

        isGrayScaled = true;
        textureOnWorking.Apply();
    }

    // threshold should be a value between -100 and 100
    private void SetContrast(int threshold)
    {
        if (!myTexture)
            return;

        var contrast = Mathf.Pow((100.0f + threshold) / 100.0f, 2f);
        Texture2D targetTexture = isGrayScaled ? textureOnWorking : myTexture;

        for (int x = 0; x < textureOnWorking.width; x++)
        {
            for (int y = 0; y < textureOnWorking.height; y++) 
            {
                Color oldColor = myTexture.GetPixel(x, y);
                float red = (((oldColor.r - 0.5f) * contrast) + 0.5f);
                float green = (((oldColor.g - 0.5f) * contrast) + 0.5f);
                float blue = (((oldColor.b - 0.5f) * contrast) + 0.5f);
                red = Mathf.Clamp(red, 0f, 1f);
                green = Mathf.Clamp(green, 0f, 1f);
                blue = Mathf.Clamp(blue, 0f, 1f);
    
                textureOnWorking.SetPixel(x, y, new Color(red, green, blue, oldColor.a));
            }
        }

        update = UpdateTypes.NeedUpdate;
        textureOnWorking.Apply();
    }

#endregion

    public static void SetTextureImporterFormat(Texture2D texture, bool isReadable)
    {
        if ( null == texture ) return;

        string assetPath = AssetDatabase.GetAssetPath( texture );
        var tImporter = AssetImporter.GetAtPath( assetPath ) as TextureImporter;
        if ( tImporter != null )
        {
            tImporter.isReadable = isReadable;

            AssetDatabase.ImportAsset( assetPath );
            AssetDatabase.Refresh();
        }
    }
}
*/