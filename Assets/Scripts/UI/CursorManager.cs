using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Texture2D clickCursor;
    public Vector2 cursorHotspot = Vector2.zero;

    void Start()
    {
        SetCursor(defaultCursor);
    }

    public void SetCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }

    void Update()
    {
        // For clicking
        if (Input.GetMouseButtonDown(0))
        {
            SetCursor(clickCursor);
        }
        // For idle
        if (Input.GetMouseButtonUp(0))
        {
            SetCursor(defaultCursor);
        }
    }
}




