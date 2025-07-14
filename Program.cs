using System;
class Program
{
    static void Main(string[] args)
    {
        float currentMoney = 1200;
        float currentBet = 0;
        bool isPlaying = true;
        int numSuits = CardIcons.suits.Count();
        int numValues = CardIcons.values.Count();
        int numDecks = 1;

        List<Card> deck;
        List<List<Card>> hands = new List<List<Card>>() { new List<Card>(), new List<Card>() }; // Dealer's hand is always index 0

        void Log(string message, bool divider = false)
        {
            Console.WriteLine(message);
            if (divider) Log("\n=-=-=\n");
        }

        #region Deck-Management Functions
        List<Card> SetupDeck(int numSuits, int numValues, int numDecks)
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

        void PrintHand(List<Card> deck)
        {
            foreach (Card card in deck) Log("> " + card.name + " " + card.nameAbbrev);
        }

        void DrawCard(List<Card> deck, List<Card> hand, int numCards = 1)
        {
            for (int i = 0; i < numCards; i++)
            {
                hand.Add(deck[0]);
                deck.RemoveAt(0);
            }
        }

        bool ScoreHand(List<Card> hand)
        {
            int handValue = 0;
            int numAces = 0;

            // Base scoring
            foreach (Card card in hand)
            {
                if (card.value == 0) numAces++;
                else if (card.value <= 10) handValue += card.value;
                else handValue += 10;
            }

            // Ace scoring
            int numElevens = numAces;
            while (numElevens > 0)
            {
                int checkScore = handValue + (numElevens * 11);

                if (checkScore < 21) handValue = checkScore;
                else
                {
                    handValue++;
                    numElevens--;
                }
            }

            // Bust check
            if (handValue == 21)
            {
                currentBet *= 1.5f;
                currentMoney += currentBet;

                Log("\nBlackjack!\nYou have won $" + currentBet + "!\n");
                BettingPhase();
                return false;
            }
            else if (handValue > 21)
            {
                Log("\nBust!\nYou have lost $" + currentBet + ".\n");
                currentBet = 0;
                BettingPhase();
                return false;
            }

            return true;
        }
        #endregion

        #region Gameplay Functions
        float BettingPhase()
        {
            Log("You have $" + currentMoney + ". How much would you like to bet? Alternatively, type \"exit\" to cash out now.");
            string playerInput = Console.ReadLine().Trim().ToLower().TrimStart(['$']);

            if (playerInput == "exit")
            {
                Log("\nYou cashed out with $" + currentMoney);
                isPlaying = false;
                return 0;
            }
            else if (float.TryParse(playerInput, out float bet))
            {
                if (bet <= 0)
                {
                    Log("You're not getting into this game for free; please enter a larger bet.");
                    return BettingPhase();
                }
                if (bet > currentMoney)
                {
                    Log("You do not have that much currentMoney to bet; please enter a smaller bet.");
                    return BettingPhase();
                }
                else
                {
                    Log("You have bet $" + bet + " on this next game. Good luck!", true);
                    currentMoney -= bet;
                    currentBet = bet;
                    return currentBet;
                }
            }
            else
            {
                Log("Invalid input; Please try again.");
                return BettingPhase();
            }
        }

        void DealPhase(int numCards)
        {
            for (int i = 1; i < hands.Count; i++)
            {
                DrawCard(deck, hands[i], numCards);

                while (numCards > 0)
                {
                    Log("You have drawn the " + hands[i][hands[i].Count - numCards].name);
                    numCards--;
                }

                Log("\nYour hand is now:\n");
                PrintHand(hands[i]);

                if (!ScoreHand(hands[i])) return;

                Log("\nYou may either hit or stand. Which would you like to do?\n");
            }
        }
        #endregion

        #region Program

        deck = SetupDeck(numSuits, numValues, numDecks);
        ShuffleDeck(deck);

        if (BettingPhase() == 0) return;
        
        DrawCard(deck, hands[0], 2);
        DealPhase(2);

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

