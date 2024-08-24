using BelieveOrNotBelieveGameServer.Models.BotModels;

namespace BelieveOrNotBelieveGameServer.Models
{
    public class Player
    {
        public string PlayerConnectionId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public List<PlayingCard> PlayersCards { get; set; } = new List<PlayingCard>();

        public bool StartGame { get; set; } = false;

        public BotDificulty BotDifficulty { get; set; } = BotDificulty.ItIsNotABot;

        public bool IsBot { get; set; } = false;
    }
}
