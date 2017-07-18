using UnityEngine;

public class ErrorMessage : MonoBehaviour
{
    public string Title { get; set; }
    public string Text { get; set; }

    private const int m_width = 400;
    private const int m_height = 50;

    void OnGUI()
    {
        GUILayout.Window(0, new Rect((Screen.width - m_width) / 2, 100, m_width, m_height), Window, Title, GUILayout.ExpandHeight(true));
    }

    void Window(int windowID)
    {
        GUILayout.Label(Text);

        if (GUILayout.Button("Quit"))
        {
            Application.Quit();
        }
    }
}
