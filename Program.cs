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
                    Console.WriteLine(newCard.name + " " + newCard.nameAbbrev);
                }
            }

            return deck;
        }

        Card[] ShuffleDeck(Card[] deck)
        {
            return deck;
        }
        #endregion

        // =-=-=-=-=-=-=-= //

        #region Program
        int numSuits = CardIcons.suits.Length;
        int numValues = CardIcons.values.Length;

        Card[] deck = SetupDeck(numSuits, numValues);
        ShuffleDeck(deck);
        #endregion
    }
}

public class Card
{
    public readonly int suit;
    public readonly int value;
    public readonly string name;
    public readonly string nameAbbrev;

    public Card(int _suit, int _value)
    {
        suit = _suit;
        value = _value;
        name = CardIcons.values[value - 1] + " of " + CardIcons.suits[suit - 1];
        nameAbbrev = "[" + CardIcons.valueAbbrevs[value - 1] + CardIcons.suitAbbrevs[suit - 1] + "]";
    }
}

public static class CardIcons
{
    public static string[] suits = { "Spades", "Hearts", "Clubs", "Diamonds" };
    public static string[] suitAbbrevs = { "♠", "♥", "♣", "♦" };
    public static string[] values = { "Ace", "Two", "Three", "Four", "Five", "Size", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King" };
    public static string[] valueAbbrevs = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
}