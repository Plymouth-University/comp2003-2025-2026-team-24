using UnityEngine;
using UnityEngine.UI;

public class PreviewUIController : MonoBehaviour
{
    public static PreviewUIController Instance;

    [SerializeField] private Image previewImage;

    private float rotation = 0f;

    private void Awake()
    {
        Instance = this;

        previewImage.enabled = false;
        previewImage.sprite = null;
    }

    public void SetPreview(GameObject prefab)
    {
        if (prefab == null)
        {
            previewImage.enabled = false;
            return;
        }

        // Get ALL sprite renderers
        var renderers = prefab.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (var sr in renderers)
        {
            if (sr.sprite != null && sr.sprite.name != "Square")
            {
                previewImage.sprite = sr.sprite;
                previewImage.enabled = true;

                // Force size so it’s visible
                previewImage.rectTransform.sizeDelta = new Vector2(120, 120);

                rotation = 0f;
                previewImage.rectTransform.rotation = Quaternion.identity;

                return;
            }
        }

        // fallback
        previewImage.enabled = false;
    }

    public void RotatePreview()
    {
        if (!previewImage.enabled) return;

        rotation += 90f;
        if (rotation >= 360f) rotation = 0f;

        previewImage.rectTransform.rotation =
            Quaternion.Euler(0, 0, rotation);
    }
}