using Domain.Enums;

namespace Domain.Models.GameModels
{
    public class Player
    {
        public string PlayerConnectionId { get; private set; } = string.Empty;

        public string Name { get; private set; } = string.Empty;

        public List<PlayingCard> PlayersCards { get; private set; } = new List<PlayingCard>();

        public bool StartGame { get; private set; } = false;

        public BotDificulty BotDifficulty { get; private set; } = BotDificulty.ItIsNotABot;

        public bool IsBot { get; private set; } = false;

        public Player() { }

        public Player(string playerConnectionId, string name)
        {
            PlayerConnectionId = playerConnectionId;
            Name = name;
        }

        public Player(string playerConnectionId, string name, bool startGame, BotDificulty botDifficulty, bool isBot, List<PlayingCard> playersCards)
        {
            PlayerConnectionId = playerConnectionId;
            Name = name;
            StartGame = startGame;
            BotDifficulty = botDifficulty;
            IsBot = isBot;
            PlayersCards = playersCards;
        }

        public void OnGameEnd()
        {
            StartGame = false;
            PlayersCards = new List<PlayingCard>();
        }

        public bool CheckIfPlayerHaveSomeCards(IReadOnlyList<int> cardsId)
        {
            int count = 0;
            foreach (int id in cardsId)
            {
                foreach (PlayingCard card in PlayersCards)
                {
                    if (Convert.ToInt32(id) == card.Id)
                    {
                        count++;
                    }
                }
            }

            if (count == cardsId.Count)
            {
                return true;
            }

            return false;
        }

        public void SetStartGame(bool startGame)
        {
            StartGame = startGame;
        }
    }
}
