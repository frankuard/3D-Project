using UnityEngine;

public class ScannerZone : MonoBehaviour
{
    public GateController gateController;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Check if player is holding a card
        Card heldCard = FindHeldCard();

        if (heldCard != null)
            gateController.GrantAccess();
        else
            gateController.DenyAccess("NO CARD DETECTED");
    }

    Card FindHeldCard()
    {
        // Finds any card that is currently parented to a hold point
        Card[] allCards = FindObjectsByType<Card>(FindObjectsSortMode.None);
        foreach (Card c in allCards)
        {
            if (c.transform.parent != null) return c; // it's held
        }
        return null;
    }
}