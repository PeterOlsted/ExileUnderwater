using UnityEngine;
using UtilTuple;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class UtilOnGUI : MonoBehaviour {
	[HideInInspector]
	public int textSize = 16;
	[HideInInspector]
	public int objectTextSize = 16;
	[HideInInspector]
	public Color textColor = Color.white;
	[HideInInspector]
	public Color objectTextColor = Color.white;
	[HideInInspector]
	public Color objectBackgroundColor = Color.black;
	[HideInInspector]
	public bool textBackground = true;
	[HideInInspector]
	public bool objectTextBackground = true;
	[HideInInspector]
	public bool onGUI = true;
	[HideInInspector]
	public bool onGUIObjects = true;
	[HideInInspector]
	public bool scaleWithDistace;
	[HideInInspector]
	public float debugWindowWidth = 0.4f;
	[HideInInspector]
	public float debugWindowHeight = 0.3f;
	
	[HideInInspector]
	Dictionary<GameObject, Dictionary<string, object>> StringDict = new Dictionary<GameObject, Dictionary<string, object>>();
	[HideInInspector]
	Vector2 temp;
	
	[HideInInspector]
	public static float deltaTime;
	
	public Material mat;
	
	float _currentTime;
	float _oldTime;
	
	private List<Util.Debug._DebugLine> removeDebugLines = new List<Util.Debug._DebugLine>();
	private GUIStyle debugLine_GUIstyle;
	private GUIStyle debugObject_GUIstyle;
	private GUIStyle debugObjectLabel_GUIstyle;
	
	void Init()
	{
		debugObject_GUIstyle = GUI.skin.box;
		debugObject_GUIstyle.alignment = TextAnchor.MiddleLeft;
		debugObject_GUIstyle.font = (Font) Resources.Load("Courier");
		debugObject_GUIstyle.normal.textColor = objectTextColor;
		
		debugObjectLabel_GUIstyle = GUI.skin.label;
		debugObjectLabel_GUIstyle.alignment = TextAnchor.MiddleLeft;
		debugObjectLabel_GUIstyle.font = (Font) Resources.Load("Courier");
		debugObjectLabel_GUIstyle.normal.textColor = objectTextColor;
		
		debugLine_GUIstyle = GUIStyle.none;
		debugLine_GUIstyle.font = GUIStyle.none.font;
	}
	
	void OnGUI()
	{
		if (debugLine_GUIstyle == null || debugObject_GUIstyle == null)
		{
			Init ();
		}
		
		if(onGUI)
		{
		    DrawDebugLines();
			DrawDebugLinesPersistent();
		}
		
		if (onGUIObjects)
		{
			//Debug On GUI
			DrawOnObject ();
		}
		RemoveExpiredDebugLines();
	}

	void UpdateDeltaTime ()
	{
		_currentTime = Time.realtimeSinceStartup;
		deltaTime = _currentTime - _oldTime;
		_oldTime = _currentTime;
		
		Util.SetDeltaTime(deltaTime);
	}
	
	void Update()
	{
		UpdateDeltaTime();
		
		if (onGUIObjects)
		{
			//Sort debug on object windows by distance
			StringDict = (from entry in StringDict orderby Vector3.Distance(entry.Key.transform.position, Camera.main.transform.position) descending 
				select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
		}
	}
	
	private Vector2 internal_debugLinesScrollPosition = Vector2.zero;
	private float internal_previousNumOfLines = 0;
	
	public void AddStringAtPosition(GameObject gameObject, string s, object value)
	{		
		if(!StringDict.ContainsKey(gameObject))
		{
			StringDict.Add(gameObject, new Dictionary<string, object>());
			StringDict[gameObject].Add(s, value);
		}
		else
		{
			if(!StringDict[gameObject].ContainsKey(s))
			{
				StringDict[gameObject].Add(s, value);
			}
			else
			{
				StringDict[gameObject][s] = value;
			}
		}
	}

	void DrawOnObject ()
	{
		foreach(GameObject go in StringDict.Keys)
		{	
			Vector3 drawPos = go.transform.position + new Vector3(0, go.transform.localScale.y, 0);
			Vector3 pos = Camera.main.WorldToScreenPoint(drawPos);
			if (pos.z > 0)
			{
				Dictionary<string, object> dic = StringDict[go];
				int longestString = 0;
				int counter = 1;
				string s = "";
				float distance = 1;
				
				if (scaleWithDistace)
				{
					distance = (Vector3.Distance(Camera.main.transform.position, drawPos))/4;
				}
				
				foreach(KeyValuePair<string, object> kvp in dic)
				{
					string ss = kvp.Key + kvp.Value;
					s += ss;
					if (ss.Length > longestString)
						longestString = ss.Length;
					if (counter != dic.Count)
					{
						counter++;
						s += "\n";
					}
				}
			
				pos.y = Screen.height - pos.y;
				
				int fontSize = (int)(objectTextSize / distance);
				
				
				if (objectTextBackground)
				{
					//debugObject_GUIstyle.normal.background = bg;
					debugObject_GUIstyle.fontSize = fontSize;
					debugObject_GUIstyle.normal.textColor = objectTextColor;
					GUI.Box(new Rect(pos.x - 6, pos.y - 12, (longestString * (fontSize * 0.65f)) + 12, fontSize * dic.Count + 10), s, debugObject_GUIstyle);
					debugObject_GUIstyle.fontSize = textSize;
				}
				else
				{
					debugObjectLabel_GUIstyle.fontSize = fontSize;
					debugObjectLabel_GUIstyle.normal.textColor = objectTextColor;
					GUI.Label(new Rect(pos.x, pos.y, (longestString * fontSize * 0.65f) + 12, fontSize * dic.Count + 12), s, debugObjectLabel_GUIstyle);
					debugObjectLabel_GUIstyle.fontSize = textSize;
				}
			}
		}
	}
	
	void DrawDebugLines()
	{
		//if(textBackground && Util.DebugLines.Count > 0) //we would want this always, so that we can scroll up and down
		if(Util.Debug.DebugLines.Count > 0)
		{
            if (internal_previousNumOfLines != Util.Debug.DebugLines.Count && GUIUtility.hotControl == 0)
			{
				internal_debugLinesScrollPosition.y = Mathf.Infinity;
			}
            internal_previousNumOfLines = Util.Debug.DebugLines.Count;
			
			float swWidth = Screen.width * debugWindowWidth, swHeight = Screen.height * debugWindowHeight;
			Rect groupRect = new Rect(5, Screen.height - swHeight - 6, swWidth, swHeight);
			Rect boxRect = new Rect(2, Screen.height - swHeight - 9, swWidth + 6, swHeight + 6);
			if (textBackground)
				GUI.Box(boxRect, "");
			GUI.BeginGroup(groupRect);
				GUI.SetNextControlName("DebugLinesScrollView");
				internal_debugLinesScrollPosition = GUILayout.BeginScrollView
				(
					internal_debugLinesScrollPosition, false, true, GUILayout.Width(swWidth), GUILayout.Height(swHeight)
				);
				
				//we are drawing everything inside a scrollview
				debugLine_GUIstyle.normal.textColor = textColor;
				debugLine_GUIstyle.margin = new RectOffset(0,0,0,0);
				debugLine_GUIstyle.border = new RectOffset(0,0,0,0);
				debugLine_GUIstyle.fontSize = textSize;
                foreach (Util.Debug._DebugLine dbl in Util.Debug.DebugLines)
			    {
					if(dbl.expirationTime <= Time.time)
						removeDebugLines.Add(dbl);
					if(dbl.color != Color.black)
					{
						debugLine_GUIstyle.normal.textColor = dbl.color;
						GUILayout.Label(string.Format("{0} ({1})", dbl.line, string.Format("{0:0.000}", dbl.creationTime)), debugLine_GUIstyle);
						debugLine_GUIstyle.normal.textColor = textColor;
					}
					else 
					{
						GUILayout.Label(string.Format("{0} ({1})", dbl.line, string.Format("{0:0.000}", dbl.creationTime)), debugLine_GUIstyle);
					}
			    }
				
			    GUILayout.EndScrollView();
			GUI.EndGroup();
		}
	}
	
	void DrawDebugLinesPersistent()
	{
		//Rect used to be created here
		//new Rect(15, 7, Screen.width - 10, textSize * 1.6f);
		float x = 15; float y = 9;
		debugLine_GUIstyle.fontSize = textSize;
		debugLine_GUIstyle.normal.textColor = textColor;
        foreach (Util.Debug._DebugLine dbl in Util.Debug.DebugLinesPersistent)
	    {
			Rect rect = GUILayoutUtility.GetRect(new GUIContent(dbl.value.ToString()) ,debugLine_GUIstyle);
			rect.x = x; rect.y = y;
            if (textBackground && Util.Debug.DebugLinesPersistent.Count > 0)
			{
				GUI.Box(rect, "");
			}
			
			if(dbl.color != Color.black)
			{
				debugLine_GUIstyle.normal.textColor = dbl.color;
				GUI.Label(rect, string.Format("{0} {1} ({2})", dbl.line, dbl.value.ToString(), string.Format("{0:0.000}", dbl.creationTime)), debugLine_GUIstyle);
				debugLine_GUIstyle.normal.textColor = textColor;
			}
			else 
			{
				GUI.Label(rect, string.Format("{0} {1} ({2})", dbl.line, dbl.value.ToString(), string.Format("{0:0.000}", dbl.creationTime)), debugLine_GUIstyle);
			}
			
	        y += textSize * 1.35f;
	    }
	}
	
	void RemoveExpiredDebugLines()
	{
		if(removeDebugLines.Count > 0)
		{
            foreach (Util.Debug._DebugLine dbl in removeDebugLines)
		    {
                Util.Debug.DebugLines.Remove(dbl);
			}
			removeDebugLines.Clear();
		}
	}
}
