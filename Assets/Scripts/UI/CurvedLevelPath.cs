using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CurvedLevelPath : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField] private RectTransform pathLayer;
    [SerializeField] private RectTransform levelLayer;

    [Header("Path Dot")]
    [SerializeField] private GameObject pathDotPrefab;

    [Header("Path Settings")]
    [SerializeField] private Vector2 dotSize =
        new Vector2(48f, 26f);

    [SerializeField, Min(10f)]
    private float dotSpacing = 42f;

    [SerializeField, Min(0f)]
    private float curveHeight = 120f;

    private IEnumerator Start()
    {
        // Chờ LevelListPopulator sinh xong các level button.
        yield return null;

        RebuildPath();
    }

    public void RebuildPath()
    {
        if (pathLayer == null ||
            levelLayer == null ||
            pathDotPrefab == null)
        {
            Debug.LogError(
                "CurvedLevelPath: Chưa gán đủ References!",
                this
            );

            return;
        }

        ClearOldPath();

        int levelCount = levelLayer.childCount;

        if (levelCount < 2)
            return;

        for (int i = 0; i < levelCount - 1; i++)
        {
            RectTransform currentLevel =
                levelLayer.GetChild(i) as RectTransform;

            RectTransform nextLevel =
                levelLayer.GetChild(i + 1) as RectTransform;

            if (currentLevel == null || nextLevel == null)
                continue;

            DrawCurve(
                currentLevel.anchoredPosition,
                nextLevel.anchoredPosition,
                i
            );
        }

        // Bảo đảm đường nằm dưới các nút.
        pathLayer.SetSiblingIndex(1);
        levelLayer.SetAsLastSibling();
    }

    private void ClearOldPath()
    {
        for (int i = pathLayer.childCount - 1; i >= 0; i--)
        {
            Destroy(pathLayer.GetChild(i).gameObject);
        }
    }

    private void DrawCurve(
        Vector2 start,
        Vector2 end,
        int segmentIndex
    )
    {
        // Điểm điều khiển của đường Bézier.
        Vector2 control = (start + end) * 0.5f;

        float direction =
            segmentIndex % 2 == 0 ? 1f : -1f;

        control.y += curveHeight * direction;

        float estimatedLength =
            Vector2.Distance(start, control) +
            Vector2.Distance(control, end);

        int dotCount = Mathf.Max(
            3,
            Mathf.CeilToInt(
                estimatedLength / dotSpacing
            )
        );

        // Bỏ qua hai đầu vì nút Level sẽ che chúng.
        for (int i = 1; i < dotCount; i++)
        {
            float t = i / (float)dotCount;

            Vector2 position =
                CalculateQuadraticBezier(
                    start,
                    control,
                    end,
                    t
                );

            Vector2 tangent =
                CalculateQuadraticTangent(
                    start,
                    control,
                    end,
                    t
                );

            GameObject dot = Instantiate(
                pathDotPrefab,
                pathLayer,
                false
            );

            dot.name =
                $"PathDot_{segmentIndex + 1}_{i}";

            RectTransform rect =
                dot.GetComponent<RectTransform>();

            rect.anchorMin =
                new Vector2(0f, 0.5f);

            rect.anchorMax =
                new Vector2(0f, 0.5f);

            rect.pivot =
                new Vector2(0.5f, 0.5f);

            rect.sizeDelta = dotSize;
            rect.anchoredPosition = position;
            rect.localScale = Vector3.one;

            float angle = Mathf.Atan2(
                tangent.y,
                tangent.x
            ) * Mathf.Rad2Deg;

            rect.localRotation =
                Quaternion.Euler(0f, 0f, angle);

            Image image = dot.GetComponent<Image>();

            if (image != null)
                image.raycastTarget = false;
        }
    }

    private Vector2 CalculateQuadraticBezier(
        Vector2 start,
        Vector2 control,
        Vector2 end,
        float t
    )
    {
        float oneMinusT = 1f - t;

        return
            oneMinusT * oneMinusT * start +
            2f * oneMinusT * t * control +
            t * t * end;
    }

    private Vector2 CalculateQuadraticTangent(
        Vector2 start,
        Vector2 control,
        Vector2 end,
        float t
    )
    {
        return
            2f * (1f - t) * (control - start) +
            2f * t * (end - control);
    }
}