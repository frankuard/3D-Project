using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Card heldCard;
    public Transform holdPoint;

    public Text cardUIText;

    Card nearbyCard;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && nearbyCard != null)
        {
            PickCard(nearbyCard);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            InspectCard();
        }

    
    
    }

    void InspectCard()
    {
        if (heldCard == null) return;

        Debug.Log("CARD INFO: " + heldCard.cardText);
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
        card.transform.localPosition = new Vector3(0.1f, -0.1f, 0.2f);
        card.transform.localRotation = Quaternion.Euler(0, 180, 0);

        Rigidbody rb = card.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (cardUIText != null)
        {
            cardUIText.text = card.cardText;
        }
    }
}