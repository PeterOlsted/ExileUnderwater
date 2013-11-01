using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UtilTuple;

public class UtilGL_Line : MonoBehaviour {

    Material m;
    static bool instanceCreated;

    static List<Tuple<Vector3, Vector3, Color>> lines;
    static List<Tuple<Vector3, Vector3, Color, float>> durationLines;

	// Use this for initialization
	void Awake () 
    {
        lines = new List<Tuple<Vector3, Vector3, Color>>();
        durationLines = new List<Tuple<Vector3, Vector3, Color, float>>();
	}

    void OnPostRender()
    {
        if (instanceCreated)
        {
            //m = new Material("Shader \"Lines/Colored Blended\" { SubShader { Pass { Blend SrcAlpha OneMinusSrcAlpha BindChannels { Bind \"Color\",color } ZWrite On Cull Front Fog { Mode Off } } } }");
            m = new Material("Shader \"Lines/Colored Blended\" { SubShader { Tags { \"RenderType\"=\"Opaque\" } Pass { ZWrite On ZTest LEqual Cull Off Fog { Mode Off } BindChannels { Bind \"vertex\", vertex Bind \"color\", color } } } }");
            GL.PushMatrix();
            m.SetPass(0);
            GL.Begin(GL.LINES);

            //DrawLines that clears after a single draw
            foreach (Tuple<Vector3, Vector3, Color> t in lines)
            {
                GL.Color(t.object3);
                GL.Vertex(t.object1);
                GL.Vertex(t.object2);
            }
            lines = new List<Tuple<Vector3, Vector3, Color>>();
            List<Tuple<Vector3, Vector3, Color, float>> removeUs = new List<Tuple<Vector3, Vector3, Color, float>>();

            //Draw the other lines
            foreach (Tuple<Vector3, Vector3, Color, float> t in durationLines)
            {
                GL.Color(t.object3);
                GL.Vertex(t.object1);
                GL.Vertex(t.object2);
                t.object4 = t.object4 - Time.deltaTime;
                if (t.object4 <= 0) //remove if time has run out
                    removeUs.Add(t);
            }

            GL.End();
            GL.PopMatrix();

            //clear list
            if (removeUs.Count > 0)
            {
                foreach (Tuple<Vector3, Vector3, Color, float> t in removeUs)
                {
                    durationLines.Remove(t);
                }
            }
        }
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
    {
        if (durationLines == null)
        {
            CreateInstance();
        }
        if (duration > 0)
            durationLines.Add(new Tuple<Vector3, Vector3, Color, float>(start, end, color, duration));
        else
        {
            if (lines == null)
                lines = new List<Tuple<Vector3, Vector3, Color>>();

            lines.Add(new Tuple<Vector3, Vector3, Color>(start, end, color));
        }
    }

    static void CreateInstance()
    {
        Camera.main.gameObject.AddComponent<UtilGL_Line>();
        instanceCreated = true;
    }
}
