namespace BelieveOrNotBelieveGameServer.Models
{
    public class CardsDeck
    {
        public int NumOfCard { get; set; }
        public List<PlayingCard> Cards { get; set; }

        public CardsDeck(int numOfCard)
        {
            NumOfCard = numOfCard;
            Cards = new List<PlayingCard>();
            if (numOfCard == 24)
            {
                Generate24CardsDeck();
            }
            else if (numOfCard == 36)
            {
                Generate36CardsDeck();
            }
            else if (numOfCard == 52)
            {
                Generate52CardsDeck();
            }
            else
            {
                throw new ArgumentException("Unsupported number of cards");
            }
        }

        public void ShuffleCards()
        {
            Random rnd = new Random();
            for(int i = 0; i < NumOfCard; i++)
            {
                int j = rnd.Next(0,NumOfCard-1);
                (Cards[i], Cards[j]) = (Cards[j], Cards[i]);
            }
        }

        void Generate24CardsDeck()
        {
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            string[] values = { "9", "10", "Jack", "Queen", "King", "Ace" };

            int count = 1;

            foreach (var suit in suits)
            {
                foreach (var value in values)
                {
                    Cards.Add(new PlayingCard { Id = count++, CardSuit = suit, CardValue = value });
                }
            }
        }

        void Generate36CardsDeck()
        {
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            string[] values = { "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };

            int count = 1;

            foreach (var suit in suits)
            {
                foreach (var value in values)
                {
                    Cards.Add(new PlayingCard { Id = count++, CardSuit = suit, CardValue = value });
                }
            }
        }

        void Generate52CardsDeck()
        {
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            string[] values = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };

            int count = 1;

            foreach (var suit in suits)
            {
                foreach (var value in values)
                {
                    Cards.Add(new PlayingCard { Id = count++, CardSuit = suit, CardValue = value });
                }
            }
        }

        public void GiveCardsToPlayers(List<Player> players)
        {
            int numCardForOnePlayer = NumOfCard / players.Count;
            int c = 0;
            for (int i = 0; i < numCardForOnePlayer / 2; i++)
            {
                for (int j = 0; j < players.Count; j++)
                {
                    players[j].PlayersCards.Add(Cards[c++]);
                    players[j].PlayersCards.Add(Cards[c++]);
                }
            }
        }
    }
}
