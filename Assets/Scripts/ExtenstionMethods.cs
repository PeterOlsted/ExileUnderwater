using UnityEngine;
using System.Collections;

public static class ExtenstionMethods{

    public static Vector3 Mul(this Vector3 v1, Vector3 v2)
    {
        return Vector3.Scale(v1, v2);
    }
}
