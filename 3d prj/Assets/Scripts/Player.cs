using UnityEngine;

public class Player : MonoBehaviour
{

    public Card heldCard;
    public Transform holdPoint;


    void InspectCard()
    {
        if (heldCard == null) return;

        Debug.Log("CARD INFO: " + heldCard.cardText);
    }


void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Card"))
    {
        Card card = other.GetComponent<Card>();

        if (card != null)
        {
            PickCard(card);
        }
    }
}

void PickCard(Card card)
{
    heldCard = card;

    card.transform.SetParent(holdPoint);
    card.transform.localPosition = Vector3.zero;
    card.transform.localRotation = Quaternion.identity;

    // disable physics after pickup
    Rigidbody rb = card.GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.isKinematic = true;
        rb.useGravity = false;
    }
}
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            InspectCard();
            
        }
    }
}
