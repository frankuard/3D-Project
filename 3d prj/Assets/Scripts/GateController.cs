using UnityEngine;
using UnityEngine.UI;

public class GateController : MonoBehaviour
{
    public GameObject door;
    public GameObject popup;
    public Text popupText;

    public void GrantAccess()
    {
        door.SetActive(false);
        ShowPopup("ACCESS GRANTED");
    }

    public void DenyAccess(string msg)
    {
        ShowPopup(msg);
    }

    void ShowPopup(string msg)
    {
        popup.SetActive(true);
        popupText.text = msg;

        CancelInvoke();
        Invoke("HidePopup", 1.5f);
    }

    void HidePopup()
    {
        popup.SetActive(false);
    }
}