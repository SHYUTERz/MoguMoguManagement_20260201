using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(GridLayoutGroup))]
public class ResponsiveGridScale_2or3 : MonoBehaviour
{
    [SerializeField] private RectTransform viewport;   // Scroll View / Viewport
    [SerializeField] private float widthToUse3Columns = 900f; // これ以上なら3列
    [SerializeField] private int minColumns = 2;
    [SerializeField] private int maxColumns = 3;

    // セルサイズは固定（比率 250:400）
    [SerializeField] private Vector2 baseCellSize = new Vector2(250f, 400f);

    private RectTransform content;
    private GridLayoutGroup grid;

    void Awake()
    {
        content = GetComponent<RectTransform>();
        grid = GetComponent<GridLayoutGroup>();

        if (viewport == null)
        {
            var sr = GetComponentInParent<ScrollRect>();
            if (sr != null) viewport = sr.viewport;
        }

        // ここで固定しておく（子を崩さない）
        grid.cellSize = baseCellSize;

        Apply();
    }

    void OnEnable() => Apply();
    void OnRectTransformDimensionsChange() => Apply();

    void Apply()
    {
        if (viewport == null || grid == null) return;

        float vw = viewport.rect.width;

        // 列数（基本2、広いときだけ3）
        int columns = (vw >= widthToUse3Columns) ? maxColumns : minColumns;
        columns = Mathf.Clamp(columns, minColumns, maxColumns);

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;

        // 「スケール前」の必要横幅を計算
        float requiredW =
            grid.padding.left + grid.padding.right
            + columns * baseCellSize.x
            + (columns - 1) * grid.spacing.x;

        // Viewportにピッタリ合わせるスケール（拡大も縮小もOK）
        float scale = (requiredW <= 0f) ? 1f : (vw / requiredW);

        // uniform scale（縦横比を壊さない）
        content.localScale = new Vector3(scale, scale, 1f);

        // 縦スクロール範囲がズレないように Contentの高さも更新
        int childCount = content.childCount;
        int rows = Mathf.CeilToInt(childCount / (float)columns);
        rows = Mathf.Max(rows, 1);

        float requiredH =
            grid.padding.top + grid.padding.bottom
            + rows * baseCellSize.y
            + (rows - 1) * grid.spacing.y;

        // 見た目の高さ = requiredH * scale
        // ContentSizeFitterと喧嘩しないよう sizeDelta を直に設定
        var size = content.sizeDelta;
        size.y = requiredH * scale;
        content.sizeDelta = size;
    }
}