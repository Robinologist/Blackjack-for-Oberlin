using System;
using System.Data.SqlTypes;
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
        if (!noConsoleDelay)
        {
            if (Console.KeyAvailable) Console.ReadKey(true); // Eat up keypresses while text is paused
            Thread.Sleep(lineDelay * delayCoefficient);
        }
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
            List<Card> deck = [];

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
        bool isPlaying;
        float currentMoney = 1200;
        float currentBet;
        List<Card> deck;
        List<Hand> hands;

        #region Gameplay Methods
        void TitleScreen()
        {
            ClearConsole();
            Log("[J♠] Blackjack in C# | by Robin Engdahl", false);
            DelayConsole(15);
        }

        void SetupPhase()
        {
            isPlaying = true;
            currentBet = 0;

            deck = CreateDeck(numSuits, numValues, numDecks);
            ShuffleDeck(deck);

            hands = [new Hand(), new Hand()]; // Dealer's hand is always index 0
            DrawCard(hands[0], deck, 2);
            hands[0].Score();
        }

        void BettingPhase()
        {
            LogHeader("Betting");

            string? playerInput;
            while (true)
            {
                LogPrompt("You have $" + currentMoney + "\nHow much would you like to bet? Alternatively, input \"exit\" to cash out now");
                playerInput = Console.ReadLine();
                playerInput = playerInput?.Trim().ToLower().TrimStart(['$']);

                if (playerInput == "exit")
                {
                    LogHeader("Escape");
                    Log("\nYou cashed out with $" + currentMoney);
                    DelayConsole(15);
                    ClearConsole();
                    isPlaying = false;
                    return;
                }
                else if (float.TryParse(playerInput, out float bet))
                {
                    if (bet <= 0)
                    {
                        Log("\nUnfortunately, $0 is below the required ante; please enter a larger bet.\n");
                    }
                    else if (bet > currentMoney)
                    {
                        Log("\nYou do not have that much money; please enter a smaller bet.\n");
                    }
                    else
                    {
                        Log("\nYou have bet $" + bet + " on this next game. Good luck!");
                        DelayConsole(15);
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
                DelayConsole(15);
                return;
            }
            else if (hand.outcome == Hand.Type.Bust)
            {
                Log("\nYou've gone bust!\nYou have lost $" + currentBet + ".\n");
                DelayConsole(15);
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
                    playerInput = playerInput?.Trim().ToLower();

                    if (playerInput == "hit" || playerInput == "h")
                    {
                        PlayerPhase(hand);
                        return;
                    }
                    else if (playerInput == "stand" || playerInput == "s")
                    {
                        Log("\nLet's see how the dealer does...\n");
                        DelayConsole(15);
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
            Hand dealerHand = hands[0];
            LogHeader("Dealer's Turn");

            Log("The dealer's second card is the " + dealerHand.cards[^1].name);
            DelayConsole(5);
            Log("\nThe dealer's hand is:");
            DelayConsole();
            dealerHand.Print();
            Log("");
            DelayConsole();

            // Drawing up to 17
            while (dealerHand.totalValue < 17)
            {
                DrawCard(dealerHand, deck);
                dealerHand.Score();
                Log("The dealer has been dealt the " + dealerHand.cards[^1].name);
                DelayConsole();
            }

            // Outcomes
            for (int i = 1; i < hands.Count; i++)
            {
                Hand playerHand = hands[i];

                if (dealerHand.outcome == Hand.Type.Blackjack)
                {
                    if (playerHand.outcome == Hand.Type.Blackjack)
                    {
                        currentMoney += currentBet;
                        Log("\nIt's a tie!");
                        Log("\nYour bet of $" + currentBet + " has been refunded");
                        DelayConsole(15);
                        return;
                    }
                    else
                    {
                        Log("Blackjack beats " + playerHand.totalValue);
                        Log("\nYou have lost $" + currentBet);
                        DelayConsole(15);
                        return;
                    }
                }
                else if (dealerHand.outcome == Hand.Type.Bust)
                {
                    currentBet *= 1.5f;
                    currentMoney += currentBet;
                    Log("\nThe dealer has gone bust!");
                    Log("You have won $" + currentBet);
                    DelayConsole(15);
                    return;
                }
                else if (dealerHand.totalValue > playerHand.totalValue)
                {
                    Log("\n" + dealerHand.totalValue + " beats " + playerHand.totalValue + "!");
                    Log("You have lost $" + currentBet);
                    DelayConsole(15);
                    return;
                }
                else if (playerHand.totalValue > dealerHand.totalValue)
                {
                    currentBet *= 1.5f;
                    currentMoney += currentBet;
                    Log("\n" + playerHand.totalValue + " beats " + dealerHand.totalValue + "!");
                    Log("You have won $" + currentBet);
                    DelayConsole(15);
                    return;
                }
                else
                {
                    currentMoney += currentBet;
                    Log("\nIt's a tie!");
                    Log("\nYour bet of $" + currentBet + " has been refunded");
                    DelayConsole(15);
                    return;
                }
            }
        }


        #endregion

        #region Program

        TitleScreen();
        while (true)
        {
            SetupPhase();

            BettingPhase();
            if (!isPlaying) return;

            bool completelyBust = true;
            for (int i = 1; i < hands.Count; i++) // For each of the player's hands
            {
                PlayerPhase(hands[i], 2);
                if (hands[i].outcome != Hand.Type.Bust) completelyBust = false;
            }
            if (!completelyBust) DealerPhase();

            if (currentMoney == 0)
            {
                LogHeader("Broke");
                Log("You have run out of money!");
                DelayConsole(10);
                Log("Stop gambling and reevaluate your life choices...");
                DelayConsole(25);
                ClearConsole();
                return;
            }
            else if (currentMoney >= 50000)
            {
                LogHeader("\nBanned");
                Log("You have been kicked out of the casino!");
                DelayConsole(10);
                Log("Take your lucky $" + currentMoney + " and get outta here...");
                DelayConsole(25);
                ClearConsole();
                return;
            }
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
    public List<Card> cards = [];
    public int totalValue = 0;
    public Type outcome = Type.Safe;

    public void Score()
    {
        totalValue = 0;
        int numAces = 0;

        // Base scoring
        foreach (Card card in cards)
        {
            if (card.value == 1) numAces++;
            else if (card.value <= 10) totalValue += card.value;
            else totalValue += 10;
        }

        // Ace scoring
        for (int i = numAces; i > 0; i--)
        {
            totalValue += 11;
            if (totalValue > 21) totalValue -= 10;
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

