using UnityEngine;

public class Card : MonoBehaviour
{
    public CardType cardType;

    public Sprite cardSprite; // image of full ID card

    public string personName;
    public string department;
    public string idNumber;

    public enum CardType
    {
        StudentID,
        CEPCard,
        StaffCard,
        Unknown
    }
}