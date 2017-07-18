using UnityEngine;

public class EndProgram : MonoBehaviour
{
    void Update ()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
    }
}
