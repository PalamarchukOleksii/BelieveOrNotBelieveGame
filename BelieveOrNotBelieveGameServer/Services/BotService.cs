using BelieveOrNotBelieveGameServer.Common.Constants;
using BelieveOrNotBelieveGameServer.Models;
using BelieveOrNotBelieveGameServer.Models.BotModels;
using BelieveOrNotBelieveGameServer.Services.Abstraction;
using System.Xml;

namespace BelieveOrNotBelieveGameServer.Services;

public class BotService : IBotService
{
    private decimal probapility = 0;

    public void MakeMove(GameTable gameTable)
    {
        var previousPlayer = gameTable.CurrentMovePlayer;
        var nextPlayer = gameTable.NextMovePlayer;
        var currentPlayer = gameTable.CurrentMovePlayer;

        if(!currentPlayer.IsBot || currentPlayer.BotDifficulty == BotDificulty.ItIsNotABot)
        {
            throw new Exception($"Error! Bot can't move instead of player {currentPlayer.Name}");
        }

        (var otherPlayers, var cardForDiscard) = ConfigureBotInfo(gameTable);

        var isNotFirstMove = gameTable.CardsOnTable.Any();
    }

    private (List<Player> otherPlayers, List<PlayingCard> cardForDiscard) ConfigureBotInfo(GameTable gameTable)
    {
        var botDifficulty = gameTable.CurrentMovePlayer.BotDifficulty;

        var players = gameTable.Players
            .Where(p => p.Name != gameTable.CurrentMovePlayer.Name)
            .Select(p =>
            {
                var playersCards = GetRandomCardsFromListByBotDificulty(p.PlayersCards, botDifficulty);

                return new Player
                {
                    PlayerConnectionId = p.PlayerConnectionId,
                    Name = p.Name,
                    StartGame = p.StartGame,
                    BotDifficulty = p.BotDifficulty,
                    IsBot = p.IsBot,
                    PlayersCards = playersCards.ToList(),
                };
            }).ToList() 
            ?? new List<Player>();
        
        var cardForDiscard = GetRandomCardsFromListByBotDificulty(gameTable.CardsForDiscard, botDifficulty);

        return (players, cardForDiscard);
    }

    private List<PlayingCard> GetRandomCardsFromListByBotDificulty(List<PlayingCard> cards, BotDificulty botDificulty)
    {
        var botVisibility = MapBotVisibilityByDificulty(botDificulty);
                
        var numberOfCardsThatVisibleToBot = (int)(botVisibility * cards.Count);
            
        var playersCards = new HashSet<PlayingCard>();
        
        Random rnd = new Random();
        
        for (int i = 0; i<numberOfCardsThatVisibleToBot; i++)
        {
            int index = rnd.Next(cards.Count);
            var card = cards[index];
            if (!playersCards.Add(card))
            {
                i--;
            }
        }

        return playersCards.ToList();
    }

    private double MapBotVisibilityByDificulty(BotDificulty botDificulty)
    {
        return botDificulty switch 
        {
            BotDificulty.Easy => BotConstants.EasyDificultyVisibility,
            BotDificulty.Middle => BotConstants.MiddleDificultyVisibility,
            BotDificulty.Hard => BotConstants.HardDificultyVisibility,
            BotDificulty.VeryHard => BotConstants.VeryHardDificultyVisibility,
            _ => 0
        };
    }
}
