using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public static class Util
{
    #region DeltaTime
    public static float deltaTime
    {
        get 
        {
            if (!_utilOnGUI)
            {
                _utilOnGUI = new GameObject();
                _utilOnGUI.name = "UtilOnGUI";
                _utilOnGUI.AddComponent<UtilOnGUI>();
            }
            return _deltaTime;
        }
    }
    private static float _deltaTime;
	public static void SetDeltaTime (float deltaTime)
	{
		_deltaTime = deltaTime;
	}
    #endregion

    #region Draw Shapes

    public class Draw
    {
        public static Color _shapeColor = Color.white;
        public static bool GL_Lines = true;
        private static int _circleResolution = 20;

        public static void Line(Vector3 start, Vector3 end, Color color, float duration)
        {
            if (GL_Lines)
            {
                UtilGL_Line.DrawLine(start, end, color, duration);
            }
            else
            {
                UnityEngine.Debug.DrawLine(start, end, color, duration, true);
            }
        }

        #region Point
        public static void Point(Vector3 position, float radius, Color color, float duration)
        {
            Line(position, position + (Vector3.up * radius), color, duration);
            Line(position, position + (Vector3.right * radius), color, duration);
            Line(position, position + (Vector3.forward * radius), color, duration);
        }

        public static void Point(Vector3 position, float radius, Color color)
        {
            Point(position, radius, color, 0);
        }

        public static void Point(Vector3 position, Color color)
        {
            Point(position, 0.25f, color, 0);
        }

        public static void Point(Vector3 position, float radius, float duration)
        {
            float cubeRadius = 0.25f; //0-1

            Line(position, position + (Vector3.up * radius), Color.green, duration);
            Line(position, position + (Vector3.right * radius), Color.red, duration);
            Line(position, position + (Vector3.forward * radius), Color.blue, duration);

            Line(position + (Vector3.right * radius * cubeRadius) + (Vector3.up * radius * cubeRadius), position + (Vector3.up * radius * cubeRadius), Color.green, duration);
            Line(position + (Vector3.forward * radius * cubeRadius) + (Vector3.right * radius * cubeRadius), position + (Vector3.right * radius * cubeRadius), Color.red, duration);
            Line(position + (Vector3.up * radius * cubeRadius) + (Vector3.forward * radius * cubeRadius), position + (Vector3.forward * radius * cubeRadius), Color.blue, duration);

            Line(position + (Vector3.forward * radius * cubeRadius) + (Vector3.up * radius * cubeRadius), position + (Vector3.up * radius * cubeRadius), Color.green, duration);
            Line(position + (Vector3.up * radius * cubeRadius) + (Vector3.right * radius * cubeRadius), position + (Vector3.right * radius * cubeRadius), Color.red, duration);
            Line(position + (Vector3.right * radius * cubeRadius) + (Vector3.forward * radius * cubeRadius), position + (Vector3.forward * radius * cubeRadius), Color.blue, duration);
        }

        public static void Point(Vector3 position, float radius)
        {
            Point(position, radius, 0);
        }

        public static void Point(Vector3 position)
        {
            Point(position, 1);
        }

        public static void Point2D(Vector3 position, float radius, Color color, float duration)
        {
            if (!Application.isPlaying && !Application.isEditor)
                return;

            if (Time.timeSinceLevelLoad > 0.1f && Camera.current != null)
            {
                Line(position + (Camera.current.transform.right + Camera.current.transform.up).normalized * radius, position + (-Camera.current.transform.right - Camera.current.transform.up).normalized * radius, color, duration);
                Line(position + (Camera.current.transform.right - Camera.current.transform.up).normalized * radius, position + (-Camera.current.transform.right + Camera.current.transform.up).normalized * radius, color, duration);
            }
        }

        public static void Point2D(Vector3 position, float radius, Color color)
        {
            Point2D(position, radius, color, 0);
        }

        public static void Point2D(Vector3 position, float radius)
        {
            Point2D(position, radius, _shapeColor, 0);
        }

        public static void Point2D(Vector3 position)
        {
            Point2D(position, 0.25f, _shapeColor, 0);
        }

        #endregion
        #region Target
        public static void Target(Vector3 position, float radius, Color color, float duration)
        {
            Sphere(position, radius, color, duration);
            Arrow(position + new Vector3(0, radius * 6, 0), position + new Vector3(0, radius, 0), color, duration);
        }

        public static void Target(Vector3 position, float radius, Color color)
        {
            Target(position, radius, color, 0);
        }

        public static void Target(Vector3 position, float radius)
        {
            Target(position, radius, _shapeColor, 0);
        }

        public static void Target(Vector3 position)
        {
            Target(position, 0.25f, _shapeColor, 0);
        }
        #endregion
        #region Arrow
        public static void Arrow(Vector3 origin, Vector3 end, Color color, float duration)
        {
            //DrawLine(origin, end, color, duration);
            if (origin == end)
                return;

            float distance = Vector3.Distance(origin, end);
            float arrowWidth = distance * 0.05f;
            float tipWidth = distance * 0.1f;
            float tipLength = distance * 0.2f;

            GameObject go = new GameObject();
            go.transform.position = origin;
            go.transform.LookAt(end);

            //Arrow Butt horizontal
            Line(origin + (go.transform.right).normalized * arrowWidth, origin - (go.transform.right).normalized * arrowWidth, color, duration);
            Line(origin + (go.transform.up).normalized * arrowWidth, origin - (go.transform.up).normalized * arrowWidth, color, duration);

            go.transform.position = Vector3.Lerp(origin, end, 1 - (tipLength / distance));

            //Arrow sides horizontal
            Line(origin + (go.transform.right).normalized * arrowWidth, go.transform.position + (go.transform.right).normalized * arrowWidth, color, duration);
            Line(origin - (go.transform.right).normalized * arrowWidth, go.transform.position - (go.transform.right).normalized * arrowWidth, color, duration);

            Line(origin + (go.transform.up).normalized * arrowWidth, go.transform.position + (go.transform.up).normalized * arrowWidth, color, duration);
            Line(origin - (go.transform.up).normalized * arrowWidth, go.transform.position - (go.transform.up).normalized * arrowWidth, color, duration);

            //Arrowhead
            Line(go.transform.position + (go.transform.right).normalized * arrowWidth, go.transform.position + (go.transform.right).normalized * tipWidth, color, duration);
            Line(go.transform.position - (go.transform.right).normalized * arrowWidth, go.transform.position - (go.transform.right).normalized * tipWidth, color, duration);

            Line(go.transform.position + (go.transform.up).normalized * arrowWidth, go.transform.position + (go.transform.up).normalized * tipWidth, color, duration);
            Line(go.transform.position - (go.transform.up).normalized * arrowWidth, go.transform.position - (go.transform.up).normalized * tipWidth, color, duration);



            Line(end, go.transform.position + (go.transform.right).normalized * tipWidth, color, duration);
            Line(end, go.transform.position - (go.transform.right).normalized * tipWidth, color, duration);

            Line(end, go.transform.position + (go.transform.up).normalized * tipWidth, color, duration);
            Line(end, go.transform.position - (go.transform.up).normalized * tipWidth, color, duration);

            GameObject.Destroy(go);
        }

        public static void Arrow(Vector3 origin, Vector3 end, Color color)
        {
            Arrow(origin, end, color, 0);
        }

        public static void Arrow(Vector3 origin, Vector3 end)
        {
            Arrow(origin, end, _shapeColor, 0);
        }
        #endregion
        #region Area
        public static void Area(Vector3 corner1, Vector3 corner2, Color color, float duration)
        {
            Line(corner1, Change.Vector3(corner1, x: corner2.x), color, duration);
            Line(corner1, Change.Vector3(corner1, y: corner2.y), color, duration);
            Line(corner1, Change.Vector3(corner1, z: corner2.z), color, duration);

            Line(corner2, Change.Vector3(corner2, x: corner1.x), color, duration);
            Line(corner2, Change.Vector3(corner2, y: corner1.y), color, duration);
            Line(corner2, Change.Vector3(corner2, z: corner1.z), color, duration);

            Line(Change.Vector3(corner1, x: corner2.x), Change.Vector3(corner2, y: corner1.y), color, duration);
            Line(Change.Vector3(corner1, y: corner2.y), Change.Vector3(corner2, z: corner1.z), color, duration);
            Line(Change.Vector3(corner1, z: corner2.z), Change.Vector3(corner2, x: corner1.x), color, duration);

            Line(Change.Vector3(corner1, x: corner2.x), Change.Vector3(corner2, z: corner1.z), color, duration);
            Line(Change.Vector3(corner1, y: corner2.y), Change.Vector3(corner2, x: corner1.x), color, duration);
            Line(Change.Vector3(corner1, z: corner2.z), Change.Vector3(corner2, y: corner1.y), color, duration);
        }

        public static void Area(Vector3 corner1, Vector3 corner2, Color color)
        {
            Area(corner1, corner2, color, 0);
        }

        public static void Area(Vector3 corner1, Vector3 corner2)
        {
            Area(corner1, corner2, _shapeColor, 0);
        }
        #endregion
        #region Bounding Box
        public static void BoundingBox(GameObject gameObject, Color color, float duration)
        {
            if (gameObject.renderer)
            {
                Area(gameObject.renderer.bounds.min, gameObject.renderer.bounds.max, color, duration);
            }
        }

        public static void BoundingBox(GameObject gameObject, Color color)
        {
            BoundingBox(gameObject, color, 0);
        }

        public static void BoundingBox(GameObject gameObject)
        {
            BoundingBox(gameObject, _shapeColor, 0);
        }

        public static void BoundingBox(Renderer renderer, Color color, float duration)
        {
            Area(renderer.bounds.min, renderer.bounds.max, color, duration);
        }

        public static void BoundingBox(Renderer renderer, Color color)
        {
            BoundingBox(renderer, color, 0);
        }

        public static void BoundingBox(Renderer renderer)
        {
            BoundingBox(renderer, _shapeColor, 0);
        }

        #endregion
        #region Cube
        public static void Cube(Vector3 position, Vector3 right, Vector3 up, Vector3 forward, Vector3 size, Color color, float duration)
        {
            Vector3 corner1, corner2, corner3, corner4, corner5, corner6, corner7, corner8;
            size /= 2;
            corner1 = position + (right * size.x) + (up * size.y) + (forward * size.z);
            corner2 = position + (right * size.x) + (up * size.y) - (forward * size.z);
            corner3 = position + (right * size.x) - (up * size.y) + (forward * size.z);
            corner4 = position + (right * size.x) - (up * size.y) - (forward * size.z);
            corner5 = position - (right * size.x) + (up * size.y) + (forward * size.z);
            corner6 = position - (right * size.x) + (up * size.y) - (forward * size.z);
            corner7 = position - (right * size.x) - (up * size.y) + (forward * size.z);
            corner8 = position - (right * size.x) - (up * size.y) - (forward * size.z);

            Line(corner1, corner2, color, duration);
            Line(corner3, corner4, color, duration);
            Line(corner5, corner6, color, duration);
            Line(corner7, corner8, color, duration);

            Line(corner1, corner3, color, duration);
            Line(corner2, corner4, color, duration);
            Line(corner3, corner7, color, duration);
            Line(corner4, corner8, color, duration);

            Line(corner1, corner5, color, duration);
            Line(corner7, corner5, color, duration);
            Line(corner2, corner6, color, duration);
            Line(corner6, corner8, color, duration);
        }

        public static void Cube(Vector3 position, Vector3 right, Vector3 up, Vector3 forward, Vector3 size, Color color)
        {
            Cube(position, right, up, forward, size, color, 0);
        }

        public static void Cube(Vector3 position, Vector3 right, Vector3 up, Vector3 forward, Vector3 size)
        {
            Cube(position, right, up, forward, size, _shapeColor, 0);
        }

        public static void Cube(Transform transform, Vector3 size)
        {
            Cube(transform.position, transform.right, transform.up, transform.forward, size);
        }

        public static void Cube(Transform transform)
        {
            Cube(transform, transform.localScale);
        }

        public static void Cube(GameObject gameObject)
        {
            Cube(gameObject.transform);
        }
        #endregion
        #region Circle
        static public void Circle(Vector3 origin, Vector3 aroundAxis, float radius, Color color, float duration, int resolution)
        {
            aroundAxis = aroundAxis.normalized;
            float step = Mathf.PI / (resolution * 2f);

            Vector3 dir1, dir2;
            dir1 = Vector3.Cross(aroundAxis, Vector3.Dot(Vector3.up, aroundAxis) == 0 ? Vector3.up : Vector3.right).normalized * radius;
            dir2 = Vector3.Cross(aroundAxis, dir1).normalized * radius;
            for (int i = 0; i < resolution; i++)
            {
                Line(origin + Vector3.RotateTowards(dir1, dir2, step * (i - 1), 0.0f), origin + Vector3.RotateTowards(dir1, dir2, step * i, 0.0f), color, duration);
                Line(origin + Vector3.RotateTowards(dir2, -dir1, step * (i - 1), 0.0f), origin + Vector3.RotateTowards(dir2, -dir1, step * i, 0.0f), color, duration);
                Line(origin + Vector3.RotateTowards(-dir1, -dir2, step * (i - 1), 0.0f), origin + Vector3.RotateTowards(-dir1, -dir2, step * i, 0.0f), color, duration);
                Line(origin + Vector3.RotateTowards(-dir2, dir1, step * (i - 1), 0.0f), origin + Vector3.RotateTowards(-dir2, dir1, step * i, 0.0f), color, duration);
            }
        }

        public static void Circle(Vector3 origin, Vector3 aroundAxis, float radius, Color color, float duration)
        {
            Circle(origin, aroundAxis, radius, _shapeColor, duration, _circleResolution);
        }

        public static void Circle(Vector3 origin, Vector3 aroundAxis, float radius, Color color)
        {
            Circle(origin, aroundAxis, radius, color, 0f, _circleResolution);
        }

        public static void Circle(Vector3 origin, Vector3 aroundAxis, float radius)
        {
            Circle(origin, aroundAxis, radius, _shapeColor, 0f, _circleResolution);
        }

        public static void Circle(Vector3 origin, Vector3 aroundAxis)
        {
            Circle(origin, aroundAxis, 1f, _shapeColor, 0f, _circleResolution);
        }
        #endregion
        #region Sphere
        static public void Sphere(Vector3 origin, float radius, Color color, float duration, int resolution)
        {
            try
            {
                Circle(origin, Vector3.up, radius, color, duration, resolution);
                Circle(origin, Vector3.right, radius, color, duration, resolution);
                Circle(origin, Vector3.forward, radius, color, duration, resolution);

                if (!Application.isPlaying && !Application.isEditor)
                {
                    return;
                }


                Camera cam;
#if UNITY_EDITOR
                if (GL_Lines)
                    cam = Camera.main;
                else if (UnityEditor.SceneView.currentDrawingSceneView != null)
                    cam = Camera.current;
                else
#endif
                    cam = Camera.main;

                if (Time.timeSinceLevelLoad > 0.1f && cam != null && duration <= 0)
                {
                    if (cam.camera.isOrthoGraphic)
                    {
                        Circle(origin, cam.camera.transform.position - origin, radius, color, duration, resolution);
                    }
                    else
                    {
                        float c = Vector3.Distance(origin, cam.camera.transform.position);
                        float horizonDistance = Mathf.Sqrt(c * c - (radius * radius));
                        float angle = Mathf.Acos(horizonDistance / c);
                        float opposite = Mathf.Tan(angle) * c;
                        if (opposite >= 0 && Mathf.Infinity > opposite) // Prevent NaN which makes it crash (not... nevermind)
                            Circle(origin, cam.camera.transform.position - origin, opposite, color, duration, resolution);
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        public static void Sphere(Vector3 origin, float radius, Color color, float duration)
        {
            Sphere(origin, radius, color, duration, _circleResolution);
        }

        public static void Sphere(Vector3 origin, float radius, Color color)
        {
            Sphere(origin, radius, color, 0f, _circleResolution);
        }

        public static void Sphere(Vector3 origin, float radius)
        {
            Sphere(origin, radius, _shapeColor, 0f, _circleResolution);
        }

        public static void Sphere(Vector3 origin)
        {
            Sphere(origin, 1f, _shapeColor, 0f, _circleResolution);
        }

        public static void Sphere(Transform transform, Color color, float duration, int resolution)
        {
            Sphere(transform.position, Mathf.Max(transform.localScale.z, Mathf.Max(transform.localScale.x, transform.localScale.y)), color, duration, resolution);
        }

        public static void Sphere(Transform transform, Color color, float duration)
        {
            Sphere(transform, color, duration, _circleResolution);
        }

        public static void Sphere(Transform transform, Color color)
        {
            Sphere(transform, color, 0);
        }

        public static void Sphere(Transform transform)
        {
            Sphere(transform, _shapeColor);
        }

        public static void Sphere(GameObject gameObject)
        {
            Sphere(gameObject.transform);
        }
        #endregion
        #region Recursive
        public static GameObject Root(GameObject gameObject)
        {
            return gameObject.transform.root.gameObject;
        }

        public static Transform Root(Transform transform)
        {
            return transform.root.gameObject.transform;
        }

        public static GameObject Root(MonoBehaviour monoBehaviour)
        {
            return Root(monoBehaviour.gameObject);
        }

        public static void BoundingBoxForChildren(Renderer renderer)
        {

            Draw.BoundingBox(renderer);
            foreach (Renderer r in renderer.GetComponentsInChildren<Renderer>())
            {
                Draw.BoundingBox(r);
            }
        }

        public static void BoundingBoxForChildren(MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour.renderer != null)
                Draw.BoundingBox(monoBehaviour.renderer);
            foreach (Renderer r in monoBehaviour.GetComponentsInChildren<Renderer>())
            {
                Draw.BoundingBox(r);
            }
        }

        public static void CubeForChildren(Transform transform)
        {
            Draw.Cube(transform);
            foreach (Transform t in transform.GetComponentsInChildren<Transform>())
            {
                Draw.Cube(t);
            }
        }

        public static void CubeForChildren(Transform transform, Vector3 size)
        {
            Draw.Cube(transform);
            foreach (Transform t in transform.GetComponentsInChildren<Transform>())
            {
                Draw.Cube(t, size);
            }
        }

        public static void CubeForChildren(GameObject gameObject)
        {
            CubeForChildren(gameObject.transform);
        }

        public static void CubeForChildren(GameObject gameObject, Vector3 size)
        {
            CubeForChildren(gameObject.transform, size);
        }
    }
		#endregion
	#endregion
	
	#region Change Types
    public class Change
    {

        /// <summary>
        /// Changes the vector2. Can change individual properties by name.
        /// Example: "ChangeVector2(thisQuat, y: 0.5f);"
        /// </summary>
        /// <returns>
        /// The updated vector2.
        /// </returns>
        /// <param name='original'>
        /// Vector2 that should be changed.
        /// </param>
        /// <param name='x'>
        /// X.
        /// </param>
        /// <param name='y'>
        /// Y.
        /// </param>
        public static Vector2 Vector2(Vector3 original, float? x = new Nullable<float>(), float? y = new Nullable<float>())
        {
            return new Vector2
            (
                x.HasValue ? x.Value : original.x,
                y.HasValue ? y.Value : original.y
            );
        }

        /// <summary>
        /// Changes the properties of a vector3.  Can change individual properties by name.
        /// Example: "Change.Vector3(thisVec3, x: 0.5f, z: 0.99f);"
        /// </summary>
        /// <returns>
        /// The updated vector3.
        /// </returns>
        /// <param name='original'>
        /// Vector 3 that should be changed.
        /// </param>
        /// <param name='x'>
        /// X.
        /// </param>
        /// <param name='y'>
        /// Y.
        /// </param>
        /// <param name='z'>
        /// Z.
        /// </param>
        public static Vector3 Vector3(Vector3 original, float? x = new Nullable<float>(), float? y = new Nullable<float>(), float? z = new Nullable<float>())
        {
            return new Vector3
            (
                x.HasValue ? x.Value : original.x,
                y.HasValue ? y.Value : original.y,
                z.HasValue ? z.Value : original.z
            );
        }

        /// <summary>
        /// Changes the properties of a vector3 via reference.  Can change individual properties by name.
        /// Example: "Change.Vector3(thisVec3, x: 0.5f, z: 0.99f);"
        /// </summary>
        /// <returns>
        /// The updated vector3.
        /// </returns>
        /// <param name='original'>
        /// Vector 3 that should be changed.
        /// </param>
        /// <param name='x'>
        /// X.
        /// </param>
        /// <param name='y'>
        /// Y.
        /// </param>
        /// <param name='z'>
        /// Z.
        /// </param>
        public static void Vector3(ref Vector3 original, float? x = new Nullable<float>(), float? y = new Nullable<float>(), float? z = new Nullable<float>())
        {
            original = Vector3(original, x, y, z);
        }

        public static Vector3 ClampVector3(Vector3 value, Vector3 min, Vector3 max)
        {
            return new Vector3
            (
                value.x < min.x ? min.x : value.x > max.x ? max.x : value.x,
                value.y < min.y ? min.y : value.y > max.y ? max.y : value.y,
                value.z < min.z ? min.z : value.z > max.z ? max.z : value.z
            );
        }

        public static Vector3 ClampVector3(Vector3 value, float min, float max)
        {
            return new Vector3
            (
                value.x < min ? min : value.x > max ? max : value.x,
                value.y < min ? min : value.y > max ? max : value.y,
                value.z < min ? min : value.z > max ? max : value.z
            );
        }

        public static Vector3 ClampVector3Magnitude(Vector3 vector, float min, float max)
        {
            float magn = vector.magnitude;
            magn = magn < min ? min : magn > max ? max : magn;
            return vector.normalized * magn;
        }

        public static Vector3 SetVector3Magnitude(Vector3 vector, float magnitude)
        {
            return vector.normalized * magnitude;
        }

        public static Vector2 ClampVector2(Vector2 value, Vector2 min, Vector2 max)
        {
            return new Vector2
            (
                value.x < min.x ? min.x : value.x > max.x ? max.x : value.x,
                value.y < min.y ? min.y : value.y > max.y ? max.y : value.y
            );
        }

        public static Vector2 ClampVector2(Vector2 value, float min, float max)
        {
            return new Vector2
            (
                value.x < min ? min : value.x > max ? max : value.x,
                value.y < min ? min : value.y > max ? max : value.y
            );
        }

        public static Vector2 ClampVector2Magnitude(Vector2 vector, float min, float max)
        {
            float magn = vector.magnitude;
            magn = magn < min ? min : magn > max ? max : magn;
            return vector.normalized * magn;
        }

        public static Vector2 SetVector2Magnitude(Vector2 vector, float magnitude)
        {
            return vector.normalized * magnitude;
        }

        /// <summary>
        /// Changes the properties of a quaternion.  Can change individual properties by name.
        /// Example: "Quaternion(thisQuat, x: 0.5f, w: 0.99f);"
        /// </summary>
        /// <returns>
        /// The updated quaternion.
        /// </returns>
        /// <param name='original'>
        /// Quaternion that should be changed.
        /// </param>
        /// <param name='x'>
        /// X.
        /// </param>
        /// <param name='y'>
        /// Y.
        /// </param>
        /// <param name='z'>
        /// Z.
        /// </param>
        /// <param name='w'>
        /// W.
        /// </param>
        public static Quaternion Quaternion(Quaternion original, float? x = new Nullable<float>(), float? y = new Nullable<float>(), float? z = new Nullable<float>(), float? w = new Nullable<float>())
        {
            return new Quaternion
            (
                x.HasValue ? x.Value : original.x,
                y.HasValue ? y.Value : original.y,
                z.HasValue ? z.Value : original.z,
                w.HasValue ? w.Value : original.w
            );
        }

        /// <summary>
        /// Changes the properties of a color.  Can change individual properties by name.
        /// Example: "ChangeColor(thisColor, r: 0.5f, a: 0.99f);"
        /// </summary>
        /// <returns>
        /// The updated color.
        /// </returns>
        /// <param name='original'>
        /// Color that should be changed.
        /// </param>
        /// <param name='r'>
        /// Red (0-1).
        /// </param>
        /// <param name='g'>
        /// Green (0-1).
        /// </param>
        /// <param name='b'>
        /// Blue (0-1).
        /// </param>
        /// <param name='a'>
        /// Alpha (0-1).
        /// </param>
        public static Color Color(Color original, float? r = new Nullable<float>(), float? g = new Nullable<float>(), float? b = new Nullable<float>(), float? a = new Nullable<float>())
        {
            return new Color
            (
                r.HasValue ? r.Value : original.r,
                g.HasValue ? g.Value : original.g,
                b.HasValue ? b.Value : original.b,
                a.HasValue ? a.Value : original.a
            );
        }

        /// <summary>
        /// Changes the color on the specified material
        /// </summary>
        /// <param name='material'>
        /// The material whose color we want to change
        /// </param>
        /// <param name='r'>
        /// The desired red component of the color to change
        /// </param>
        /// <param name='g'>
        /// The desired green component of the color to change
        /// </param>
        /// <param name='b'>
        /// The desired blue component of the color to change
        /// </param>
        /// <param name='a'>
        /// The desired alpha component of the color to change
        /// </param>
        /// <param name='propertyName'>
        /// Property name: This is the equivalent of propertyName parameter on Color.SetColor(). Leave it out to change the main color of the material. 
        /// Typical examples are "_SpecColor", "_Emission", and "_ReflectColor"
        /// </param>
        public static void ColorOnMaterial(Material material, float? r = new Nullable<float>(), float? g = new Nullable<float>(), float? b = new Nullable<float>(), float? a = new Nullable<float>(), string propertyName = "_Color")
        {
            try
            {
                material.SetColor(propertyName, new Color
                (
                    r.HasValue ? r.Value : material.color.r,
                    g.HasValue ? g.Value : material.color.g,
                    b.HasValue ? b.Value : material.color.b,
                    a.HasValue ? a.Value : material.color.a
                ));
            }
            catch (Exception e) //TODO: We might be able to do something better than this, to show the user that he did something specific wrong
            {
                UnityEngine.Debug.LogException(e);
            }
        }
    }
	#endregion
	
	#region Transform
	/// <summary>
	/// A smooth LookAt for a transform (t)
	/// </summary>
	/// <param name='t'>
	/// T.
	/// </param>
	/// <param name='target'>
	/// Target.
	/// </param>
	/// <param name='speed'>
	/// Speed.
	/// </param>

    public static void SmoothLookAt(Transform t, Vector3 target, float speed)
    {
        Vector3 dir = target - t.position;
        t.forward = Vector3.Slerp(t.forward, dir, speed);
    }
	#endregion
	
	#region Input
    public class Input
    {
        /// <summary>
        /// Gets the magnitude of an analogue stick.
        /// </summary>
        /// <returns>
        /// The analogue magnitude. (0-1) But depending on the controller might be more.
        /// </returns>
        /// <param name='xAxis'>
        /// Horizontal (X axis).
        /// </param>
        /// <param name='yAxis'>
        /// Vertical (Y axis).
        /// </param>
        public static float GetAnalogueMagnitude(float xAxis, float yAxis)
        {
            return Mathf.Sqrt(xAxis * xAxis + yAxis * yAxis);
        }

        /// <summary>
        /// Gets the magnitude of an analogue stick.
        /// </summary>
        /// <returns>
        /// The analogue magnitude. (0-1) But depending on the controller might be more.
        /// </returns>
        /// <param name='xAxis'>
        /// Horizontal (X axis).
        /// </param>
        /// <param name='yAxis'>
        /// Vertical (Y axis).
        /// </param>
        public static float GetAnalogueMagnitude(string xAxis, string yAxis)
        {
            float horizontal = UnityEngine.Input.GetAxis(xAxis);
            float vertical = UnityEngine.Input.GetAxis(yAxis);
            return GetAnalogueMagnitude(horizontal, vertical);
        }
    }
	#endregion
	
	#region Name Generation

	private static char RandomChar ()
	{
		char[] vowels = {'a', 'e', 'i', 'o', 'u', 'y'};
		char[] consonants = {'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p',
							'q', 'r', 's', 't', 'v', 'w', 'x', 'z'};
		
		int letter = UnityEngine.Random.Range(0, 26);
		
		if (letter <= 5) //Vowel
			return vowels[letter];
		else //Consonant
		{
			letter -= 6;
			return consonants[letter];
		}
	}
	
	private static bool IsCharVowel(char c)
	{
		char[] vowels = {'a', 'e', 'i', 'o', 'u', 'y'};
		foreach(char ch in vowels)
		{
			if (ch == c)
				return true;
		}
		return false;
	}
	
	
	private static string GenerateRandomName(int min, int max)
	{
		string name = "";
		
		name += RandomChar();
		name = name.ToUpper();
		
		int length = UnityEngine.Random.Range(min, max+1);
		while (name.Length < length)
		{
			char nextLetter = RandomChar();
			if (name.Length > 2)
			{
				if (IsCharVowel(nextLetter))
					name += nextLetter;	
				else if (IsCharVowel(name[name.Length-1]))
					name += nextLetter;
				else
				{
					//Retry until a vowel is generated	
				}
			}
			else
			{
				if (!IsCharVowel(name[0]))
				{
					if (IsCharVowel(nextLetter))
						name += nextLetter;
				}
			}
		}
		
		return name;
	}
	#endregion

	#region Randomization
    public class Random
    {

        /// <summary>
        /// Gives some pseudo random variation to the basevalue. The variation will be applied so that there is a 50% chance of the variation to apply negatively. 
        /// Example: RandomVariation(1f, 0.5f) returns a value between 0.75f and 1.25f
        /// </summary>
        /// <returns>
        /// A value between basevalue - 0.5f * variation and basevalue + 0.5f * variation
        /// </returns>
        /// <param name='baseValue'>
        /// Base value
        /// </param>
        /// <param name='variation'>
        /// Variation
        /// </param>
        public static float Variation(float baseValue, float variation)
        {
            return baseValue - variation * 0.5f + UnityEngine.Random.value * variation;
        }

        /// <summary>
        /// Gives some pseudo random variation to the basevalue. The variation will be applied only positively, meaning the basevalue is the lowest possible value returned.
        /// Example: RandomVariationPositive(1f, 0.5f) returns a value between 1f and 1.5f
        /// </summary>
        /// <returns>
        /// A value between basevalue and basevalue + variation
        /// </returns>
        /// <param name='baseValue'>
        /// Base value
        /// </param>
        /// <param name='variation'>
        /// Positive variation
        /// </param>
        public static float VariationPositive(float baseValue, float variation)
        {
            return baseValue + UnityEngine.Random.value * variation;
        }

        /// <summary>
        /// Gives some pseudo random variation to the basevalue. The variation will be applied only negatively, meaning the basevalue is the highest possible value returned.
        /// Example: RandomVariationNegative(1f, 0.5f) returns a value between 1f and 0.5f
        /// </summary>
        /// <returns>
        /// A value between basevalue and basevalue + variation
        /// </returns>
        /// <param name='baseValue'>
        /// Base value
        /// </param>
        /// <param name='variation'>
        /// Negative variation
        /// </param>
        public static float VariationNegative(float baseValue, float variation)
        {
            return baseValue - UnityEngine.Random.value * variation;
        }

        public static Color Color()
        {
            return new Color(UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1), 1);
        }

        public static Color SimpleColor()
        {
            Color c = new Color();
            switch (UnityEngine.Random.Range(0, 10))
            {
                case 0: c = UnityEngine.Color.red; break;
                case 1: c = UnityEngine.Color.green; break;
                case 2: c = UnityEngine.Color.blue; break;
                case 3: c = UnityEngine.Color.yellow; break;
                case 4: c = UnityEngine.Color.magenta; break;
                case 5: c = new Color(1, 0.64f, 0, 1); break; //orange
                case 6: c = new Color(1, 0, 1, 1); break; //pink
                case 7: c = new Color(0.5f, 0, 1, 1); break; //purple
                case 8: c = new Color(0, 1, 1, 1); break; //Tourqise
                case 9: c = new Color(0, 0.5f, 1, 1); break; //Soft blue
            }
            return c;
        }

        public static Vector3 Vector3(float minMagnitude, float maxMagnitude)
        {
            Vector3 v = new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100)).normalized;
            return v * UnityEngine.Random.Range(minMagnitude, maxMagnitude);
        }

        /// <summary>
        /// Generates a random name with a length between .
        /// </summary>
        /// <returns>
        /// The name.
        /// </returns>
        public static string GenerateRandomName(int min, int max)
        {
            return GenerateRandomName(min, max);
        }
    }
	#endregion

	#region Debug
    
        [SerializeField]
        private static bool onGUI = true;
        public static bool OnGUI
        {
            get
            {
                return onGUI;
            }
            set
            {
                onGUI = value;
                _UtilOnGUI.GetComponent<UtilOnGUI>().onGUI = onGUI;
            }
        }
        private static bool onGUIObject = true;
        public static bool OnGUIObject
        {
            get
            {
                return onGUIObject;
            }
            set
            {
                onGUIObject = value;
                _UtilOnGUI.GetComponent<UtilOnGUI>().onGUIObjects = onGUIObject;
            }
        }
        private static GameObject _utilOnGUI;
        public static GameObject _UtilOnGUI
        {
            get
            {
                if (_utilOnGUI == null)
                {
                    GameObject g = GameObject.Find("UtilGUI_Object");
                    if (g == null || g.tag != "EditorOnly")
                    {
                        _utilOnGUI = new GameObject();
                        _utilOnGUI.name = "UtilGUI_Object";
                        _utilOnGUI.tag = "EditorOnly";
                        _utilOnGUI.isStatic = true;
                        _utilOnGUI.AddComponent<UtilOnGUI>();
                    }
                    else
                    {
                        _utilOnGUI = g;
                    }
                }
                return _utilOnGUI;
            }
        }

    public class Debug
    {
        public class _DebugLine
        {
            public GameObject objectRef;
            public string line;
            public float expirationTime, creationTime;
            public Color color;
            public object value;

            public _DebugLine(string _line, float _expirationTime, GameObject _objectRef, Color _color, object _value)
            {
                line = _line;
                expirationTime = _expirationTime + Time.time;
                if (_objectRef)
                    objectRef = _objectRef;
                else
                    objectRef = null;
                color = _color;
                creationTime = Time.time;
                value = _value;
            }

            public _DebugLine(string _line, float _expirationTime, GameObject _objectRef, Color _color)
            {
                line = _line;
                expirationTime = _expirationTime + Time.time;
                if (_objectRef)
                    objectRef = _objectRef;
                else
                    objectRef = null;
                color = _color;
                creationTime = Time.time;
                value = null;
            }
        }

        public static List<_DebugLine> DebugLines = new List<_DebugLine>();
        public static List<_DebugLine> DebugLinesPersistent = new List<_DebugLine>();

        public static void Log(string line)
        {
            if (OnGUI)
            {
                Log(line, float.MaxValue);
            }
        }

        public static void Log(string line, float timeShowing)
        {
            if (onGUI)
            {
                DebugLines.Add(new _DebugLine(line, timeShowing, null, Color.black));
            }
        }

        public static void Log(string line, float timeShowing, Color textColor)
        {
            DebugLines.Add(new _DebugLine(line, timeShowing, null, textColor));
        }

        public static void DebugLog(string line, Color textColor)
        {
            DebugLines.Add(new _DebugLine(line, float.MaxValue, null, textColor));
        }

        //TODO: we should be able to draw some lines with more than one value 
        public static void Persistent(string line, object value)
        {
            if (onGUI)
            {
                int index = -1;

                for (int i = 0; i < DebugLinesPersistent.Count; i++)
                {
                    if (DebugLinesPersistent[i].line.ToLower().Equals(line.ToLower()))
                    {
                        index = i;
                    }
                }
                if (index != -1)
                {
                    DebugLinesPersistent.RemoveAt(index);
                }
                DebugLinesPersistent.Add(new _DebugLine(line, -1f, null, Color.black, value));
            }
        }

        public static void LogOnObject(string line, object value, GameObject gameObject)
        {
            if (onGUIObject)
            {
                _UtilOnGUI.GetComponent<UtilOnGUI>().AddStringAtPosition(gameObject, line, value);
            }
        }

        public static void Graph(string name, float data)
        {
            UtilGraph.AddPlotData(name, data);
        }

        //public static void AddDebugLinePersistent();
        //public static void AddDebugLine();
        //public static void AddDebugLinePersistent();
        //public static void AddDebugLineOnScreen();
    }
	#endregion
	
	#region Math
	public static int Difference(int var1, int var2)
	{
		if (var1 < var2)
			return var2-var1;
		else
			return var1-var2;
	}
	
	public static float Difference(float var1, float var2)
	{
		if (var1 < var2)
			return var2-var1;
		else
			return var1-var2;
	}
	#endregion

}

    #region Transform
public static class TransformExtension
{
    public static void SmoothLookAt(this Transform t, Vector3 target, float lerpValue)
    {
        Vector3 dir = target - t.position;
        t.forward = Vector3.Slerp(t.forward, dir, lerpValue);
    }

    public static void Reset(this Transform t)
    {
        t.position = new Vector3();
        t.rotation = Quaternion.identity;
    }
}
    #endregion

    #region Vector2
    /*public static class Vector2Extension
    {
        public static Vector2 X(this Vector3 vector, float x)
        {
            return new Vector2(x, vector.y);
        }

        public static Vector2 Y(this Vector3 vector, float y)
        {
            return new Vector2(vector.x, y);
        }
    }*/
    #endregion

    #region Vector3
    public static class Vector3Extension
    {
        public static Vector3 X(this Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }

        public static Vector3 Y(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        public static Vector3 Z(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        public static Vector3 XY(this Vector3 vector, float x, float y)
        {
            return new Vector3(x, y, vector.z);
        }

        public static Vector3 YZ(this Vector3 vector, float y, float z)
        {
            return new Vector3(vector.x, y, z);
        }
    }
    #endregion

    #region Quaternion
    public static class QuaternionExtension
    {
        public static Quaternion X(this Quaternion quat, float x)
        {
            return new Quaternion(x, quat.y, quat.z, quat.w);
        }

        public static Quaternion Y(this Quaternion quat, float y)
        {
            return new Quaternion(quat.x, y, quat.z, quat.w);
        }

        public static Quaternion Z(this Quaternion quat, float z)
        {
            return new Quaternion(quat.x, quat.y, z, quat.w);
        }

        public static Quaternion W(this Quaternion quat, float w)
        {
            return new Quaternion(quat.x, quat.y, quat.z, w);
        }
    }
    #endregion

        #region Color
    public static class ColorExtension
    {
        public static Color R(this Color color, float r)
        {
            return new Color(r, color.g, color.b, color.a);
        }

        public static Color G(this Color color, float g)
        {
            return new Color(color.r, g, color.b, color.a);
        }

        public static Color B(this Color color, float b)
        {
            return new Color(color.r, color.g, b, color.a);
        }

        public static Color A(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }

        public static Color RG(this Color color, float r, float g)
        {
            return new Color(r, g, color.b, color.a);
        }

        public static Color RB(this Color color, float r, float b)
        {
            return new Color(r, color.g, b, color.a);
        }

        public static Color RA(this Color color, float r, float a)
        {
            return new Color(r, color.g, color.b, a);
        }

        public static Color GB(this Color color, float g, float b)
        {
            return new Color(color.r, g, b, color.a);
        }

        public static Color GA(this Color color, float g, float a)
        {
            return new Color(color.r, g, color.b, a);
        }

        public static Color BA(this Color color, float b, float a)
        {
            return new Color(color.r, color.g, b, a);
        }

        public static Color RGB(this Color color, float r, float g, float b)
        {
            return new Color(r, g, b, color.a);
        }
    }
    #endregion

	#region Tuple
namespace UtilTuple
{
	public class Tuple<T1, T2>
	{
		public T1 object1;
		public T2 object2;
		
		public Tuple(T1 t1, T2 t2)
		{
			object1 = t1;
			object2 = t2;
		}
		
		public T1 GetFirst()
		{
			return object1;
		}
		
		public T2 GetSecond()
		{
			return object2;
		}
	}
	
	public class Tuple<T1, T2, T3>
	{
		public T1 object1;
		public T2 object2;
		public T3 object3;
		
		public Tuple(T1 t1, T2 t2, T3 t3)
		{
			object1 = t1;
			object2 = t2;
			object3 = t3;
		}
		
		public T1 GetFirst()
		{
			return object1;
		}
		
		public T2 GetSecond()
		{
			return object2;
		}
		
		public T3 GetThird()
		{
			return object3;
		}
	}
	
	public class Tuple<T1, T2, T3, T4>
	{
		public T1 object1;
		public T2 object2;
		public T3 object3;
		public T4 object4;
		
		public Tuple(T1 t1, T2 t2, T3 t3, T4 t4)
		{
			object1 = t1;
			object2 = t2;
			object3 = t3;
			object4 = t4;
		}
		
		public T1 GetFirst()
		{
			return object1;
		}
		
		public T2 GetSecond()
		{
			return object2;
		}
		
		public T3 GetThird()
		{
			return object3;
		}
		
		public T4 GetFourth()
		{
			return object4;
		}
	}
	
	public class Tuple<T1, T2, T3, T4, T5>
	{
		public T1 object1;
		public T2 object2;
		public T3 object3;
		public T4 object4;
		public T5 object5;
		
		public Tuple(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
		{
			object1 = t1;
			object2 = t2;
			object3 = t3;
			object4 = t4;
			object5 = t5;
		}
	}
	
	public class Tuple<T1, T2, T3, T4, T5, T6>
	{
		public T1 object1;
		public T2 object2;
		public T3 object3;
		public T4 object4;
		public T5 object5;
		public T6 object6;
		
		public Tuple(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
		{
			object1 = t1;
			object2 = t2;
			object3 = t3;
			object4 = t4;
			object5 = t5;
			object6 = t6;
		}
		
	}
	
	public class Tuple<T1, T2, T3, T4, T5, T6, T7>
	{
		public T1 object1;
		public T2 object2;
		public T3 object3;
		public T4 object4;
		public T5 object5;
		public T6 object6;
		public T7 object7;
		
		public Tuple(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
		{
			object1 = t1;
			object2 = t2;
			object3 = t3;
			object4 = t4;
			object5 = t5;
			object6 = t6;
			object7 = t7;
		}
	}
	
	public class Tuple<T1, T2, T3, T4, T5, T6, T7, T8>
	{
		public T1 object1;
		public T2 object2;
		public T3 object3;
		public T4 object4;
		public T5 object5;
		public T6 object6;
		public T7 object7;
		public T8 object8;
		
		public Tuple(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
		{
			object1 = t1;
			object2 = t2;
			object3 = t3;
			object4 = t4;
			object5 = t5;
			object6 = t6;
			object7 = t7;
			object8 = t8;
		}
	}
}
	#endregion
