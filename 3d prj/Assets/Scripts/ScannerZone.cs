using UnityEngine;

public class ScannerZone : MonoBehaviour
{
    public GateController gateController;
    public string[] allowedCardTypes = { "Staff", "Student", "Security" };

    private bool _hasScanned = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_hasScanned) return;

        Card heldCard = FindHeldCard();

        if (heldCard == null)
        {
            gateController.DenyAccess("NO CARD DETECTED");
            return;
        }

        bool allowed = false;
        foreach (string t in allowedCardTypes)
        {
            if (heldCard.cardType.Trim().ToLower() == t.Trim().ToLower())
                allowed = true;
        }

        if (allowed)
        {
            _hasScanned = true;
            gateController.GrantAccess();
        }
        else
        {
            gateController.DenyAccess("ACCESS LEVEL TOO LOW");
        }
    }

    Card FindHeldCard()
    {
        Card[] allCards = FindObjectsByType<Card>(FindObjectsSortMode.None);
        foreach (Card c in allCards)
        {
            if (c.IsHeld()) return c;
        }
        return null;
    }
}