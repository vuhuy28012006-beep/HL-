using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChapterBackgroundRepeater : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform backgroundLayer;

    [Header("Tile Settings")]
    [SerializeField] private float tileWidth = 1920f;
    [SerializeField] private float tileHeight = 1080f;

    private IEnumerator Start()
    {
        // Chờ LevelListPopulator tính xong chiều rộng Content.
        yield return null;

        BuildBackground();
    }

    public void BuildBackground()
    {
        if (content == null || backgroundLayer == null)
        {
            Debug.LogError(
                "ChapterBackgroundRepeater: Chưa gán Content hoặc BackgroundLayer!",
                this
            );
            return;
        }

        ChapterData chapter = ChapterSession.SelectedChapter;

        if (chapter == null)
        {
            Debug.LogError(
                "ChapterBackgroundRepeater: Chưa chọn Chapter!",
                this
            );
            return;
        }

        if (chapter.levelSelectBackground == null)
        {
            Debug.LogError(
                "Chapter chưa có Level Select Background!",
                chapter
            );
            return;
        }

        ClearOldTiles();

        float contentWidth = Mathf.Max(
            content.rect.width,
            tileWidth
        );

        int tileCount =
            Mathf.CeilToInt(contentWidth / tileWidth) + 1;

        for (int i = 0; i < tileCount; i++)
        {
            GameObject tileObject =
                new GameObject(
                    "BackgroundTile_" + (i + 1),
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(Image)
                );

            RectTransform tile =
                tileObject.GetComponent<RectTransform>();

            tile.SetParent(backgroundLayer, false);

            tile.anchorMin = new Vector2(0f, 0.5f);
            tile.anchorMax = new Vector2(0f, 0.5f);
            tile.pivot = new Vector2(0.5f, 0.5f);

            tile.sizeDelta =
                new Vector2(tileWidth + 4f, tileHeight);

            tile.anchoredPosition = new Vector2(
                i * tileWidth + tileWidth * 0.5f,
                0f
            );

            // Lật xen kẽ để hai mép tiếp giáp giống nhau.
            tile.localScale = new Vector3(
                i % 2 == 0 ? 1f : -1f,
                1f,
                1f
            );

            Image image = tileObject.GetComponent<Image>();

            image.sprite = chapter.levelSelectBackground;
            image.color = Color.white;
            image.raycastTarget = false;
            image.preserveAspect = false;
        }

        backgroundLayer.SetAsFirstSibling();
    }

    private void ClearOldTiles()
    {
        for (
            int i = backgroundLayer.childCount - 1;
            i >= 0;
            i--
        )
        {
            Destroy(
                backgroundLayer.GetChild(i).gameObject
            );
        }
    }
}