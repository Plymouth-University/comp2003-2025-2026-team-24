using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    private GameObject _selectedPrefab;
    private GameObject _ghostObject;

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

    private void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _rotation += 90f;

            if (_ghostObject != null)
            {
                _ghostObject.transform.rotation = Quaternion.Euler(0, 0, _rotation);
            }
        }
    }

    public void SetSelectedPrefab(GameObject prefab)
    {
        _selectedPrefab = prefab;
        CreateGhost(prefab);
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
        {
            Destroy(_ghostObject);
        }

        _ghostObject = Instantiate(prefab);

        var col = _ghostObject.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        SetGhostTransparency(_ghostObject, 0.5f);
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

        _ghostObject.transform.position = snapped;
    }

    public void SetGhostValid(bool isValid)
    {
        if (_ghostObject == null) return;

        Color color = isValid ? Color.green : Color.red;
        color.a = 0.5f;

        var sr = _ghostObject.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = color;
        }
    }

    private void SetGhostTransparency(GameObject obj, float alpha)
    {
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }
}