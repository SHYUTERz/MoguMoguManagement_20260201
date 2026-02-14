using UnityEngine;
using UnityEngine.EventSystems;

public class ManualVerticalScroll : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] private RectTransform viewport; // 受付エリア（自分でもOK）
    [SerializeField] private RectTransform content;  // 動かす対象

    [SerializeField] private float dragSpeed = 1.0f; // 1でOK。速ければ2とか
    private Vector2 startContentPos;

    void Reset()
    {
        viewport = transform as RectTransform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (viewport == null) viewport = transform as RectTransform;
        startContentPos = content.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (viewport == null || content == null) return;

        // 画面上のドラッグ量（pixel）をそのままUI座標へ
        float deltaY = eventData.delta.y * dragSpeed;

        Vector2 pos = content.anchoredPosition;
        pos.y += deltaY; // 上ドラッグで上に戻す/下へ送るが好みなら符号逆でもOK
        content.anchoredPosition = ClampY(pos);
    }

    private Vector2 ClampY(Vector2 pos)
    {
        // ContentがViewportより小さいなら動かさない
        float viewportH = viewport.rect.height;
        float contentH = content.rect.height;

        float maxY = Mathf.Max(0f, contentH - viewportH);
        pos.y = Mathf.Clamp(pos.y, 0f, maxY);
        pos.x = content.anchoredPosition.x; // 横は固定
        return pos;
    }
}