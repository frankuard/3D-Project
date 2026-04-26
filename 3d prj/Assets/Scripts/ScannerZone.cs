using UnityEngine;

public class ScannerZone : MonoBehaviour
{
    public GateController gate;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Scanner hit by: " + other.name);

        // ALWAYS get Player from parent chain
        Player player = other.GetComponentInParent<Player>();

        if (player == null)
        {
            Debug.Log("No Player found in hierarchy");
            return;
        }

        if (player.heldCard == null)
        {
            gate.DenyAccess("NO CARD");
            return;
        }

        Card.CardType type = player.heldCard.cardType;

        if (type == Card.CardType.StudentID ||
            type == Card.CardType.StaffCard)
        {
            gate.GrantAccess();
        }
        else
        {
            gate.DenyAccess("INVALID CARD");
        }
    }
}