using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace PrettyRarities.Core;

public static partial class Utilities
{
    public static Func<T, V> GetFieldAccessor<T, V>(string fieldName) {
        var param = Expression.Parameter(typeof(T), "arg");
        var member = Expression.Field(param, fieldName);
        var lambda = Expression.Lambda(typeof(Func<T, V>), member, param);

        return lambda.Compile() as Func<T, V>;
    }

    public static Color MulticolorLerp(float increment, params Color[] colors)
    {
        increment %= 0.999f;
        int currentColorIndex = (int)(increment * colors.Length);
        Color currentColor = colors[currentColorIndex];
        Color nextColor = colors[(currentColorIndex + 1) % colors.Length];
        return Color.Lerp(currentColor, nextColor, increment * colors.Length % 1f);
    }

    public static Vector4 ToVector4(this Rectangle rectangle) => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

    public static void CreatePerspectiveMatrixes(out Matrix view, out Matrix projection) {
        int height = Main.instance.GraphicsDevice.Viewport.Height;

        Vector2 zoom = Main.GameViewMatrix.Zoom;
        Matrix zoomScaleMatrix = Matrix.CreateScale(zoom.X, zoom.Y, 1f);

        // Get a matrix that aims towards the Z axis (these calculations are relative to a 2D world).
        view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up);

        // Offset the matrix to the appropriate position.
        view *= Matrix.CreateTranslation(0f, -height, 0f);

        // Flip the matrix around 180 degrees.
        view *= Matrix.CreateRotationZ(MathHelper.Pi);

        if (Main.LocalPlayer.gravDir == -1f)
            view *= Matrix.CreateScale(1f, -1f, 1f) * Matrix.CreateTranslation(0f, height, 0f);

        // Account for the current zoom.
        view *= zoomScaleMatrix;

        projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth * zoom.X, 0f, Main.screenHeight * zoom.Y, 0f, 1f) * zoomScaleMatrix;
    }
}
