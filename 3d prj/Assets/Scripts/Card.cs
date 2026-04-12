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


     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
