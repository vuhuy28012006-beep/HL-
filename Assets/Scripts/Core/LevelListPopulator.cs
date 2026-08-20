using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelListPopulator : MonoBehaviour
{
    [Header("Scroll Map")]
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform backgroundLayer;
    [SerializeField] private RectTransform pathLayer;
    [SerializeField] private RectTransform levelLayer;

    [Header("Prefabs")]
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private GameObject pathSegmentPrefab;

    [Header("Map Size")]
    [SerializeField] private float minimumContentWidth = 1920f;
    [SerializeField] private float mapHeight = 1080f;
    [SerializeField] private float backgroundTileWidth = 1920f;

    [Header("Level Positions")]
    [SerializeField] private float horizontalSpacing = 400f;
    [SerializeField] private float sidePadding = 280f;
    [SerializeField] private float lowY = -170f;
    [SerializeField] private float highY = 150f;

    [Header("Path")]
    [SerializeField] private float pathThickness = 18f;

    [Header("Chapter UI - Optional")]
    [SerializeField] private TMP_Text chapterNameText;
    [SerializeField] private TMP_Text chapterDescriptionText;
    [Header("Chapter Background")]
    [SerializeField] private Image chapterBackgroundImage;

    private void Start()
    {
        ChapterData chapter = ChapterSession.SelectedChapter;

        if (chapter == null)
        {
            Debug.LogError(
                "LevelListPopulator: chua co Chapter nao duoc chon. " +
                "Hay vao scene Map va bam mot chapter truoc."
            );
            return;
        }

        if (content == null ||
            backgroundLayer == null ||
            pathLayer == null ||
            levelLayer == null)
        {
            Debug.LogError(
                "LevelListPopulator: chua gan du Content hoac cac Layer.",
                this
            );
            return;
        }

        if (levelButtonPrefab == null)
        {
            Debug.LogError(
                "LevelListPopulator: chua gan Level Button Prefab.",
                this
            );
            return;
        }

        if (chapterNameText != null)
            chapterNameText.text = chapter.chapterName;

        if (chapterDescriptionText != null)
            chapterDescriptionText.text = chapter.description;

        BuildMap(chapter);
        if (chapterBackgroundImage != null &&
            chapter.levelSelectBackground != null)
        {
            chapterBackgroundImage.sprite =
                chapter.levelSelectBackground;

            chapterBackgroundImage.color = Color.white;
        }
    }

    private void BuildMap(ChapterData chapter)
    {
        ClearChildren(backgroundLayer);
        ClearChildren(pathLayer);
        ClearChildren(levelLayer);

        int levelCount = chapter.levels != null
            ? chapter.levels.Count
            : 0;

        float routeWidth =
            Mathf.Max(0, levelCount - 1) * horizontalSpacing;

        float requiredWidth =
            sidePadding * 2f + routeWidth;

        float contentWidth =
            Mathf.Max(minimumContentWidth, requiredWidth);

        content.sizeDelta = new Vector2(
            contentWidth,
            0f
        );

        BuildBackground(
            chapter.levelSelectBackground,
            contentWidth
        );

        List<Vector2> levelPositions =
            CalculateLevelPositions(
                levelCount,
                contentWidth,
                routeWidth
            );

        BuildPaths(levelPositions);
        BuildLevelButtons(chapter, levelPositions);
    }

    private List<Vector2> CalculateLevelPositions(
        int levelCount,
        float contentWidth,
        float routeWidth
    )
    {
        List<Vector2> positions =
            new List<Vector2>();

        float firstX =
            (contentWidth - routeWidth) * 0.5f;

        for (int i = 0; i < levelCount; i++)
        {
            float x =
                firstX + i * horizontalSpacing;

            float y =
                i % 2 == 0 ? lowY : highY;

            positions.Add(new Vector2(x, y));
        }

        return positions;
    }

    private void BuildBackground(
        Sprite backgroundSprite,
        float contentWidth
    )
    {
        if (backgroundSprite == null)
        {
            Debug.LogWarning(
                "ChapterData chua co Level Select Background."
            );
            return;
        }

        int tileCount = Mathf.CeilToInt(
            contentWidth / backgroundTileWidth
        );

        for (int i = 0; i < tileCount; i++)
        {
            GameObject tile = new GameObject(
                "BackgroundTile_" + (i + 1),
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image)
            );

            tile.transform.SetParent(
                backgroundLayer,
                false
            );

            RectTransform tileRect =
                tile.GetComponent<RectTransform>();

            tileRect.anchorMin =
                new Vector2(0f, 0.5f);

            tileRect.anchorMax =
                new Vector2(0f, 0.5f);

            tileRect.pivot =
                new Vector2(0.5f, 0.5f);

            tileRect.anchoredPosition =
                new Vector2(
                    (i + 0.5f) * backgroundTileWidth,
                    0f
                );

            tileRect.sizeDelta =
                new Vector2(
                    backgroundTileWidth,
                    mapHeight
                );

            Image image = tile.GetComponent<Image>();

            image.sprite = backgroundSprite;
            image.color = Color.white;
            image.type = Image.Type.Simple;
            image.preserveAspect = false;
            image.raycastTarget = false;

            // Lat nguoc xen ke de cho noi giua 2 tile bot ro.
            if (i % 2 == 1)
            {
                tileRect.localScale =
                    new Vector3(-1f, 1f, 1f);
            }
        }
    }

    private void BuildLevelButtons(
        ChapterData chapter,
        List<Vector2> positions
    )
    {
        for (int i = 0; i < positions.Count; i++)
        {
            LevelData levelData =
                chapter.levels[i];

            if (levelData == null)
                continue;

            GameObject buttonObject =
                Instantiate(
                    levelButtonPrefab,
                    levelLayer
                );

            buttonObject.name =
                "LevelButton_" + levelData.levelNumber;

            RectTransform buttonRect =
                buttonObject.GetComponent<RectTransform>();

            buttonRect.anchorMin =
                new Vector2(0f, 0.5f);

            buttonRect.anchorMax =
                new Vector2(0f, 0.5f);

            buttonRect.pivot =
                new Vector2(0.5f, 0.5f);

            buttonRect.anchoredPosition =
                positions[i];

            buttonRect.localScale =
                Vector3.one;

            LevelSelectButton levelButton =
                buttonObject.GetComponent<LevelSelectButton>();

            if (levelButton != null)
            {
                levelButton.Setup(
                    levelData,
                    chapter.chapterNumber
                );
            }
            else
            {
                Debug.LogError(
                    "Level Button Prefab khong co " +
                    "LevelSelectButton component.",
                    buttonObject
                );
            }
        }
    }

    private void BuildPaths(
        List<Vector2> positions
    )
    {
        if (pathSegmentPrefab == null)
        {
            Debug.LogWarning(
                "Chua gan Path Segment Prefab. " +
                "Level van duoc tao nhung chua co duong noi."
            );
            return;
        }

        /*for (int i = 0; i < positions.Count - 1; i++)
        {
            CreatePathSegment(
                positions[i],
                positions[i + 1],
                i + 1
            );
        }*/
    }

    private void CreatePathSegment(
        Vector2 from,
        Vector2 to,
        int segmentNumber
    )
    {
        GameObject segmentObject =
            Instantiate(
                pathSegmentPrefab,
                pathLayer
            );

        segmentObject.name =
            "PathSegment_" + segmentNumber;

        RectTransform segmentRect =
            segmentObject.GetComponent<RectTransform>();

        Vector2 direction = to - from;
        float distance = direction.magnitude;

        segmentRect.anchorMin =
            new Vector2(0f, 0.5f);

        segmentRect.anchorMax =
            new Vector2(0f, 0.5f);

        segmentRect.pivot =
            new Vector2(0.5f, 0.5f);

        segmentRect.anchoredPosition =
            (from + to) * 0.5f;

        segmentRect.sizeDelta =
            new Vector2(
                distance,
                pathThickness
            );

        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;

        segmentRect.localRotation =
            Quaternion.Euler(0f, 0f, angle);

        segmentRect.localScale =
            Vector3.one;

        Image image =
            segmentObject.GetComponent<Image>();

        if (image != null)
            image.raycastTarget = false;
    }

    private void ClearChildren(
        Transform parent
    )
    {
        for (int i = parent.childCount - 1;
             i >= 0;
             i--)
        {
            Destroy(
                parent.GetChild(i).gameObject
            );
        }
    }
}