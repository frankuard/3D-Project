using UnityEngine;

public class Card : MonoBehaviour
{
    public CardType cardType;

    [TextArea]
    public string cardText;

    public enum CardType
    {
        StudentID,
        CEPCard,
        StaffCard,
        Unknown
    }
}