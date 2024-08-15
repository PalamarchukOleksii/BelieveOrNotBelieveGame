import { useState, useEffect } from "react";
import PlayersCard from "../components/PlayersCard/PlayersCard";
import ChooseCardValue from "../components/ChooseCardValue/ChooseCardValue";
import Opponents from "../components/Opponents/Opponents";
import MakeAssume from "../components/MakeAssume/MakeAssume";
import MakeMove from "../components/MakeMove/MakeMove";
import StartGame from "../components/StartGame/StartGame";
import CardHeap from "../components/CardHeap/CardHeap";

const exampleValues = [
  "2",
  "3",
  "4",
  "5",
  "6",
  "7",
  "8",
  "9",
  "10",
  "Jack",
  "Queen",
  "King",
];

function GameTable({ connection }) {
  const [gameStarted, setGameStarted] = useState(false);
  const [cards, setCards] = useState([]);
  const [cardsOnTableCount, setCardsOnTableCount] = useState(0);
  const [cardsForMove, setCardsForMove] = useState([]);

  useEffect(() => {
    try {
      connection.on("ReceiveCard", (cards) => {
        console.log(cards);
        setCards(cards);
      });

      connection.on("ReceiveStart", (msg) => {
        console.log(msg);
        alert(msg);
      });

      connection.on("ReceiveNotStart", (msg) => {
        console.log(msg);
        alert(msg);
        setGameStarted(false);
      });
    } catch (e) {
      console.log(e);
    }

    return () => {
      if (connection && connection.off) {
        connection.off("ReceiveCard");
        connection.off("ReceiveStart");
        connection.off("ReceiveNotStart");
      }
    };
  }, [connection, gameStarted]);

  const handleStartGame = (numOfCard) => {
    connection.invoke("StartGame", numOfCard).catch((err) => {
      console.error("Error starting game:", err);
    });
    setGameStarted(true);
  };

  const handleCardClick = (card) => {
    setCardsForMove((prevCardsForMove) => {
      if (prevCardsForMove.some((c) => c.id === card.id)) {
        return prevCardsForMove.filter((c) => c.id !== card.id);
      } else {
        return [...prevCardsForMove, card];
      }
    });
  };

  return (
    <>
      <Opponents connection={connection} />
      {!gameStarted ? (
        <StartGame onStart={handleStartGame} />
      ) : (
        <>
          <CardHeap count={cardsOnTableCount} />
          <ChooseCardValue values={exampleValues} />
          <MakeAssume />
          <PlayersCard
            cards={cards}
            onCardClick={handleCardClick}
            cardsForMove={cardsForMove}
          />
          <MakeMove />
        </>
      )}
    </>
  );
}

export default GameTable;
