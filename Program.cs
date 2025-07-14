class Program
{
    static void Main(string[] args)
    {
        #region Functions
        Card[] SetupDeck(int numSuits, int numValues)
        {
            Card[] deck = new Card[numSuits * numValues];

            for (int s = 1; s <= numSuits; s++)
            {
                for (int v = 1; v <= numValues; v++)
                {
                    Card newCard = new(s, v);
                    deck[((s - 1) * numValues) + (v - 1)] = newCard;
                    // Console.WriteLine(s + " | " + v);
                }
            }

            return deck;
        }

        Card[] ShuffleDeck(Card[] deck)
        {
            return deck;
        }
        #endregion

        #region Program
        int numSuits = 4;
        int numValues = 13;

        Card[] deck = SetupDeck(numSuits, numValues);
        ShuffleDeck(deck);
        #endregion
    }
}

public class Card
{
    int suit;
    int value;

    public Card(int _suit, int _value)
    {
        suit = _suit;
        value = _value;
    }
}
