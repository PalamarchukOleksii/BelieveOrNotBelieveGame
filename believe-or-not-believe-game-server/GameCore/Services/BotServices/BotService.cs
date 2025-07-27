// using GameCore.Abstractions.BotAbstractions;
// using GameCore.Common.Helpers;
// using GameCore.Enums;
// using GameCore.Models.BotModels;
// using GameCore.Models.GameModels;
// using GameCore.Sessions;
//
// namespace GameCore.Services.BotServices;
//
// public class BotService : IBotService
// {
//     private readonly IBotFirstMoveService _botFirstMoveService;
//     private readonly IBotNotFirstMoveService _botNotFirstMoveService;
//
//     public BotService(IBotFirstMoveService botFirstMoveService, IBotNotFirstMoveService botNotFirstMoveService)
//     {
//         _botNotFirstMoveService = botNotFirstMoveService;
//         _botFirstMoveService = botFirstMoveService;
//     }
//
//     public BotResponse MakeMove(GameSession gameTable)
//     {
//         var bot = gameTable.CurrentMovePlayer;
//
//         if (!bot.IsBot || bot.BotDifficulty == BotDificulty.ItIsNotABot)
//         {
//             throw new ProblemDetailsException(
//                 400,
//                 $"Error! BotModels can't move instead of player {bot.Name}");
//         }
//
//         var botInfo = ConfigureBotInfo(gameTable);
//
//         var isNotFirstMove = gameTable.CardsOnTable.Any();
//
//         if (isNotFirstMove && gameTable.Move is null)
//         {
//             throw new ProblemDetailsException(400, "Move can't be null");
//         }
//
//         if (isNotFirstMove)
//         {
//             return _botNotFirstMoveService.MakeNotFirstMove(botInfo);
//         }
//         else
//         {
//             return _botFirstMoveService.MakeFirstMove(botInfo);
//         }
//     }
//
//     private BotInfo ConfigureBotInfo(GameSession gameTable)
//     {
//         var botDifficulty = gameTable.CurrentMovePlayer.BotDifficulty;
//
//         var players = gameTable.Players
//             .Where(p => p.Name != gameTable.CurrentMovePlayer.Name)
//             .Select(p =>
//             {
//                 var playersCards = RandomHelper.GetRandomCardsFromListByBotDificulty(p.PlayersCards, botDifficulty);
//
//                 return new Player
//                 (
//                     p.PlayerConnectionId,
//                     p.Name,
//                     p.StartGame,
//                     p.BotDifficulty,
//                     p.IsBot,
//                     playersCards.ToList()
//                 );
//             }).ToList()
//             ?? new List<Player>();
//
//         var cardForDiscard = RandomHelper.GetRandomCardsFromListByBotDificulty(gameTable.CardsForDiscard, botDifficulty);
//
//         return new BotInfo(
//             gameTable.CurrentMovePlayer,
//             players,
//             cardForDiscard,
//             gameTable.Move!,
//             gameTable.PreviousMovePlayer.Name,
//             gameTable.NextMovePlayer.Name
//             );
//     }
// }

