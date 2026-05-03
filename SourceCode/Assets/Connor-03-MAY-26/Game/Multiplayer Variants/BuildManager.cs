using UnityEngine;

public class BuildManager_MP : MonoBehaviour
{
    public static BuildManager_MP Instance;

    private GameObject _selectedPrefab;
    private GameObject _ghostObject;

    private SpriteRenderer[] _ghostRenderers;

    private float _rotation = 0f;
    private bool _homePlaced = false;

    [Header("Preview Settings")]
    [SerializeField] private Vector3 previewOffset = new Vector3(6f, 3f, 0f);
    [SerializeField] private float previewScale = 1.5f;
    [SerializeField] private Transform previewAnchor;
    private GameObject _previewObject;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        HandleRotation();
        UpdateGhost();
    }

    // =========================
    // ROTATION
    // =========================

    private void RotateGhost()
    {
        _rotation += 90f;

        if (_rotation >= 360f)
            _rotation = 0f;

        //  FORCE SNAP HERE TOO
        _rotation = Mathf.Round(_rotation / 90f) * 90f;

        if (_ghostObject != null)
        {
            _ghostObject.transform.rotation =
                Quaternion.Euler(0, 0, _rotation);
        }

        if (_previewObject != null)
        {
            _previewObject.transform.rotation =
                Quaternion.Euler(0, 0, _rotation);
        }
    }

    private void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateGhost();

        }
    }

    public void RotateFromButton()
    {
        RotateGhost();
    }

    // =========================
    // PREFAB SELECTION
    // =========================

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
        return Mathf.Round(_rotation / 90f) * 90f;
    }

    public bool IsHomePlaced()
    {
        return _homePlaced;
    }

    public void SetHomePlaced()
    {
        _homePlaced = true;
    }

    // =========================
    // TURN VALIDATION
    // =========================

    public bool CanPlace()
    {
        // Nothing selected
        if (_selectedPrefab == null)
            return false;

        // Already placed this turn
        if (!GameManager_MP.Instance.CanPlaceTile())
            return false;

        return true;
    }

    // =========================
    // GHOST SYSTEM
    // =========================


    private void CreateGhost(GameObject prefab)
    {
        if (_ghostObject != null)
            Destroy(_ghostObject);

        _ghostObject = Instantiate(prefab);

        foreach (var col in _ghostObject.GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        foreach (SpriteRenderer sr in _ghostObject.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.sortingOrder = 999;

            // FORCE SOLID COLOUR (NOT TRANSPARENT)
            sr.color = Color.green;

            // IGNORE original sprite shading
            sr.material = new Material(Shader.Find("Sprites/Default"));

            // Outline
            GameObject outline = new GameObject("Outline");
            outline.transform.SetParent(sr.transform, false);
            outline.transform.localPosition = Vector3.zero;
            outline.transform.localScale = Vector3.one * 1.1f;

            SpriteRenderer osr = outline.AddComponent<SpriteRenderer>();
            osr.sprite = sr.sprite;
            osr.sortingOrder = 998;
            osr.color = Color.black;
        }

        _ghostObject.transform.rotation = Quaternion.Euler(0, 0, _rotation);
    }

    private void UpdateGhost()
    {
        if (_ghostObject == null || Camera.main == null)
            return;

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

        Color baseColor = isValid ? Color.green : Color.red;

        foreach (SpriteRenderer sr in _ghostObject.GetComponentsInChildren<SpriteRenderer>())
        {
            if (sr.gameObject.name == "Outline")
                sr.color = Color.black;
            else
                sr.color = baseColor;
        }
    }

}