using BelieveOrNotBelieveGameServer.Common.Constants;
using BelieveOrNotBelieveGameServer.Common.Helpers;
using BelieveOrNotBelieveGameServer.Models;
using BelieveOrNotBelieveGameServer.Models.BotModels;
using BelieveOrNotBelieveGameServer.Services.Abstraction;

namespace BelieveOrNotBelieveGameServer.Services.BotServices;

public class BotNotFirstMoveService : IBotNotFirstMoveService
{
    public BotResponse MakeNotFirstMove(Player bot, List<Player> otherPlayers, List<PlayingCard> cardForDiscard, GameTable gameTable)
    {
        var rnd = new Random();

        var myCards = bot.PlayersCards;
        var move = gameTable.Move!;
        var cardValue = move.CardValue!;
        var cardsNumber = move.CardsId.Count;

        var allPlayersCards = otherPlayers.SelectMany(x => x.PlayersCards).ToList();

        return MakeDecision(bot, allPlayersCards, myCards, cardForDiscard, cardValue, cardsNumber);
    }

    private BotResponse MakeDecision(
        Player bot, 
        List<PlayingCard> allPlayersCards, 
        List<PlayingCard> myCards, 
        List<PlayingCard> cardForDiscard,
        string cardValue, 
        int countOfCardsInLastMove)
    {
        if (myCards.Any(c => c.Value == cardValue) 
            || allPlayersCards.Any(c => c.Value == cardValue)
            || cardForDiscard.Any(c => c.Value == cardValue))
        {
            var mySameCardsCount = myCards.Where(c => c.Value == cardValue).Count();
            var allPlayersSameCardsCount = allPlayersCards.Where(c => c.Value == cardValue).Count();
            var cardForDiscardSameCardsCount = cardForDiscard.Where(c => c.Value == cardValue).Count();
            
            return DesideToAssumeOrMakeMove(
                bot, myCards, cardValue, 
                mySameCardsCount + allPlayersSameCardsCount + countOfCardsInLastMove + cardForDiscardSameCardsCount > CardConstants.MaxCardsInOneMove);
        }
        else
        {
            return GenerateRandomAssume();
        }
    }

    private BotResponse DesideToAssumeOrMakeMove(
        Player bot, 
        List<PlayingCard> myCards, 
        string cardValue,         
        bool lieIsMostPosibleAssume)
    {
        var isAssuming = RandomHelper.GenerateRandomBool();

        if(isAssuming && lieIsMostPosibleAssume)
        {
            return new BotResponse(isBelieve: !lieIsMostPosibleAssume, isNotBelieve: lieIsMostPosibleAssume); 
        }
        else if(isAssuming)
        {
            return GenerateRandomAssume();
        }

        var isToTellTruth = RandomHelper.GenerateRandomBool();
        var cardsForMove = new List<PlayingCard>();

        if(isToTellTruth && myCards.Any(c => c.Value == cardValue) )
        {
            cardsForMove = myCards.Where(x => x.Value == cardValue).ToList();
        }
        else
        {
            int numberOfCardsForMove = 1;
                
            var coeficientOfNumber = RandomHelper.GenerateRandomInt(10, 1);
            if(coeficientOfNumber >= 5 && coeficientOfNumber < 9)
            {
                numberOfCardsForMove = 2;
            }
            else if (coeficientOfNumber == 9)
            {
                numberOfCardsForMove = 3;
            }
            else if (coeficientOfNumber == 10)
            {
                numberOfCardsForMove = 4;
            }

            cardsForMove = RandomHelper.GetRandomCardsFromList(myCards, numberOfCardsForMove);
        }

        return new BotResponse
            (
                new Move
                (
                    bot.Name,
                    cardValue,
                    cardsForMove.Select(x => x.Id).ToList()
                )
            );
    }

    private BotResponse GenerateRandomAssume()
    {
        var isBelieve = RandomHelper.GenerateRandomBool();
        return new BotResponse(isBelieve , !isBelieve);
    }
}
