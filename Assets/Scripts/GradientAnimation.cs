using UnityEngine;
using UnityEngine.UI;

public class GradientAnimation : MonoBehaviour
{
    public Material mat;
    public float animSpeed = 1f;
    public Color[] colors;

    private static Color topLeftColor;
    private static Color topRightColor;
    private static Color bottomLeftColor;
    private static Color bottomRightColor;

    private Color targetTopLeftColor;
    private Color targetTopRightColor;
    private Color targetBottomLeftColor;
    private Color targetBottomRightColor;

    private static float transitionProgress = 0f;
    private int currentColorIndex = 0;
    private static bool IsColorsSet = false;

    private void Start()
    {
        SetTargetColors();
        transitionProgress = 0f;
        if (!IsColorsSet)
        {
            SetCurrentColors();
            IsColorsSet = true;
        }
    }

    private void Update()
    {
        transitionProgress += animSpeed * Time.deltaTime;

        topLeftColor = Color.Lerp(topLeftColor, targetTopLeftColor, transitionProgress);
        topRightColor = Color.Lerp(topRightColor, targetTopRightColor, transitionProgress);
        bottomLeftColor = Color.Lerp(bottomLeftColor, targetBottomLeftColor, transitionProgress);
        bottomRightColor = Color.Lerp(bottomRightColor, targetBottomRightColor, transitionProgress);

        mat.SetColor("_TopLeftColor", topLeftColor);
        mat.SetColor("_TopRightColor", topRightColor);
        mat.SetColor("_BottomLeftColor", bottomLeftColor);
        mat.SetColor("_BottomRightColor", bottomRightColor);

        if (transitionProgress >= 1f)
        {
            transitionProgress = 0f;
            currentColorIndex = (currentColorIndex + 1) % colors.Length;
            SetTargetColors();
        }
        SetCurrentColors();
    }

    private void SetCurrentColors()
    {
        topLeftColor = colors[currentColorIndex];
        topRightColor = colors[(currentColorIndex + 1) % colors.Length];
        bottomLeftColor = colors[(currentColorIndex + 2) % colors.Length];
        bottomRightColor = colors[(currentColorIndex + 3) % colors.Length];
    }

    private void SetTargetColors()
    {
        targetTopLeftColor = colors[(currentColorIndex + 1) % colors.Length];
        targetTopRightColor = colors[(currentColorIndex + 2) % colors.Length];
        targetBottomLeftColor = colors[(currentColorIndex + 3) % colors.Length];
        targetBottomRightColor = colors[(currentColorIndex + 4) % colors.Length];
    }
}
