#region Settings Class
public static class Settings
{
    public static string[] suits = ["Spades", "Hearts", "Clubs", "Diamonds"];
    public static string[] suitAbbrevs = ["♠", "♥", "♣", "♦"];
    public static string[] values = ["Ace", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King"];
    public static string[] valueAbbrevs = ["A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"];

    public static int numDecks = 6;
    public static int numSuits = suits.Length;
    public static int numValues = values.Length;
    public static int shuffleThresholdPercent = 75;
    public static bool showCardCount = false;

    public static int textSpeed = 10;
    public static int lineDelay = 120;

    public static bool noConsoleDelay = false;
    public static bool noConsoleClear = false;
    public static ResponseShortcutMode responseShortcutMode = ResponseShortcutMode.letter;

    public static int[] PossibleBets = [5, 10, 25, 50, 75, 100, 150, 300, 500, 1000];
}
#endregion

class Program
{
    static void Main(string[] args)
    {
        bool isPlaying;
        float currentMoney = 1200;
        int currentBet;
        Deck deck;
        List<Hand> hands;

        #region Gameplay Methods
        void TitleScreen()
        {
            Log.Clear();
            Log.Message("[J♠] Blackjack in C# | by Robin Engdahl", false);
            Log.Delay(15);
        }

        void PrintPlayerHand(Hand playerHand)
        {
            Log.Message("Your hand is:");
            Log.Delay();
            playerHand.Print();
            Log.Delay();
        }

        void PrintDealerHand(bool showOnlyFirst = true)
        {
            Log.Message("The dealer's hand is:");
            Log.Delay();
            hands[0].Print(showOnlyFirst);
            Log.Delay();
        }

        void SetupPhase()
        {
            isPlaying = true;
            currentBet = 0;

            hands = [new Hand(), new Hand()]; // Dealer's hand is always index 0
            deck.Deal(hands[0], 2);
            hands[0].Score();
        }

        void MenuPhase()
        {
            while (true)
            {
                Log.Header("Menu");
                switch (Log.GetPlayerInputString("Welcome to Blackjack!", ["Play", "Tutorial", "Settings", "Exit"]))
                {
                    case "play":
                        Log.Header("Betting");
                        while (true)
                        {
                            if (Settings.showCardCount) Log.Message("There are " + (deck.cards.Count + 2) + " cards left in the deck\n");

                            int playerInputBet = Log.GetPlayerInputInt("You have $" + currentMoney + "\nHow much would you like to bet?", ['$']);
                            if (playerInputBet <= 0)
                            {
                                Log.Message("\nSurprisingly, $" + playerInputBet + " is below the required ante of $1; Please enter a larger bet\n");
                            }
                            else if (playerInputBet > currentMoney)
                            {
                                Log.Message("\nYou do not have that much money; Please enter a smaller bet\n");
                            }
                            else
                            {
                                Log.Message("\nYou have bet $" + playerInputBet + " on this next game. Good luck!");
                                Log.Delay(15);
                                currentMoney -= playerInputBet;
                                currentBet = playerInputBet;
                                return;
                            }
                        }

                    case "tutorial":
                        while (true)
                        {
                            Log.Header("Tutorial");
                            switch (Log.GetPlayerInputString("What part of the game would you like a tutorial on?", ["Basics", "Values", "Naturals", "Splitting", "Doubling Down", "Insurance", "Exit"]))
                            {
                                case "basics":
                                    Log.Header("Basic Rules");

                                    Log.Message("Blackjack is a push-your-luck betting game in which you will draw cards and add up their scores to get as close to a total of 21 without going over");
                                    Log.Delay(15);
                                    Log.Message("\nYou will initially be dealt two cards, as will the dealer (although one of their cards will be face-down)");
                                    Log.Message("Then, you will have the option to either \"hit\" (be dealt another card) or \"stand\" (stick with your current total)");
                                    Log.Message("You may hit as many times as you want, but if your total exceeds 21 (known as \"going bust\") you will lose it all");
                                    Log.Delay(15);
                                    Log.Message("\nOnce you decide to stand, the dealer will reveal their face-down card and then deal themselves cards until their hand's score is greater than 17");
                                    Log.Message("• If they end with a total that is less than your hand's score, you win");
                                    Log.Message("• If they end with a total that is greater than your hand's score, you lose");
                                    Log.Message("• If their hand's score exceed 21, they go bust and you win\n");
                                    Log.Delay(5);

                                    Log.GetPlayerInputString("Return to main menu:", ["Exit"]);
                                    break;

                                case "values":
                                    Log.Header("Values");

                                    Log.Message("Number cards 2-10 count as their number value");
                                    for (int v = 2; v <= 10; v++) Log.Message("[" + v + "]");
                                    Log.Delay(15);
                                    Log.Message("\nAll face cards count as ten");
                                    for (int v = 11; v <= 1; v++) Console.Write(Settings.valueAbbrevs[v]);
                                    Log.Delay(15);
                                    Log.Message("\nAces count as either one or eleven; You can decide based on what's most convenient");
                                    Console.Write("[A]\n");
                                    Log.Delay(5);

                                    Log.GetPlayerInputString("Return to main menu:", ["Exit"]);
                                    break;

                                case "naturals":
                                    Log.Header("Naturals");

                                    Log.Message("If the very first two cards you are dealt are a Ten / face card and an Ace, congratulations! You've achieved a \"natural\" Blackjack! Watch out though, because the dealer can be dealt naturals too!");
                                    Log.Message("\nBecause one of the dealer's cards is face-down, you might not know whether they have a natural or not. If their face-up card is a Ten, face card, or Ace, there's a chance that they have a natural");
                                    Log.Message("If that is the case, they will secretly check their face-down card, and reveal a natural if they have it");
                                    Log.Message("\nIf you have a natural and the dealer's doesn't, you win, but if the dealer has a natural and you don't, you lose\n");
                                    Log.Delay(5);

                                    Log.GetPlayerInputString("Return to main menu:", ["Exit"]);
                                    break;

                                case "splitting":
                                    Log.Header("Splitting");

                                    Log.Message("If your first two cards share the same value, you may choose to split them into two unique hands");
                                    Log.Message("\nWhen splitting pairs, one hand takes your original bet, and you must match that bet for the other hand");
                                    Log.Message("You will play the two hands seperately, hitting until you stand or go bust with each one before it is the dealer's turn");
                                    Log.Message("\nWhen splitting pairs, one hand takes your original bet, and you must match it for the other hand");
                                    Log.Message("If you do not have enough money to effectively double your bet in this way, you may not split your hand");
                                    Log.Message("\nLastly, if you choose to split a pair of aces, you will only be dealt one more card for each hand, after which the dealer will play");
                                    Log.Message("If you score a blackjack with that one last card in either hand, the payout is 2x your bet, rather than the usual 1.5x\n");
                                    Log.Delay(5);

                                    Log.GetPlayerInputString("Return to main menu:", ["Exit"]);
                                    break;

                                case "doubling down":
                                    Log.Header("Doubling Down");

                                    Log.Message("If your first two cards total a value of 9, 10, or 11, you may choose to double down");
                                    Log.Message("\nDoubling down means that you double your bet and will be dealt just one more card before the dealer's turn");
                                    Log.Message("It's high risk, but high reward");
                                    Log.Message("\nOf course, if you do not have the money to double your bet, you cannot double down\n");
                                    Log.Delay(5);

                                    Log.GetPlayerInputString("Return to main menu:", ["Exit"]);
                                    break;

                                case "insurance":
                                    Log.Header("Insurance");

                                    Log.Message("When the dealer is dealt an Ace as their face-up card, and therefore could have a natural blackjack, you can choose to purchase insurance");
                                    Log.Message("\nPurchasing insurance means that you are betting that the dealer does indeed have a blackjack, as a way of minimizing your losses if they do");
                                    Log.Message("You can bet up to half of the original bet you made on this outcome, and if the dealer has a blackjack you will win 2x your insurance bet");
                                    Log.Message("This means if you bet the maximum amount, you could win back all the money that you lost from the dealer's natural");
                                    Log.Message("\nThat being said, if the dealer does not have a blackjack, you will lose your insurance bet, so choose wisely\n");
                                    Log.Delay(5);

                                    Log.GetPlayerInputString("Return to main menu:", ["Exit"]);
                                    break;

                                case "exit":
                                    break;
                            }
                            break;
                        }
                        break;

                    case "settings":
                        Log.Header("Settings");

                        switch (Log.GetPlayerInputString("Which settings would you like to edit?", ["Number of decks", "Shuffle threshold", "Card count visibility", "Text speed", "Line speed", "Response Shortcuts", "Debug Mode"]))
                        {
                            case "number of decks":
                                Log.Header("Number of Decks");
                                Log.Message("This setting controls how many standard decks of cards are shuffled together to make the complete deck cards are dealt from");
                                Log.Message("The most common way to play Blackjack is with 6 decks, meaning 312 cards total");
                                Log.Message("\n Note: Changing this setting from its current value will reshuffle the deck");
                                Log.Delay(15);
                                int numDecks = Log.GetPlayerInputInt("\nHow many decks of cards would you like to play with?");
                                if (numDecks != Settings.numDecks)
                                {
                                    Settings.numDecks = numDecks;
                                    deck = new();
                                    deck.Shuffle();
                                }
                                Log.Message("\nSettings updated!");
                                break;

                            case "shuffle threshold":
                                Log.Header("Shuffle Threshold");
                                Log.Message("This setting controls at what percentage of the deck being played does the dealer reshuffle the deck?");
                                Log.Message("Standard Blackjack usually reshuffles once through with about 75% of the cards");
                                Log.Delay(15);
                                Settings.shuffleThresholdPercent = Log.GetPlayerInputInt("\nAfter what percentage of the total cards being used would you like to reshuffle the deck?", [], ['%']);
                                Log.Message("\nSettings updated!");
                                break;

                            case "card count visibility":
                                Log.Header("Card Count Visibility");
                                Log.Message("This setting is a toggle that, when active, shows the number of cards left in the deck at the start of a round");
                                Log.Delay(15);
                                switch (Log.GetPlayerInputString("\nWould you like to enable automatic card counting?", ["Yes", "No"]))
                                {
                                    case "yes":
                                        Settings.showCardCount = true;
                                        break;

                                    case "no":
                                        Settings.showCardCount = false;
                                        break;
                                }
                                Log.Message("\nSettings updated!");
                                break;

                            case "text speed":
                                Log.Header("Text Speed");
                                Log.Message("This setting controls the delay between each text character being printed to the terminal");
                                Log.Message("The default value is 10. Lower values will be faster, higher ones will be slower");
                                Log.Delay(15);
                                Settings.textSpeed = Log.GetPlayerInputInt("\nHow much delay would you like in between each text character being printed?");
                                Log.Message("\nSettings updated!");
                                break;

                            case "line speed":
                                Log.Header("Line Speed");
                                Log.Message("This setting controls the delay between each new line being printed to the terminal");
                                Log.Message("The default value is 120. Lower values will be faster, higher ones will be slower");
                                Log.Delay(15);
                                Settings.textSpeed = Log.GetPlayerInputInt("\nHow much delay would you like in between each new line being printed?");
                                Log.Message("\nSettings updated!");
                                break;

                            case "response shortcuts":
                                Log.Header("Response Shortcut");
                                Log.Message("This setting determines what shortcut will be displayed next to multiple-choice prompts");
                                Log.Message("\"Letter\" is the default, and will display the first letter of the choice next to it • \"[T] True\"");
                                Log.Message("\"Value\" will display which index the choice is next to it • \"[1] True\"");
                                Log.Message("\"None\" will not display anything next to the choice • \"[True]\"");

                                Log.Message("\nNote: This only controls the visuals for choice text - no matter what this setting is set to, you can still use letter or number shortcuts");
                                switch (Log.GetPlayerInputString("\nWhat would you like to be displayed next to text choices?", ["Letter", "Value", "None"]))
                                {
                                    case "letter":
                                        Settings.responseShortcutMode = ResponseShortcutMode.letter;
                                        break;

                                    case "value":
                                        Settings.responseShortcutMode = ResponseShortcutMode.number;
                                        break;

                                    case "none":
                                        Settings.responseShortcutMode = ResponseShortcutMode.none;
                                        break;
                                }
                                Log.Message("\nSettings updated!");
                                break;

                            case "debug mode":
                                Log.Header("Debug Mode");
                                Log.Message("This setting is a toggle that, when active, does two things");
                                Log.Message("It allows text in the terminal to instantly appear instead of animating in and it prevents text from being cleared for screen transitions");
                                Log.Delay(15);
                                switch (Log.GetPlayerInputString("\nWould you like to enable debug mode?", ["Yes", "No"]))
                                {
                                    case "yes":
                                        Settings.noConsoleClear = true;
                                        Settings.noConsoleDelay = true;
                                        break;

                                    case "no":
                                        Settings.noConsoleClear = false;
                                        Settings.noConsoleDelay = false;
                                        break;
                                }
                                Log.Message("\nSettings updated!");
                                break;
                        }
                        Log.Delay(15);
                        break;

                    case "exit":
                        Log.Header("Exit");
                        switch (Log.GetPlayerInputString("Are you sure you want to quit?", ["Yes", "No"]))
                        {
                            case "yes":
                                Log.Header("Escaped");
                                Log.Message("You cashed out with $" + currentMoney);
                                isPlaying = false;
                                Log.Delay(15);
                                Log.Clear();
                                return;

                            case "no":
                            break;
                        }
                        break;
                        
                }
            }
        }

        void PlayerSetupPhase(Hand playerHand)
        {
            Log.Header("Player's Turn");

            Hand dealerHand = hands[0];
            deck.Deal(playerHand, 2);

            for (int i = 2; i > 0; i--)
            {
                Log.Message("You have been dealt the " + playerHand.cards[^i].name);
                Log.Delay();
            }

            playerHand.Score();

            Log.Message("");
            PrintPlayerHand(playerHand);

            // Naturals
            if (playerHand.outcome == Hand.Type.Blackjack)
            {
                Log.Message("\nA natural!");
                Log.Delay(15);
                Log.Message("Can the dealer match it?");
                Log.Delay(15);
            }

            Log.Message("");
            PrintDealerHand();

            if (dealerHand.cards[0].value == 1 || dealerHand.cards[0].value >= 10)
            {
                if (playerHand.outcome == Hand.Type.Blackjack) Log.Message("\nThe dealer could still tie with a natural of their own; They will check their face-down card...");
                else Log.Message("\nThe dealer could have a natural; They will check their face-down card...");
                Log.Delay(15);

                if (dealerHand.outcome == Hand.Type.Blackjack)
                {
                    Log.Message("The dealer's face-down card is the " + dealerHand.cards[^1].name);
                    Log.Delay();
                    Log.Message("Blackjack!\n");
                    if (playerHand.outcome == Hand.Type.Blackjack)
                    {
                        Log.Message("It's a tie!");
                        Payout(GameOutcome.tie, playerHand);
                        return;
                    }
                    else
                    {
                        Payout(GameOutcome.lose, playerHand);
                        return;
                    }
                }
                else
                {
                    if (playerHand.outcome == Hand.Type.Blackjack)
                    {
                        Log.Message("The dealer does not have a natural!");
                        Payout(GameOutcome.win, playerHand);
                        return;
                    }
                    else Log.Message("The dealer does not have a natural");
                }
            }
            else
            {
                if (playerHand.outcome == Hand.Type.Blackjack)
                {
                    Log.Message("\nThe dealer cannot have a natural!");
                    Payout(GameOutcome.win, playerHand);
                    return;
                }
            }

            // Doubling down
            if (playerHand.outcome != Hand.Type.Blackjack && playerHand.cards.Count == 2)
            {
                if (playerHand.cards[0].value + playerHand.cards[1].value >= 9 && playerHand.cards[0].value + playerHand.cards[1].value <= 11)
                {
                    Log.Message("\nYour cards add up to " + (playerHand.cards[0].value + playerHand.cards[1].value));
                    if (currentMoney >= currentBet)
                    {
                        switch (Log.GetPlayerInputString("Would you like to double down?", ["Yes", "No"]))
                        {
                            case "yes":
                                currentMoney -= currentBet;
                                currentBet *= 2;
                                Log.Message("\nYou have doubled down with a total bet of " + currentBet + "!");
                                deck.Deal(playerHand);
                                Log.Delay();
                                Log.Message("\nAs your final card for this hand, you have been dealt the " + playerHand.cards[^1].name);
                                Log.Message("");
                                PrintPlayerHand(playerHand);
                                Log.Delay(15);
                                DealerPhase();
                                return;

                            case "no":
                                break;
                        }
                    }
                    else
                    {
                        Log.Message("Unfortunately, you do not have enough money to double down");
                    }
                    Log.Delay(15);
                }
                // Splitting pairs
                if (playerHand.cards[0].value == playerHand.cards[1].value)
                {
                    Log.Message("\nYou have a pair!");
                    if (currentMoney >= currentBet)
                    {
                        switch (Log.GetPlayerInputString("Would you like to split it?", ["Yes", "No"]))
                        {
                            case "yes":
                                Hand newHand = new();
                                hands.Add(newHand);
                                newHand.cards.Add(playerHand.cards[1]);
                                playerHand.cards.RemoveAt(1);
                                Log.Message("\nYou have split your pair!");
                                Log.Delay(15);
                                int index1 = hands.IndexOf(playerHand);
                                int index2 = hands.IndexOf(newHand);
                                Log.Header("Player's Turn (Hand #" + index1 + ")");
                                PrintPlayerHand(playerHand);
                                PlayerPhase(playerHand);
                                Log.Header("Player's Turn (Hand #" + index2 + ")");
                                PrintPlayerHand(newHand);
                                PlayerPhase(newHand);
                                return;

                            case "no":
                                break;
                        }
                        Log.Delay(15);
                    }
                    else
                    {
                        Log.Message("Unfortunately, you do not have enough money to split the pair and double your bet");
                    }
                    Log.Delay(15);
                }
            }

            PlayerPhase(hands[1]);
        }

        void PlayerPhase(Hand playerHand)
        {
            // Player Input
            switch (Log.GetPlayerInputString("\nYou may either hit or stand. Which would you like to do?", ["Hit", "Stand"]))
                {
                    case "hit":
                        deck.Deal(playerHand);
                        Log.Message("\nYou have been dealt the " + playerHand.cards[^1].name);
                        break;

                    case "stand":
                        Log.Message("\nWe'll see how the dealer does...");
                        Log.Delay(15);
                        return;
                }

            Log.Message("");
            PrintPlayerHand(playerHand);

            // Outcomes
            if (playerHand.outcome == Hand.Type.Blackjack)
            {
                Log.Message("\nBlackjack!");
                Log.Message("We'll see if the dealer can match it...");
                Log.Delay(15);
                return;
            }
            else if (playerHand.outcome == Hand.Type.Bust)
            {
                Log.Message("\nYou've gone bust!");
                Payout(GameOutcome.lose, playerHand);
                return;
            }
            else
            {
                Log.Message("");
                PrintDealerHand();
            }

            PlayerPhase(playerHand);
            return;
        }

        void DealerPhase()
        {
            Hand dealerHand = hands[0];
            Log.Header("Dealer's Turn");

            Log.Message("The dealer's face-down card is the " + dealerHand.cards[^1].name);
            Log.Delay(5);
            Log.Message("");
            PrintDealerHand(false);
            Log.Message("");
            Log.Delay();

            // Drawing up to 17
            while (dealerHand.totalValue < 17)
            {
                deck.Deal(dealerHand);
                dealerHand.Score();
                Log.Message("The dealer has been dealt the " + dealerHand.cards[^1].name);
                Log.Delay();
            }
            Log.Message("");

            // Outcomes
            int index = 1;
            bool multipleHands = false;
            if (hands.Count > 2) multipleHands = true;
            while (hands.Count > 1)
            {
                if (multipleHands) Log.Message("Hand #" + index);
                Hand playerHand = hands[1];

                if (dealerHand.outcome == Hand.Type.Blackjack)
                {
                    Log.Message("Blackjack!");
                    if (playerHand.outcome == Hand.Type.Blackjack)
                    {
                        currentMoney += currentBet;
                        Log.Message("It's a tie!");
                        Payout(GameOutcome.tie, playerHand);
                    }
                    else
                    {
                        Log.Message("21 beats " + playerHand.totalValue);
                        Payout(GameOutcome.lose, playerHand);
                    }
                }
                else if (dealerHand.outcome == Hand.Type.Bust)
                {
                    Log.Message("The dealer has gone bust!");
                    Payout(GameOutcome.win, playerHand);
                }
                else if (dealerHand.totalValue > playerHand.totalValue)
                {
                    Log.Message(dealerHand.totalValue + " beats " + playerHand.totalValue + "!");
                    Payout(GameOutcome.lose, playerHand);
                }
                else if (playerHand.totalValue > dealerHand.totalValue)
                {
                    Log.Message(playerHand.totalValue + " beats " + dealerHand.totalValue + "!");
                    Payout(GameOutcome.win, playerHand);
                    Log.Delay(15);
                }
                else
                {
                    Log.Message("It's a tie!");
                    Payout(GameOutcome.tie, playerHand);
                }

                if (multipleHands)
                {
                    index++;
                    Log.Message("");
                }
            }
        }

        void Payout(GameOutcome gameOutcome, Hand playerHand)
        {
            switch (gameOutcome)
            {
                case GameOutcome.win:
                    {
                        currentBet = (int)Math.Ceiling(currentBet * 1.5);
                        currentMoney += currentBet;
                        Log.Message("You have won a total of $" + currentBet);
                        break;
                    }
                case GameOutcome.lose:
                    {
                        Log.Message("You have lost your bet of $" + currentBet);
                        break;
                    }
                case GameOutcome.tie:
                    {
                        currentMoney += currentBet;
                        Log.Message("Your bet of $" + currentBet + " has been refunded");
                        break;
                    }
            }
            hands.Remove(playerHand);
            Log.Delay(15);
        }

        bool CheckMoney()
        {
            if (currentMoney == 0)
            {
                Log.Header("Broke");
                Log.Message("You have run out of money!");
                Log.Delay(10);
                Log.Message("Stop gambling and reevaluate your life choices...");
                Log.Delay(25);
                Log.Clear();
                return true;
            }
            else if (currentMoney >= 50000)
            {
                Log.Header("\nBanned");
                Log.Message("You have been kicked out of the casino!");
                Log.Delay(10);
                Log.Message("Take your lucky $" + currentMoney + " and get outta here...");
                Log.Delay(25);
                Log.Clear();
                return true;
            }
            else return false;
        }

        bool CheckDeckPercent()
        {
            int totalCards = Settings.numDecks * Settings.numSuits * Settings.numValues;
            if (deck.cards.Count < (int)((float)totalCards / 100 * Settings.shuffleThresholdPercent)) return true;
            else return false;
        }

        #endregion

        #region Program

        TitleScreen();
        deck = new();
        deck.Shuffle();

        while (true)
        {
            SetupPhase();
            if (CheckDeckPercent())
            {
                Log.Message("\nThe deck is " + Settings.shuffleThresholdPercent + "% of the way through; it's time to reshuffle");
                deck = new();
                deck.Shuffle();
                Log.Delay(15);
                Log.Message("The deck has been shuffled!");
                Log.Delay(15);
            }

            MenuPhase();
            if (!isPlaying) return;

            PlayerSetupPhase(hands[1]);
            if (hands.Count == 1) continue;
            DealerPhase();
            if (CheckMoney()) return;
        }
    }
    #endregion
}

#region Card Data Class
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
        name = Settings.values[value - 1] + " of " + Settings.suits[suit - 1];
        nameAbbrev = "[" + Settings.valueAbbrevs[value - 1] + Settings.suitAbbrevs[suit - 1] + "]";
    }
}
#endregion

#region Deck Data Class
public class Deck
{
    public List<Card> cards = [];

    public Deck()
    {
        for (int d = 0; d < Settings.numDecks; d++)
        {
            for (int s = 1; s <= Settings.numSuits; s++)
            {
                for (int v = 1; v <= Settings.numValues; v++)
                {
                    Card newCard = new(s, v);
                    cards.Add(newCard);
                }
            }
        }
    }

    public void Shuffle()
    {
        Random random = new();

        for (int i = 0; i < cards.Count; i++)
        {
            int randomIndex = random.Next(i, cards.Count);
            (cards[i], cards[randomIndex]) = (cards[randomIndex], cards[i]);
        }
    }

    public void Deal(Hand hand, int numCards = 1)
    {
        for (int i = 0; i < numCards; i++)
        {
            hand.cards.Add(cards[0]);
            cards.RemoveAt(0);
        }
        hand.Score();
    }

#endregion
}

#region Hand Data Class
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
            if (showOnlyFirst && i > 0) Log.Message("• Unknown Card [?]");
            else Log.Message("• " + card.name + " " + card.nameAbbrev);
        }
    }

    public enum Type
    {
        Blackjack, Safe, Bust
    }
}
#endregion

#region Console Class
public static class Log
{
    public static void Message(string message, bool animateText = true)
    {
        if (!Settings.noConsoleDelay) Delay();

        if (animateText)
        {
            foreach (char c in message)
            {
                Console.Write(c);
                if (!Settings.noConsoleDelay) Thread.Sleep(Settings.textSpeed);
                if (Console.KeyAvailable) Console.ReadKey(true); // Eat up keypresses while text is animating
            }
            Console.WriteLine();
        }
        else Console.WriteLine(message);
    }

    public static string GetPlayerInputString(string prompt, string[] options, char[]? startCharacters = null)
    {
        Message(prompt + "\n"); // Write prompt
        string? playerInput;
        while (true)
        {
            // Write options
            for (int i = 0; i < options.Length; i++)
            {
                string option = options[i];
                switch (Settings.responseShortcutMode)
                {
                    case ResponseShortcutMode.none:
                        {
                            Message("[" + option + "] ", false);
                            break;
                        }
                    case ResponseShortcutMode.letter:
                        {
                            Message("[" + option[0].ToString().ToUpper() + "] " + option, false);
                            break;
                        }
                    case ResponseShortcutMode.number:
                        {
                            Message("[" + i + "] " + option, false);
                            break;
                        }
                }
            }
            Console.Write("\n› ");

            playerInput = Console.ReadLine();
            playerInput ??= "";
            playerInput = playerInput.Trim().ToLower();
            if (startCharacters != null) playerInput.TrimStart(startCharacters);

            for (int i = 0; i < options.Length; i++)
            {
                string option = options[i].Trim().ToLower();

                if (playerInput == option) return option;
                else if (playerInput == option[0].ToString()) return option;
                else if (playerInput == i.ToString()) return option;
            }
            Message("\nInvalid input; Please try again\n");
        }
    }

    public static int GetPlayerInputInt(string prompt, char[]? startCharacters = null, char[]? endCharacters = null)
    {
        string? playerInputString;
        while (true)
        {
            // Write prompt & options
            Message(prompt);
            Console.Write("\n› ");

            playerInputString = Console.ReadLine();
            playerInputString ??= "";
            playerInputString = playerInputString.Trim().ToLower();
            if (startCharacters != null) playerInputString.TrimStart(startCharacters);
            if (endCharacters != null) playerInputString.TrimEnd(endCharacters);

            if (int.TryParse(playerInputString, out int playerInputInt)) return playerInputInt;
            else
            {
                Message("\nInvalid input; Please try again\n");
            }
        }
    }

    public static void Header(string header)
    {
        Clear();
        Message("|=-=| " + header.ToUpper() + " |=-=|\n", false);
        Delay(1);
    }

    public static void Delay(int delayCoefficient = 2)
    {
        if (!Settings.noConsoleDelay)
        {
            if (Console.KeyAvailable) Console.ReadKey(true); // Eat up keypresses while text is paused
            Thread.Sleep(Settings.lineDelay * delayCoefficient);
        }
    }

    public static void Clear()
    {
        if (!Settings.noConsoleClear) Console.Clear();
        else Message("\n-\n");
    }
}
#endregion

#region Public Enums
public enum GameOutcome
{
    win, lose, tie
}

public enum ResponseShortcutMode
{
    none, letter, number
}
#endregion
