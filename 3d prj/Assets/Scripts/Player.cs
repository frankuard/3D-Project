using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Card heldCard;
    public Transform holdPoint;

    public GameObject cardInfoPanel;
    public Image cardImageUI;

    Card nearbyCard;
    bool uiVisible = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && nearbyCard != null)
        {
            PickCard(nearbyCard);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleCardUI();
        }
    }

    void ToggleCardUI()
    {
        if (heldCard == null) return;

        uiVisible = !uiVisible;
        cardInfoPanel.SetActive(uiVisible);

        if (uiVisible)
        {
            cardImageUI.sprite = heldCard.cardSprite;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Card"))
        {
            nearbyCard = other.GetComponent<Card>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Card"))
        {
            nearbyCard = null;
        }
    }

    void PickCard(Card card)
    {
        if (heldCard != null) return;

        heldCard = card;

        card.transform.SetParent(holdPoint);
        card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.identity;

        Rigidbody rb = card.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
}