using System;
using System.Formats.Asn1;
using System.Security.Cryptography.X509Certificates;
class Program
{
    public static int numDecks = 1;
    public static int textSpeed = 10;
    public static int lineDelay = 200;
    public static int numSuits = CardIcons.suits.Count();
    public static int numValues = CardIcons.values.Count();

    #region Console Methods

    public static void Log(string message, bool animateText = true)
    {
        Thread.Sleep(lineDelay);

        if (animateText)
        {
            foreach (char c in message)
            {
                Console.Write(c);
                Thread.Sleep(textSpeed);
            }
            Console.WriteLine();
        }
        else Console.WriteLine(message);
    }

    public static void LogPrompt(string message)
    {
        Log(message);
        Console.Write("> ");
    }

    public static void DelayConsole(int delayCoefficient = 3)
    {
        Thread.Sleep(lineDelay * delayCoefficient);
    }

    static void Main(string[] args)
    {
        float currentMoney = 1200;
        float currentBet = 0;
        bool isPlaying = true;
        List<Card> deck = [];
        List<Hand> hands = [];

        #endregion

        #region Deck-Management Methods
        List<Card> CreateDeck(int numSuits, int numValues, int numDecks)
        {
            List<Card> deck = new();

            for (int d = 0; d < numDecks; d++)
            {
                for (int s = 1; s <= numSuits; s++)
                {
                    for (int v = 1; v <= numValues; v++)
                    {
                        Card newCard = new(s, v);
                        deck.Add(newCard);
                    }
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

        void DrawCard(Hand hand, List<Card> deck, int numCards = 1)
        {
            for (int i = 0; i < numCards; i++)
            {
                hand.cards.Add(deck[0]);
                deck.RemoveAt(0);
            }
            hand.Score();
        }

        #endregion

        #region Gameplay Methods
        void SetupPhase()
        {
            Console.Clear();

            deck = CreateDeck(numSuits, numValues, numDecks);
            ShuffleDeck(deck);

            hands = new List<Hand>() { new Hand(), new Hand() }; // Dealer's hand is always index 0
            DrawCard(hands[0], deck, 2);
        }

        void BettingPhase()
        {
            Console.Clear();

            string? playerInput;
            while (true)
            {
                LogPrompt("You have $" + currentMoney + ". How much would you like to bet? Alternatively, outcome \"exit\" to cash out now.");
                playerInput = Console.ReadLine();
                playerInput?.Trim().ToLower().TrimStart(['$']);

                if (playerInput == "exit")
                {
                    Log("\nYou cashed out with $" + currentMoney);
                    isPlaying = false;
                    break;
                }
                else if (float.TryParse(playerInput, out float bet))
                {
                    if (bet <= 0)
                    {
                        Log("\nYou're not getting into this game for free; please enter a larger bet.");
                    }
                    if (bet > currentMoney)
                    {
                        Log("\nYou do not have that much money; please enter a smaller bet.");
                    }
                    else
                    {
                        Log("\nYou have bet $" + bet + " on this next game. Good luck!");
                        DelayConsole(7);
                        Console.Clear();
                        currentMoney -= bet;
                        currentBet = bet;
                        break;
                    }
                }
                else
                {
                    Log("\nInvalid input; Please try again.");
                }
            }
        }

        void PlayerPhase(int numCards = 1)
        {
            for (int i = 1; i < hands.Count; i++)
            {
                DrawCard(hands[i], deck, numCards);

                while (numCards > 0)
                {
                    Log("You have been dealt the " + hands[i].cards[^numCards].name);
                    numCards--;
                    DelayConsole();
                }
                hands[i].Score();

                string? playerInput;
                while (true)
                {
                    Log("\nYour hand is:");
                    DelayConsole();
                    hands[i].Print();
                    DelayConsole();

                    // Outcomes
                    if (hands[i].outcome == Hand.Type.Blackjack)
                    {
                        Log("\nBlackjack!\nLet's see if the dealer can match it.");
                        DelayConsole();
                        break;
                    }
                    else if (hands[i].outcome == Hand.Type.Bust)
                    {
                        Log("\nBust!\nYou have lost $" + currentBet + ".\n");
                        DelayConsole();
                        currentBet = 0;
                        break;
                    }
                    else
                    {
                        Log("\nThe dealer's hand is:");
                        DelayConsole();
                        hands[0].Print(true);
                        DelayConsole();

                        // Player Input
                        LogPrompt("\nYou may either hit or stand. Which would you like to do?\n");
                        playerInput = Console.ReadLine();
                        playerInput?.Trim().ToLower();

                        if (playerInput == "hit" || playerInput == "h")
                        {
                            PlayerPhase();
                        }
                        else if (playerInput == "stand" || playerInput == "s") break;
                        else
                        {
                            Log("Invalid input; Please try again.\n");
                        }
                    }
                }
            }
        }

        void DealerPhase()
        {
            Console.Clear();

            Log("\nIt is the dealer's turn.\n");
            DelayConsole();
            Log("\nThe dealer's hidden card was the " + hands[0].cards[hands[0].cards.Count - 1] + "\n");
            DelayConsole();
        }

        // void Payout()
        // {
        //     currentBet *= 1.5f;
        //     currentMoney += currentBet;

        //     Log("\nYou have won $" + currentBet + "!\n");
        // }


        #endregion

        #region Program

        while (true)
        {
            SetupPhase();
            BettingPhase();
            if (!isPlaying) return;
            PlayerPhase(2);
            DealerPhase();
        }

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

public class Hand
{
    public List<Card> cards = new();
    int totalValue = 0;
    public Type outcome = Type.Safe;

    public void Score()
    {
        totalValue = 0;
        int numAces = 0;

        // Base scoring
        foreach (Card card in cards)
        {
            if (card.value == 0) numAces++;
            else if (card.value <= 10) totalValue += card.value;
            else totalValue += 10;
        }

        // Ace scoring
        int numElevens = numAces;
        while (numElevens > 0)
        {
            int checkScore = totalValue + (numElevens * 11);

            if (checkScore < 21) totalValue = checkScore;
            else
            {
                totalValue++;
                numElevens--;
            }
        }

        // Outcomes
        if (totalValue == 21) outcome = Type.Blackjack;
        else if (totalValue > 21) outcome = Type.Bust;
        else outcome = Type.Safe;
    }

    public void Print(bool showOnlyFirst = false)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            if (showOnlyFirst && i > 0) Program.Log("• Unknown Card [?]");
            else Program.Log("• " + card.name + " " + card.nameAbbrev);
        }
    }

    public enum Type
    {
        Blackjack, Safe, Bust
    }
}

#endregion

