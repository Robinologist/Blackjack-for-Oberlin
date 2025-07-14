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
                }
            }
            return deck;
        }

        Card[] ShuffleDeck(Card[] deck)
        {
            Random random = new();

            for (int i = 0; i < deck.Length; i++)
            {
                int randomIndex = random.Next(i, deck.Length);
                (deck[i], deck[randomIndex]) = (deck[randomIndex], deck[i]);
            }
            return deck;
        }

        void PrintDeck(Card[] deck)
        {
            foreach (Card card in deck) Console.WriteLine(card.name + " " + card.nameAbbrev);
            Console.WriteLine("");
        }

        #endregion

        #region Program

        int numSuits = CardIcons.suits.Length;
        int numValues = CardIcons.values.Length;

        Card[] deck = SetupDeck(numSuits, numValues);
        PrintDeck(deck);
        ShuffleDeck(deck);
        PrintDeck(deck);

        #endregion
    }
}

#region Data Classes

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
    public static string[] values = { "Ace", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King" };
    public static string[] valueAbbrevs = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
}

#endregion