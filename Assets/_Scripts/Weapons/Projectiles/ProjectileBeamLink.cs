using UnityEngine;

public sealed class ProjectileBeamLink : MonoBehaviour
{
    [SerializeField] LineRenderer line;
    [SerializeField] Transform projectileEnd;
    [SerializeField] float lifetime = 0.15f;
    [SerializeField] bool fadeWidth = true;
    [SerializeField] bool debugLogs;

    Vector3 _startPos;
    float _timer;
    float _startWidth;
    bool _active;

    void Awake()
    {
        if (line == null)
            line = GetComponent<LineRenderer>();

        if (line != null)
        {
            _startWidth = line.widthMultiplier;
            line.enabled = false;
        }
    }

    public void Init(Vector3 startPos)
    {
        if (line == null)
            line = GetComponent<LineRenderer>();

        if (projectileEnd == null)
            projectileEnd = transform.root;

        _startPos = startPos;
        _timer = 0f;
        _active = true;

        line.useWorldSpace = true;
        line.positionCount = 2;
        line.widthMultiplier = _startWidth > 0f ? _startWidth : line.widthMultiplier;
        line.enabled = true;

        UpdateLine();
    }

    void LateUpdate()
    {
        if (!_active || line == null || projectileEnd == null)
            return;

        _timer += Time.deltaTime;

        if (_timer >= lifetime)
        {
            ResetBeam();
            return;
        }

        if (fadeWidth)
        {
            float t = Mathf.Clamp01(_timer / lifetime);
            line.widthMultiplier = Mathf.Lerp(_startWidth, 0f, t);
        }

        UpdateLine();
    }

    void UpdateLine()
    {
        line.SetPosition(0, _startPos);
        line.SetPosition(1, projectileEnd.position);
    }

    public void ResetBeam()
    {
        _active = false;
        _timer = 0f;

        if (line == null)
            return;

        line.enabled = false;

        if (_startWidth > 0f)
            line.widthMultiplier = _startWidth;
    }
}