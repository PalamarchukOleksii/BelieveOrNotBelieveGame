import React, { useState, useEffect } from "react";
import PlayersCard from "../components/PlayersCard/PlayersCard";
import Opponents from "../components/Opponents/Opponents";
import MakeAssume from "../components/MakeAssume/MakeAssume";
import MakeMove from "../components/MakeMove/MakeMove";
import StartGame from "../components/StartGame/StartGame";
import CardHeap from "../components/CardHeap/CardHeap";

import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import LastMovePanel from "../components/LastMovePanel/LastMovePanel";

function GameTable({ connection }) {
  const [gameStarted, setGameStarted] = useState(false);
  const [cards, setCards] = useState([]);
  const [cardsOnTableCount, setCardsOnTableCount] = useState(0);
  const [cardsForMove, setCardsForMove] = useState([]);
  const [valueForMove, setValueForMove] = useState("");
  const [canMakeMove, setCanMakeMove] = useState(false);
  const [canMakeAssume, setCanMakeAssume] = useState(false);
  const [cardsInDiscardCount, setCardsInDiscardCount] = useState(0);
  const [canChooseValue, setCanChooseValue] = useState(false);
  const [makeMoveValues, setMakeMoveValues] = useState([]);
  const [lastMoveInfo, setLastMoveInfo] = useState("");

  const notifi = (msg) => {
    toast(msg);
  };

  useEffect(() => {
    try {
      connection.on("ReceiveCard", (cards) => {
        console.log(cards);
        setCards(cards);
      });

      connection.on("ReceiveStartMove", (msg) => {
        console.log(msg);
        notifi(msg);
        setGameStarted(true);
        setCanMakeMove(true);
        setCanChooseValue(true);
        setCanMakeAssume(false);
      });

      connection.on("ReceiveNotStart", (msg) => {
        console.log(msg);
        notifi(msg);
        setGameStarted(false);
      });

      connection.on("ReceiveCardOnTableCount", (count) => {
        console.log("Card on table count: ", count);
        setCardsOnTableCount(count);
      });

      connection.on("RecieveMove", (msg) => {
        setCanMakeMove(false);
        setCanMakeAssume(false);
        setCanChooseValue(false);
        console.log(msg);
        notifi(msg);

        setLastMoveInfo(msg);

        setCardsForMove([]);

        const lastWord = msg.split(" ").pop();
        setValueForMove(lastWord);
      });

      connection.on("ReceiveNextMoveAssume", (msg) => {
        console.log(msg);
        notifi(msg);
        setCanMakeMove(true);
        setCanMakeAssume(true);
        setCanChooseValue(false);
      });

      connection.on("ReceiveNextAssume", (msg) => {
        console.log(msg);
        notifi(msg);
        setCanMakeAssume(true);
        setCanMakeMove(false);
        setCanChooseValue(false);
      });

      connection.on("ReceiveGameOver", (msg) => {
        console.log(msg);
        notifi(msg);
        setGameStarted(false);
        setCards([]);
        setCardsOnTableCount(0);
        setCardsForMove([]);
        setValueForMove("");
        setCanMakeMove(false);
        setCanMakeAssume(false);
        setCardsInDiscardCount(0);
        setCanChooseValue(false);
        setLastMoveInfo("");
      });

      connection.on("ReceiveAssume", (msg) => {
        console.log(msg);
        notifi(msg);
        setValueForMove("");
        setCardsForMove([]);
        setCanMakeAssume(false);
        setCanMakeMove(false);
        setCanChooseValue(false);
        setLastMoveInfo("");
      });

      connection.on("RecieveNotMove", (msg) => {
        console.log(msg);
        notifi(msg);
      });

      connection.on("ReceiveFirstMove", (msg) => {
        console.log(msg);
        notifi(msg);
        setCanMakeMove(true);
        setCanChooseValue(true);
        setCanMakeAssume(false);
      });

      connection.on("ReceiveDiscardCardsCount", (count) => {
        console.log("Card in discard count: ", count);
        setCardsInDiscardCount(count);
      });

      connection.on("ReceiveMakeMoveValues", (msg) => {
        setMakeMoveValues(msg);
      });

      connection.on("ReceiveCurrentMovePlayer", (msg) => {
        notifi(msg);
      });

      connection.on("ReceiveJoin", (msg) => {
        notifi(msg);
      });
    } catch (e) {
      console.log(e);
    }

    return () => {
      if (connection && connection.off) {
        connection.off("ReceiveCard");
        connection.off("ReceiveStartMove");
        connection.off("ReceiveNotStart");
        connection.off("ReceiveCardOnTableCount");
        connection.off("RecieveMove");
        connection.off("ReceiveNextMoveAssume");
        connection.off("ReceiveGameOver");
        connection.off("ReceiveFirstMove");
        connection.off("RecieveNotMove");
        connection.off("ReceiveAssume");
        connection.off("ReceiveNextAssume");
        connection.off("ReceiveDiscardCardsCount");
        connection.off("ReceiveMakeMoveValues");
        connection.off("ReceiveCurrentMovePlayer");
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

  const handleChooseValue = (event) => {
    setValueForMove(event.target.value);
  };

  const handleMove = () => {
    const cardsId = cardsForMove.map((element) => element.id).join(" ");
    if (cardsId && valueForMove) {
      connection
        .invoke("MakeMove", valueForMove, cardsId)
        .catch((err) => console.error("Error when making move:", err));
    }
  };

  const handleAssume = (assume) => {
    connection.invoke("MakeAssume", assume).catch((err) => {
      console.error("Error when making assume:", err);
    });
  };

  return (
    <>
      <Opponents connection={connection} />
      <ToastContainer
        position="top-right"
        closeOnClick
        theme="dark"
        transition:Slide
      />
      {lastMoveInfo && <LastMovePanel info={lastMoveInfo} />}
      {!gameStarted ? (
        <StartGame onStart={handleStartGame} />
      ) : (
        <>
          <CardHeap
            count={cardsOnTableCount}
            style={{
              position: "relative",
              display: "flex",
              width: "200px",
              height: "200px",
            }}
          />
          <CardHeap
            count={cardsInDiscardCount}
            style={{
              position: "fixed",
              display: "flex",
              width: "100px",
              height: "200px",
              left: "-50px",
              top: "50%",
              transform: "translateY(-50%)",
            }}
          />
          {canMakeMove && (
            <MakeMove
              onMove={handleMove}
              values={canChooseValue ? makeMoveValues : valueForMove}
              onSelectValue={handleChooseValue}
              cardValue={valueForMove}
            />
          )}
          {canMakeAssume && <MakeAssume onAssume={handleAssume} />}
          <PlayersCard
            cards={cards}
            onCardClick={handleCardClick}
            cardsForMove={cardsForMove}
          />
        </>
      )}
    </>
  );
}

export default GameTable;
