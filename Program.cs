class Program
{
    static void Main(string[] args)
    {
        int numSuits = 4;
        int numValues = 13;

        Console.WriteLine("Hello World");
        SetupDeck(numSuits, numValues);
        // switch (Console.ReadLine())

        void SetupDeck(int numSuits, int numValues)
        {
            for (int s = 1; s <= numSuits; s++)
            {
                for (int v = 1; v <= numValues; v++)
                {
                    Card newCard = new(s, v);
                    Console.WriteLine(s + " | " + v);
                }
            }
        }
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
