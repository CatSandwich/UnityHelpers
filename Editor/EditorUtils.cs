using System;
using System.Linq;
using UnityEngine;

public static class EditorUtils
{
    public static readonly Vector2[] Adjacent = new Vector2[8]
    {
        Vector2.up, 
        Vector2.up   + Vector2.right, 
                       Vector2.right, 
        Vector2.down + Vector2.right, 
        Vector2.down,
        Vector2.down + Vector2.left, 
                       Vector2.left, 
        Vector2.up   + Vector2.left
    };
    
    private static Shader Shader => _shader ??= Shader.Find("Unlit/Grid");
    private static Shader _shader;
    private static Material Material => _material ??= new Material(Shader);
    private static Material _material;

    public const float TITLE_HEIGHT = 21f;
    private static Func<float, float, float> Total = (e1, e2) => e1 + e2;
    public static Rect[,] Split(this Rect rect, int cols, int rows, float[] colSizes = null, float[] rowSizes = null)
    {
        var grid = new Rect[cols, rows];

        for (var x = 0; x < cols; x++)
        {
            var xPosRelative = colSizes?.Take(x).DefaultIfEmpty(0f).Aggregate(Total) * rect.width
                ?? rect.width / cols * x;
            
            var width = colSizes?[x] * rect.width
                ?? rect.width / cols;    
            
            for (var y = 0; y < rows; y++)
            {
                var yPosRelative = rowSizes?.Take(y).DefaultIfEmpty(0).Aggregate(Total)
                    ?? rect.height / rows * y;

                var height = rowSizes?[y]
                    ?? rect.height / rows;
                
                grid[x, y] = new Rect(
                    rect.x + xPosRelative,
                    rect.y + yPosRelative,
                    width,
                    height
                );
            }
        }

        return grid;
    }
    
    public static void DrawArrow(Color color, Vector3 start, Vector3 end)
    {
        const int len = 10;
        var up = new Vector3(0f, len, 0f);
        var left = new Vector3(-len, 0f, 0f);
        const float rads = 0.785398f; //45 degrees
        
        DrawLine(color, start, end);

        var xDiff = end.x - start.x;
        var yDiff = end.y - start.y;
        var angle = (float)Math.Atan2(yDiff, xDiff) * -1f;

        GL.Begin(GL.TRIANGLES);
        GL.Color(color);
        for (var i = 1; i <= 5; i++)
        {
            var point = Vector3.Lerp(start, end, i / 6f);
            GL.Vertex(point);
            GL.Vertex(point + Mathf.Sin(angle + rads) * up + Mathf.Cos(angle + rads) * left);
            GL.Vertex(point + Mathf.Sin(angle - rads) * up + Mathf.Cos(angle - rads) * left);
        }
        GL.End();
    }

    public static void DrawLine(Color color, Vector3 start, Vector3 end)
    {
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(start);
        GL.Vertex(end);
        GL.End();
    }
}
