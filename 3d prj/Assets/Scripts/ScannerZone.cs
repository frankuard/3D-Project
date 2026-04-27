using UnityEngine;

public class ScannerZone : MonoBehaviour
{
    public GateController gate;

    private Player nearbyPlayer;

    void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();

        if (player != null)
        {
            nearbyPlayer = player;
            Debug.Log("Player entered scanner zone");
        }
    }

    void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();

        if (player != null)
        {
            nearbyPlayer = null;
            Debug.Log("Player left scanner zone");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (nearbyPlayer != null)
            {
                ScanCard();
            }
        }
    }

    void ScanCard()
    {
        if (nearbyPlayer.heldCard == null)
        {
            gate.DenyAccess("NO CARD");
            return;
        }

        Card.CardType type = nearbyPlayer.heldCard.cardType;

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