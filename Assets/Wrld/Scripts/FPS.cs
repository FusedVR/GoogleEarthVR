using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour {

    Text fps = null;

	void Start ()
    {
        fps = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        fps.text = string.Format("{0}", 1 / Time.deltaTime);
	}
}
