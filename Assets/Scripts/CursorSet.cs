using UnityEngine;

public class CursorSet : MonoBehaviour
{
    [SerializeField] private Texture2D normalCursor;
    [SerializeField] private Texture2D clickCursor;
    private Texture2D currentCursor;

    public bool isClicking = false;

    private void Start()
    {
        SetCursor(normalCursor);
    }

    void Update()
    {
        // Handle cursor update when game is not paused
        if (Time.timeScale == 1)
        {
            SetCursorIfGunIsDisabled();
        }
    }

    // OnGUI method ensures cursor updates work even with time paused
    void OnGUI()
    {
        if (Time.timeScale == 0) // Handling when the game is paused
        {
            SetCursorIfGunIsDisabled();
        }
    }

    void SetCursorIfGunIsDisabled()
    {
        PaintballShoot paintballShoot = FindObjectOfType<PaintballShoot>();

        // Ensure cursor changes when time is paused or shooting is disabled
        if (paintballShoot == null || !paintballShoot.isActiveAndEnabled || Time.timeScale == 0)
        {
            // Detect mouse input during paused state using Event.current
            if (Event.current != null && Event.current.isMouse)
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)  // Left Mouse Button
                {
                    isClicking = true;
                    SetCursor(clickCursor);
                }
                else if (Event.current.type == EventType.MouseUp && Event.current.button == 0)  // Left Mouse Button
                {
                    isClicking = false;
                    SetCursor(normalCursor);
                }
            }
        }
    }

    void SetCursor(Texture2D cursor)
    {
        if (currentCursor != cursor)
        {
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
            currentCursor = cursor;
        }
    }
}