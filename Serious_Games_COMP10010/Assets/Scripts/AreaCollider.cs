using UnityEngine;

public class AreaCollider : MonoBehaviour
{
    public Area currentArea { get; protected set; } = null;
    [SerializeField] private Transform target;
    [SerializeField] private bool setParentNull = true;
    public System.Action<Area> OnAreaChanged;

    private void Start()
    {
        if (setParentNull && target)
            transform.SetParent ( null );
    }

    private void LateUpdate ()
    {
        if (setParentNull && target)
        {
            Vector3 v = target.position;
            v.y = 0;
            transform.position = v;
            transform.rotation = target.rotation;
        }
    }

    public void ChangeArea (Area area)
    {
        currentArea = area;
        OnAreaChanged?.Invoke ( currentArea );
        FindObjectOfType<AreaText> ().SetText ( area.AreaName );
    }
}