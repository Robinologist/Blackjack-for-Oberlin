class Program
{
    static void Main(string[] args)
    {
        #region Functions

        List<Card> SetupDeck(int numSuits, int numValues)
        {
            List<Card> deck = new();

            for (int s = 1; s <= numSuits; s++)
            {
                for (int v = 1; v <= numValues; v++)
                {
                    Card newCard = new(s, v);
                    deck.Add(newCard);
                }
            }
            return deck;
        }

        List<Card> ShuffleDeck(List<Card> deck)
        {
            Random random = new();

            for (int i = 0; i < deck.Count; i++)
            {
                int randomIndex = random.Next(i, deck.Count);
                (deck[i], deck[randomIndex]) = (deck[randomIndex], deck[i]);
            }
            return deck;
        }

        void PrintDeck(List<Card> deck)
        {
            foreach (Card card in deck) Console.WriteLine(card.name + " " + card.nameAbbrev);
            Console.WriteLine("");
        }

        void DrawCard(List<Card> deck, List<Card> hand)
        {
            Card drawnCard = deck[0];

            hand.Add(drawnCard);
            deck.RemoveAt(0);
        }

        #endregion

        #region Program

        int numSuits = CardIcons.suits.Count();
        int numValues = CardIcons.values.Count();

        List<Card> deck = SetupDeck(numSuits, numValues);
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
    public static string[] suits = ["Spades", "Hearts", "Clubs", "Diamonds"];
    public static string[] suitAbbrevs = ["♠", "♥", "♣", "♦"];
    public static string[] values = ["Ace", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King"];
    public static string[] valueAbbrevs = ["A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"];
}

#endregion