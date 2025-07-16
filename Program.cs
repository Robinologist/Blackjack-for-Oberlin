using System;
class Program
{
    public static int numDecks = 1;
    public static int textSpeed = 10;
    public static int lineDelay = 120;
    public static int numSuits = CardIcons.suits.Count();
    public static int numValues = CardIcons.values.Count();
    public static bool noConsoleDelay = false;
    public static bool noConsoleClear = false;

    #region Console Methods

    public static void Log(string message, bool animateText = true)
    {
        if (!noConsoleDelay) DelayConsole();

        if (animateText)
        {
            foreach (char c in message)
            {
                Console.Write(c);
                if (!noConsoleDelay) Thread.Sleep(textSpeed);

                if (Console.KeyAvailable) Console.ReadKey(true); // Eat up keypresses while text is animating
            }
            Console.WriteLine();
        }
        else Console.WriteLine(message);
    }

    public static void LogPrompt(string prompt)
    {
        Log(prompt);
        Console.Write("> ");
    }

    public static void LogHeader(string header)
    {
        ClearConsole();
        Log("|=-=| " + header.ToUpper() + " |=-=|\n", false);
        DelayConsole(1);
    }

    public static void DelayConsole(int delayCoefficient = 2)
    {
        if (!noConsoleDelay) Thread.Sleep(lineDelay * delayCoefficient);
    }

    public static void ClearConsole()
    {
        if (!noConsoleClear) Console.Clear();
        else Console.WriteLine("\n-\n");
    }

    #endregion
    
    #region Deck-Management Methods
    public static List<Card> CreateDeck(int numSuits, int numValues, int numDecks)
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

        public static List<Card> ShuffleDeck(List<Card> deck)
        {
            Random random = new();

            for (int i = 0; i < deck.Count; i++)
            {
                int randomIndex = random.Next(i, deck.Count);
                (deck[i], deck[randomIndex]) = (deck[randomIndex], deck[i]);
            }

            return deck;
        }

        public static void DrawCard(Hand hand, List<Card> deck, int numCards = 1)
        {
            for (int i = 0; i < numCards; i++)
            {
                hand.cards.Add(deck[0]);
                deck.RemoveAt(0);
            }
            hand.Score();
        }

        #endregion

    static void Main(string[] args)
    {
        bool isPlaying = true;
        float currentMoney = 1200;
        float currentBet = 0;
        List<Card> deck = [];
        List<Hand> hands = [];

        #region Gameplay Methods
        void TitleScreen()
        {
            ClearConsole();
            Log("[J♠] Blackjack in C# | by Robin Engdahl", false);
            DelayConsole(7);
        }

        void SetupPhase()
        {
            isPlaying = true;

            deck = CreateDeck(numSuits, numValues, numDecks);
            ShuffleDeck(deck);

            hands = new List<Hand>() { new Hand(), new Hand() }; // Dealer's hand is always index 0
            DrawCard(hands[0], deck, 2);
        }

        void BettingPhase()
        {
            LogHeader("Betting");

            string? playerInput;
            while (true)
            {
                LogPrompt("You have $" + currentMoney + ". How much would you like to bet? Alternatively, input \"exit\" to cash out now.");
                playerInput = Console.ReadLine();
                playerInput?.Trim().ToLower().TrimStart(['$']);

                if (playerInput == "exit")
                {
                    Log("\nYou cashed out with $" + currentMoney);
                    DelayConsole(10);
                    ClearConsole();
                    isPlaying = false;
                    return;
                }
                else if (float.TryParse(playerInput, out float bet))
                {
                    if (bet <= 0)
                    {
                        Log("\nYou're not getting into this game for free; please enter a larger bet.\n");
                    }
                    if (bet > currentMoney)
                    {
                        Log("\nYou do not have that much money; please enter a smaller bet.\n");
                    }
                    else
                    {
                        Log("\nYou have bet $" + bet + " on this next game. Good luck!");
                        DelayConsole(10);
                        currentMoney -= bet;
                        currentBet = bet;
                        return;
                    }
                }
                else
                {
                    Log("\nInvalid input; Please try again.\n");
                }
            }
        }

        void PlayerPhase(Hand hand, int numCards = 1)
        {
            LogHeader("Player's Turn");

            DrawCard(hand, deck, numCards);

            while (numCards > 0)
            {
                    Log("You have been dealt the " + hand.cards[^numCards].name);
                    numCards--;
                    DelayConsole();
                }
                hand.Score();

                string? playerInput;
                Log("\nYour hand is:");
                DelayConsole();
                hand.Print();
                DelayConsole();

            // Outcomes
            if (hand.outcome == Hand.Type.Blackjack)
            {
                Log("\nBlackjack!");
                Log("Let's see if the dealer can match it...");
                DelayConsole(10);
                return;
            }
            else if (hand.outcome == Hand.Type.Bust)
            {
                Log("\nBust!\nYou have lost $" + currentBet + ".\n");
                DelayConsole(10);
                currentBet = 0;
                isPlaying = false;
                return;
            }
            else
            {
                Log("\nThe dealer's hand is:");
                DelayConsole();
                hands[0].Print(true);
                DelayConsole();

                while (true)
                {
                    // Player Input
                    LogPrompt("\nYou may either hit or stand. Which would you like to do?");
                    playerInput = Console.ReadLine();
                    playerInput?.Trim().ToLower();

                    if (playerInput == "hit" || playerInput == "h")
                    {
                        PlayerPhase(hand);
                        return;
                    }
                    else if (playerInput == "stand" || playerInput == "s")
                    {
                        Log("Let's see how the dealer does...");
                        return;
                    }
                    else
                    {
                        Log("Invalid input; Please try again.\n");
                    }
                }
            }
        }

        void DealerPhase()
        {
            LogHeader("Dealer's Turn");

            Log("The dealer's hidden card was the " + hands[0].cards[hands[0].cards.Count - 1].name);
            DelayConsole(5);
        }

        // void Payout()
        // {
        //     currentBet *= 1.5f;
        //     currentMoney += currentBet;

        //     Log("\nYou have won $" + currentBet + "!\n");
        // }


        #endregion

        #region Program

        TitleScreen();
        while (true)
        {
            SetupPhase();
            BettingPhase();
            if (!isPlaying) return;
            for (int i = 1; i < hands.Count; i++) // For each of the player's hands
            {
                PlayerPhase(hands[i], 2);
            }
            if (!isPlaying) continue;
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

            if (checkScore <= 21) totalValue = checkScore;
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

