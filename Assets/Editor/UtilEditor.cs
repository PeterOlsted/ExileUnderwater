using UnityEngine;
using UnityEditor;
using System.Collections;

public class UtilEditor : EditorWindow 
{
	public GUIStyle debuglineStyle;
    [SerializeField]
    public bool drawGL_Lines = false;
	[SerializeField]
	public Color defaultColor = Color.green;
	[SerializeField]
    public bool textDebugging = true;
	[SerializeField]
    public bool objectDebugging = true;
	[SerializeField]
    public int debugLineTextSize = 12;
	[SerializeField]
    public int debugObjectTextSize = 16;
	[SerializeField]
	public Color debugLineColor = Color.white;
	[SerializeField]
	public Color debugObjectColor = Color.white;
	[SerializeField]
	public bool debugLineBackground = true;
	[SerializeField]
	public bool debugObjectBackground = true;
	[SerializeField]
	public bool scaleWithDistance = true;
	[SerializeField]
	private Vector2 scrollPos;
	[SerializeField]
    public float debugWindowWidth = 0.4f;
	[SerializeField]
    public float debugWindowHeight = 0.3f;
    [SerializeField]
    public bool drawGraph = true;
    [SerializeField]
    public Rect graphRect = new Rect(10, 10, 350, 100);
    [SerializeField]
    public float graphUpdateTime = 0.05f;
	
	[MenuItem ("Window/Util Editor")]
    static void Init () {
        // Get existing open window if one exist, otherwise create a new one
        UtilEditor window = (UtilEditor)EditorWindow.GetWindow(typeof(UtilEditor));
        window.Load();
    }
	
	static UtilOnGUI _UtilGUI;

	void Update()
	{
		if (Time.timeSinceLevelLoad > 0 && Time.timeSinceLevelLoad < 1)
		{
			UpdateValues();
		}
	}
	
	void OnGUI()
	{
		bool update = false;
		
		Color c;
		bool b;
		int i;
		float f;
		
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUIStyle.none);
		//visualDebugging = EditorGUILayout.BeginToggleGroup("Text Debugging", visualDebugging);
		EditorGUILayout.LabelField(new GUIContent("General options"), EditorStyles.boldLabel);
		//EditorGUILayout.HelpBox("General options for debugging and visual appearance", MessageType.Info);
        b = EditorGUILayout.Toggle(new GUIContent("Use GL_Lines", "Whether or not shapes should be drawn using GL_Lines og regular Gizmo lines. GL_Lines are visible in a build, but invisible in the SceneView"), drawGL_Lines);
        if (drawGL_Lines != b) { drawGL_Lines = b; update = true; }
		c = EditorGUILayout.ColorField(new GUIContent("Default shape color", "The color used by default in most Util visual debugging methods.\n(for instance: DrawSphere() will draw with this color if you do not specify another color)"), defaultColor);
		if (defaultColor != c)
		{
			defaultColor = c; update = true;
		}
		EditorGUILayout.Space();
		
		
		
		b = EditorGUILayout.BeginToggleGroup(new GUIContent("Text Debugging","Uncheck TextDebugging to disable text messages sent via DebugLine() and DebugLinePersistent()"), textDebugging);
		    if (textDebugging != b) {textDebugging = b; update = true;}
			EditorGUILayout.BeginHorizontal(GUIStyle.none);
				GUILayout.Label("Width:");
				f = EditorGUILayout.Slider(debugWindowWidth, 0.2f, 0.99f);
				if (debugWindowWidth != f) {debugWindowWidth = f; update = true;}
				GUILayout.Label("Height:");
				f = EditorGUILayout.Slider(debugWindowHeight, 0.05f, 0.99f);
				if (debugWindowHeight != f) {debugWindowHeight = f; update = true;}
			EditorGUILayout.EndHorizontal();
			i = (int)EditorGUILayout.IntSlider(new GUIContent("Debug line text size", "The size of the text used in DebugLine() and DebugLinePersistent()"), debugLineTextSize, 8, 20);
			if (debugLineTextSize != i) {debugLineTextSize = i; update = true;}
			c = EditorGUILayout.ColorField(new GUIContent("Debug line text color", "The color of the text used in DebugLine() and DebugLinePersistent()"), debugLineColor);
			if (debugLineColor != c) {debugLineColor = c; update = true;}
			b = EditorGUILayout.Toggle(new GUIContent("Debug line background", "Whether or not the persistent debug lines should have a background"), debugLineBackground);
			if (debugLineBackground != b) {debugLineBackground = b; update = true;}
			
        EditorGUILayout.EndToggleGroup();
		
		EditorGUILayout.Space();
		
		b = EditorGUILayout.BeginToggleGroup(new GUIContent("Game Object Debugging","Uncheck Game Object Debugging to disable debug messages sent via DebugOnObject()"), objectDebugging);
		    if (objectDebugging != b) {objectDebugging = b; update = true;}
			i = (int)EditorGUILayout.IntSlider(new GUIContent("Debug Object text size", "The size of the text used in DebugOnObject()"), debugObjectTextSize, 10, 100);
			if (debugObjectTextSize != i) {debugObjectTextSize = i; update = true;}
			c = EditorGUILayout.ColorField(new GUIContent("Debug line text color", "The color of the text used in DebugLine() and DebugLinePersistent()"), debugObjectColor);
			if (debugObjectColor != c) {debugObjectColor = c; update = true;}
			b = EditorGUILayout.Toggle(new GUIContent("Debug object background", "Whether or not the persistent debug lines should have a background"), debugObjectBackground);
			if (debugObjectBackground != b) {debugObjectBackground = b; update = true;}
			b = EditorGUILayout.Toggle(new GUIContent("Scale with distance", "Uncheck to disable scaling of windows based on distance to Game Object"), scaleWithDistance);
			if (scaleWithDistance != b) {scaleWithDistance = b; update = true;}
		EditorGUILayout.EndToggleGroup();

        EditorGUILayout.Space();

        b = EditorGUILayout.BeginToggleGroup(new GUIContent("Graph Values", "Uncheck Graph Values to disable displaying the information though Graph()"), drawGraph);
            if (drawGraph != b) { drawGraph = b; update = true; }
            Rect r = (Rect)EditorGUILayout.RectField(graphRect);
            if (graphRect != r) { graphRect = r; update = true;}
            f = EditorGUILayout.FloatField(new GUIContent("Graph Update Time", "The time between the graph debugger updates the values. 0 means every frame"), graphUpdateTime);
            if (graphUpdateTime != f) { graphUpdateTime = f; update = true; }

        EditorGUILayout.EndToggleGroup();
		
		EditorGUILayout.EndScrollView();
		
		if (update)
			UpdateValues();
	}
	
	void UpdateValues()
	{
        Util.Draw.GL_Lines = drawGL_Lines;
		Util.Draw._shapeColor = defaultColor;
		
		Util.OnGUI = textDebugging;
		Util._UtilOnGUI.GetComponent<UtilOnGUI>().textSize = debugLineTextSize;
		Util._UtilOnGUI.GetComponent<UtilOnGUI>().textColor = debugLineColor;
		Util._UtilOnGUI.GetComponent<UtilOnGUI>().textBackground = debugLineBackground;
		Util._UtilOnGUI.GetComponent<UtilOnGUI>().debugWindowWidth = debugWindowWidth;
		Util._UtilOnGUI.GetComponent<UtilOnGUI>().debugWindowHeight = debugWindowHeight;
		
		Util.OnGUIObject = objectDebugging;
		Util._UtilOnGUI.GetComponent<UtilOnGUI>().objectTextSize = debugObjectTextSize;
		Util._UtilOnGUI.GetComponent<UtilOnGUI>().objectTextColor = debugObjectColor;
		Util._UtilOnGUI.GetComponent<UtilOnGUI>().objectTextBackground = debugObjectBackground;
		Util._UtilOnGUI.GetComponent<UtilOnGUI>().scaleWithDistace = scaleWithDistance;

        UtilGraph.DrawGraphs = drawGraph;
        UtilGraph.ChangeOutlineSize(graphRect);
        UtilGraph.updateTime = graphUpdateTime;
	}

    void OnEnable()
    {
        Load();
    }

    void OnDisable()
    {
        Save();
    }

    void OnLostFocus()
    {
        Save();
    }

    //Possibly: OnDestroy, OnFocus

    void Save()
    {
        //Debug.Log("Saving...");
        //Booleans
        EditorPrefs.SetBool("textDebugging", textDebugging);
        EditorPrefs.SetBool("objectDebugging", objectDebugging);
        EditorPrefs.SetBool("drawGL_Lines", drawGL_Lines);
        EditorPrefs.SetBool("debugLineBackground", debugLineBackground);
        EditorPrefs.SetBool("debugObjectBackground", debugObjectBackground);
        EditorPrefs.SetBool("scaleWithDistance", scaleWithDistance);
        EditorPrefs.SetBool("drawGraph", drawGraph);

        //Integers
        EditorPrefs.SetInt("debugLineTextSize", debugLineTextSize);
        EditorPrefs.SetInt("debugObjectTextSize", debugObjectTextSize);

        //Floats
        EditorPrefs.SetFloat("debugWindowWidth", debugWindowWidth);
        EditorPrefs.SetFloat("debugWindowHeight", debugWindowHeight);
        EditorPrefs.SetFloat("graphUpdateTime", graphUpdateTime);

        //Colors
        EditorPrefs.SetFloat("shapeColorR", defaultColor.r);
        EditorPrefs.SetFloat("shapeColorG", defaultColor.g);
        EditorPrefs.SetFloat("shapeColorB", defaultColor.b);

        EditorPrefs.SetFloat("debugLineColorR", debugLineColor.r);
        EditorPrefs.SetFloat("debugLineColorG", debugLineColor.g);
        EditorPrefs.SetFloat("debugLineColorB", debugLineColor.b);

        EditorPrefs.SetFloat("debugObjectColorR", debugObjectColor.r);
        EditorPrefs.SetFloat("debugObjectColorG", debugObjectColor.g);
        EditorPrefs.SetFloat("debugObjectColorB", debugObjectColor.b);

        //Rects
        EditorPrefs.SetInt("graphRectX", (int)graphRect.x);
        EditorPrefs.SetInt("graphRectY", (int)graphRect.y);
        EditorPrefs.SetInt("graphRectW", (int)graphRect.width);
        EditorPrefs.SetInt("graphRectH", (int)graphRect.height);
        //Debug.Log("Saved!");
    }

    void Load()
    {
        //Debug.Log("Loading...");
        //Booleans
        if (EditorPrefs.HasKey("textDebugging"))
            textDebugging = EditorPrefs.GetBool("textDebugging");

        if (EditorPrefs.HasKey("objectDebugging"))
            objectDebugging = EditorPrefs.GetBool("objectDebugging");

        if (EditorPrefs.HasKey("drawGL_Lines"))
            drawGL_Lines = EditorPrefs.GetBool("drawGL_Lines");

        if (EditorPrefs.HasKey("debugLineBackground"))
            debugLineBackground = EditorPrefs.GetBool("debugLineBackground");

        if (EditorPrefs.HasKey("debugObjectBackground"))
            debugObjectBackground = EditorPrefs.GetBool("debugObjectBackground");

        if (EditorPrefs.HasKey("scaleWithDistance"))
            scaleWithDistance = EditorPrefs.GetBool("scaleWithDistance");

        if (EditorPrefs.HasKey("drawGraph"))
            drawGraph = EditorPrefs.GetBool("drawGraph");

        //Integers
        if (EditorPrefs.HasKey("debugLineTextSize"))
            debugLineTextSize = EditorPrefs.GetInt("debugLineTextSize");

        if (EditorPrefs.HasKey("debugObjectTextSize"))
            debugObjectTextSize = EditorPrefs.GetInt("debugObjectTextSize");

        //Floats
        if (EditorPrefs.HasKey("debugWindowWidth"))
            debugWindowWidth = EditorPrefs.GetFloat("debugWindowWidth");

        if (EditorPrefs.HasKey("debugWindowHeight"))
            debugWindowHeight = EditorPrefs.GetFloat("debugWindowHeight");

        if (EditorPrefs.HasKey("graphUpdateTime"))
            graphUpdateTime = EditorPrefs.GetFloat("graphUpdateTime");

        //Colors
        if (EditorPrefs.HasKey("shapeColorR"))
            defaultColor = new Color(EditorPrefs.GetFloat("shapeColorR"),EditorPrefs.GetFloat("shapeColorG"),EditorPrefs.GetFloat("shapeColorB"),1);

        if (EditorPrefs.HasKey("debugLineColorR"))
            debugLineColor = new Color(EditorPrefs.GetFloat("debugLineColorR"), EditorPrefs.GetFloat("debugLineColorG"), EditorPrefs.GetFloat("debugLineColorB"), 1);

        if (EditorPrefs.HasKey("debugObjectColorR"))
            debugObjectColor = new Color(EditorPrefs.GetFloat("debugObjectColorR"), EditorPrefs.GetFloat("debugObjectColorG"), EditorPrefs.GetFloat("debugObjectColorB"), 1);

        //Rects
        if (EditorPrefs.HasKey("graphRectX"))
            graphRect = new Rect(EditorPrefs.GetInt("graphRectX"), EditorPrefs.GetInt("graphRectY"), EditorPrefs.GetInt("graphRectW"), EditorPrefs.GetInt("graphRectH"));
        //Debug.Log("Loaded!");
    }

}
