using UnityEngine;

public class ChangeCameraDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject display1, display2;

    [SerializeField]
    private GameObject roomCanvas, archiveCanvas;

    private Camera cam1;
    private Camera cam2;

    private void Start()
    {
        cam1 = display1.GetComponent<Camera>();
        cam2 = display2.GetComponent<Camera>();
        roomCanvas.GetComponent<Canvas>().targetDisplay = 0;
        archiveCanvas.GetComponent<Canvas>().targetDisplay = 1;
        display2.SetActive(false);
    }

    public void changeDisplay()
    {
        if(cam1.targetDisplay == 0)
        {
            cam1.targetDisplay = 1;
            cam2.targetDisplay = 0;
            roomCanvas.GetComponent<Canvas>().targetDisplay = 1;
            archiveCanvas.GetComponent<Canvas>().targetDisplay = 0;
        }
        else
        {
            cam1.targetDisplay = 0;
            cam2.targetDisplay = 1;
            roomCanvas.GetComponent<Canvas>().targetDisplay = 0;
            archiveCanvas.GetComponent<Canvas>().targetDisplay = 1;
        }
        display1.SetActive(!display1.gameObject.activeInHierarchy);
        display2.SetActive(!display2.gameObject.activeInHierarchy);
    }
}
