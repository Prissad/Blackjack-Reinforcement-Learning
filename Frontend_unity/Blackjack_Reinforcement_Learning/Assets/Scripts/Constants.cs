namespace Assets.Scripts
{
    class Constants
    {
        // Scene Name
        public static string blackjackTableScene = "Blackjack Table";
        // Names for GameObject
        public static string deck = "Deck";
        public static string player = "Player";
        public static string dealer = "Dealer";
        public static string firstCard = "Card1";
        public static string secondCard = "Card2";
        public static string cardInitial = "Card";
        // Game Execution Constants
        public static int attemptsGetValue = 5;


        // Game Constants
        public static int charlieRuleCardLimit = 5;
        public static int dealerMinStickValue = 17;
        public static int blackjackValue = 21;
        public static int bustValue = 22;
        // Game Result Messages
        public static string playerBust = "Player goes Bust! Dealer Wins!";
        public static string dealerBust = "Dealer goes Bust! Player Wins!";
        public static string playerWin = "Player Wins!";
        public static string dealerWin = "Dealer Wins!";
        public static string tie = "It's a Tie. Nobody Wins.";
        public static string playerBlackjack = "Player got a Blackjack! Player Wins!";


        // Deck Constants
        public static int numberColors = 4;
        public static int numberCardsPerColor = 13;
        public static int numberCardsDeck = numberCardsPerColor * numberColors + 1;
        // Card Constants
        public static int aceValue = 11;
        public static int jackQueenKingValue = 10;
        public static int minValue = 2;
        public static int maxValue = aceValue;
        public static int cardBackValue = 0;


        // Sprites position in Deck
        public static int acePosition = 9;
        public static int jackPosition = 10;
        public static int kingPosition = 11;
        public static int queenPosition = 12;
        public static int backCardPosition = numberCardsDeck - 1;
        // Sprites position in Hands
        public static float cardPositionZ = 0;
        public static float positionStepX = 2;
        public static float positionStepY = 0;


        // Player Constants
        public static int initialHandValue = 0;
        public static int initialPlayerMoney = 1000;        
        // Dealer Constants
        public static int hiddenCardPositionHand = 0;



    }
}
