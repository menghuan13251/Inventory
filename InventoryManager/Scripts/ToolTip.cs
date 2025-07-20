using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{

    private Text toolTipText;
    private Text contentText;
    private CanvasGroup canvasGroup;

    private float targetAlpha = 0;

    public float smoothing = 1;
    private RectTransform rectTransform; // 【新增】引用自身的 RectTransform
    void Start()
    {
        toolTipText = GetComponent<Text>();
        contentText = transform.Find("Content").GetComponent<Text>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>(); // 【新增】获取组件
    }
    public void AdjustPivot(Vector2 mousePosition)
    {
        float pivotX = mousePosition.x / Screen.width;
        float pivotY = mousePosition.y / Screen.height;

        // 如果鼠标在屏幕右侧，Pivot.x 设置为1；在左侧，设置为0。
        // 如果鼠标在屏幕上方，Pivot.y 设置为1；在下方，设置为0。
        // 这样可以让提示框总是在屏幕内展开。
        rectTransform.pivot = new Vector2(pivotX < 0.5f ? 0 : 1, pivotY < 0.5f ? 0 : 1);
    }
    void Update()
    {
        if (canvasGroup.alpha != targetAlpha)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, smoothing * Time.deltaTime);
            if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < 0.01f)
            {
                canvasGroup.alpha = targetAlpha;
            }
        }
    }

    public void Show(string text)
    {
        toolTipText.text = text;
        contentText.text = text;
        targetAlpha = 1;
    }
    public void Hide()
    {
        targetAlpha = 0;
    }
    public void SetLocalPotion(Vector3 position)
    {
        transform.localPosition = position;
    }

}