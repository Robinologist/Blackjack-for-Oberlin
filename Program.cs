using System;
using System.Collections;
using System.Data.SqlTypes;
using System.Formats.Asn1;
using System.Security.Cryptography.X509Certificates;
class Program
{
    public static int numDecks = 1;
    public static int textSpeed = 10;
    public static int lineDelay = 120;
    public static int numSuits = CardIcons.suits.Count();
    public static int numValues = CardIcons.values.Count();
    public static bool noConsoleDelay = true;
    public static bool noConsoleClear = true;

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
        Console.Write("› ");
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
                    bet = (int)bet;
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

        void PlayerSetupPhase(Hand playerHand)
        {
            LogHeader("Player's Turn");

            Hand dealerHand = hands[0];
            DrawCard(playerHand, deck, 2);

            for (int i = 2; i > 0; i--)
            {
                Log("You have been dealt the " + playerHand.cards[^i].name);
                DelayConsole();
            }

            playerHand.Score();

            Log("\nYour hand is:");
            DelayConsole();
            playerHand.Print();
            DelayConsole();

            // Naturals
            if (playerHand.outcome == Hand.Type.Blackjack)
            {
                Log("\nA natural blackjack!");
                Log("Can the dealer match it?");
                DelayConsole(15);
            }

            Log("\nThe dealer's hand is:");
            DelayConsole();
            dealerHand.Print(true);
            DelayConsole();

            if (dealerHand.cards[0].value == 1 || dealerHand.cards[0].value >= 10)
            {
                if (playerHand.outcome == Hand.Type.Blackjack) Log("\nThe dealer could still tie with a natural of their own; They will check their face-down card...");
                else Log("\nThe dealer could have a natural; They will check their face-down card...");
                DelayConsole(15);

                if (dealerHand.outcome == Hand.Type.Blackjack)
                {
                    Log("The dealer's face-down card is the " + dealerHand.cards[^1].name);
                    Log("A natural blackjack!\n");
                    if (playerHand.outcome == Hand.Type.Blackjack)
                    {
                        Log("It's a tie!");
                        Payout(GameOutcome.tie, playerHand);
                    }
                    else Payout(GameOutcome.lose, playerHand);
                }
                else
                {
                    if (playerHand.outcome == Hand.Type.Blackjack)
                    {
                        Log("The dealer does not have a natural!");
                        Payout(GameOutcome.win, playerHand);
                    }
                    else Log("The dealer does not have a natural");
                }
            }
            else
            {
                if (playerHand.outcome == Hand.Type.Blackjack)
                {
                    Log("\nThe dealer cannot have a natural!");
                    Payout(GameOutcome.win, playerHand);
                }
            }
        }

        void PlayerPhase(Hand playerHand)
        {
            string? playerInput;
            while (true)
            {
                // Player Input
                LogPrompt("\nYou may either hit or stand. Which would you like to do?");
                playerInput = Console.ReadLine();
                playerInput = playerInput?.Trim().ToLower();

                if (playerInput == "hit" || playerInput == "h")
                {
                    DrawCard(playerHand, deck);
                    Log("\nYou have been dealt the " + playerHand.cards[^1].name);
                    break;
                }
                else if (playerInput == "stand" || playerInput == "s")
                {
                    Log("\nLet's see how the dealer does...");
                    DelayConsole(15);
                    return;
                }
                else
                {
                    Log("Invalid input; Please try again.");
                }
            }

            Log("\nYour hand is:");
            DelayConsole();
            playerHand.Print();
            DelayConsole();

            // Outcomes
            if (playerHand.outcome == Hand.Type.Blackjack)
            {
                Log("\nBlackjack!");
                Log("Let's see if the dealer can match it...");
                DelayConsole(15);
                return;
            }
            else if (playerHand.outcome == Hand.Type.Bust)
            {
                Log("\nYou've gone bust!");
                Payout(GameOutcome.lose, playerHand);
                return;
            }
            else
            {
                Log("\nThe dealer's hand is:");
                DelayConsole();
                hands[0].Print(true);
                DelayConsole();
            }

            PlayerPhase(playerHand);
        }

        void DealerPhase()
        {
            Hand dealerHand = hands[0];
            LogHeader("Dealer's Turn");

            Log("The dealer's face-down card is the " + dealerHand.cards[^1].name);
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
                        Payout(GameOutcome.tie, playerHand);
                        return;
                    }
                    else
                    {
                        Log("\n21 beats " + playerHand.totalValue);
                        Payout(GameOutcome.lose, playerHand);
                        return;
                    }
                }
                else if (dealerHand.outcome == Hand.Type.Bust)
                {
                    Log("\nThe dealer has gone bust!");
                    Payout(GameOutcome.win, playerHand);
                    return;
                }
                else if (dealerHand.totalValue > playerHand.totalValue)
                {
                    Log("\n" + dealerHand.totalValue + " beats " + playerHand.totalValue + "!");
                    Payout(GameOutcome.lose, playerHand);
                    return;
                }
                else if (playerHand.totalValue > dealerHand.totalValue)
                {
                    Log("\n" + playerHand.totalValue + " beats " + dealerHand.totalValue + "!");
                    Payout(GameOutcome.win, playerHand);
                    DelayConsole(15);
                    return;
                }
                else
                {
                    Log("\nIt's a tie!");
                    Payout(GameOutcome.tie, playerHand);
                    return;
                }
            }
        }

        void Payout(GameOutcome gameOutcome, Hand playerHand)
        {
            switch (gameOutcome)
            {
                case GameOutcome.win:
                    {
                        currentBet *= 1.5f;
                        currentMoney += currentBet;
                        Log("You have won a total of $" + currentBet);
                        break;
                    }
                case GameOutcome.lose:
                    {
                        Log("You have lost your bet of $" + currentBet);
                        break;
                    }
                case GameOutcome.tie:
                    {
                        currentMoney += currentBet;
                        Log("Your bet of $" + currentBet + " has been refunded");
                        break;
                    }
            }
            hands.Remove(playerHand);
            DelayConsole(15);
        }

        bool CheckMoney()
        {
            if (currentMoney == 0)
            {
                LogHeader("Broke");
                Log("You have run out of money!");
                DelayConsole(10);
                Log("Stop gambling and reevaluate your life choices...");
                DelayConsole(25);
                ClearConsole();
                return true;
            }
            else if (currentMoney >= 50000)
            {
                LogHeader("\nBanned");
                Log("You have been kicked out of the casino!");
                DelayConsole(10);
                Log("Take your lucky $" + currentMoney + " and get outta here...");
                DelayConsole(25);
                ClearConsole();
                return true;
            }
            else return false;
        }


        #endregion

        #region Program

        TitleScreen();
        while (true)
        {
            SetupPhase();

            BettingPhase();
            if (!isPlaying) return;

            int handsInPlay = hands.Count - 1;
            for (int i = 1; i < hands.Count; i++) // For each of the player's hands
            {
                PlayerSetupPhase(hands[i]);
                if (hands.Count - 1 < i) break;
                PlayerPhase(hands[i]);
            }
            if (hands.Count == 1) continue;
            DealerPhase();
            if (CheckMoney()) return;
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

public enum GameOutcome
{
    win, lose, tie
}

#endregion

