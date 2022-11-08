using UnityEngine;

public class Helpers
{
    private static Camera _mainCamera;
    
    public static Camera Camera => _mainCamera ? _mainCamera : _mainCamera = Camera.main;

    /// <summary>
    /// Rotates a vector by a set amount of radians
    /// </summary>
    /// <param name="vector">The vector to rotate</param>
    /// <param name="radians">The amount of radians to rotate the vector by</param>
    public static Vector2 RotateVector(Vector2 vector, float radians)
    {
        var x = vector.x;
        var y = vector.y;
        var sine = Mathf.Sin(radians);
        var cosine = Mathf.Cos(radians);
            
        return new Vector2(x * cosine - y *sine, x * sine + y * cosine);
    }

    public static float AngleFloat(Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x);
    }
}