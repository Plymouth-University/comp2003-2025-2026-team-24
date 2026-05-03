using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    private GameObject _selectedPrefab;
    private GameObject _ghostObject;

    private SpriteRenderer[] _ghostRenderers;

    private float _rotation = 0f;
    private bool _homePlaced = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        HandleRotation();
        UpdateGhost();
    }



    private void RotateGhost()
    {
        _rotation += 90f;

        if (_rotation >= 360f)
            _rotation = 0f;

        if (_ghostObject != null)
        {
            _ghostObject.transform.rotation =
                Quaternion.Euler(0, 0, _rotation);
        }
    }

    private void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateGhost();
            PreviewUIController.Instance.RotatePreview();
        }
    }

    public void RotateFromButton()
    {
        RotateGhost();
    }

    public void SetSelectedPrefab(GameObject prefab)
    {
        _selectedPrefab = prefab;
        CreateGhost(prefab);
        PreviewUIController.Instance.SetPreview(prefab);

    }

    public GameObject GetSelectedPrefab()
    {
        return _selectedPrefab;
    }

    public float GetRotation()
    {
        return _rotation;
    }

    public bool IsHomePlaced()
    {
        return _homePlaced;
    }

    public void SetHomePlaced()
    {
        _homePlaced = true;
    }

    private void CreateGhost(GameObject prefab)
    {
        if (_ghostObject != null)
            Destroy(_ghostObject);

        _ghostObject = Instantiate(prefab);

        Collider2D[] cols = _ghostObject.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D c in cols)
            c.enabled = false;

        _ghostRenderers = _ghostObject.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in _ghostRenderers)
        {
            // Main faint sprite
            sr.sortingOrder = 999;
            sr.color = new Color(1f, 1f, 1f, 0.08f);

            // Create outline clone
            GameObject outline = new GameObject("Outline");
            outline.transform.SetParent(sr.transform, false);
            outline.transform.localPosition = Vector3.zero;
            outline.transform.localRotation = Quaternion.identity;
            outline.transform.localScale = Vector3.one * 1.08f;

            SpriteRenderer osr = outline.AddComponent<SpriteRenderer>();
            osr.sprite = sr.sprite;
            osr.sortingOrder = 998;
            osr.color = new Color(0f, 1f, 0f, 0.55f);
        }

        _ghostObject.transform.rotation = Quaternion.Euler(0, 0, _rotation);
    }

    private void UpdateGhost()
    {
        if (_ghostObject == null) return;
        if (Camera.main == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 snapped = new Vector3(
            Mathf.Round(mousePos.x),
            Mathf.Round(mousePos.y),
            0
        );

        _ghostObject.transform.position = snapped + new Vector3(0f, 0f, -0.1f);

        _ghostObject.transform.localScale = Vector3.one * 0.82f;
    }

    public void SetGhostValid(bool isValid)
    {
        if (_ghostObject == null) return;

        Color outlineColor = isValid
            ? new Color(0f, 1f, 0f, 0.55f)
            : new Color(1f, 0f, 0f, 0.55f);

        SpriteRenderer[] all = _ghostObject.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in all)
        {
            if (sr.gameObject.name == "Outline")
                sr.color = outlineColor;
            else
                sr.color = new Color(1f, 1f, 1f, 0.05f);
        }
    }

    private void SetGhostColour(Color colour, float alpha)
    {
        if (_ghostRenderers == null) return;

        colour.a = alpha;

        foreach (SpriteRenderer sr in _ghostRenderers)
        {
            if (sr != null)
                sr.color = colour;
        }
    }
}