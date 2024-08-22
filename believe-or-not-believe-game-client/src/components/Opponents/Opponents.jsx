import OpponentsPlayer from "../OpponentsPlayer/OpponentsPlayer";
import "./Opponents.css";
import { useState, useEffect } from "react";

function Opponents({ connection }) {
  const [opponentsInfo, setOpponentsInfo] = useState([]);

  useEffect(() => {
    if (!connection) {
      console.error("Connection is undefined or null.");
      return;
    }

    connection.on("ReceiveOpponentsInfo", (opponents) => {
      console.log("Received opponents info:", opponents);
      setOpponentsInfo(opponents);
    });

    try {
      connection.invoke("SendInfoAboutOpponents");
    } catch (e) {
      console.error("Error while setting up connection listener:", e);
    }

    return () => {
      if (connection && connection.off) {
        connection.off("ReceiveOpponentsInfo");
      }
    };
  }, [connection]);

  return (
    <div className="opponents">
      {opponentsInfo.map((opponent) => (
        <OpponentsPlayer
          key={opponent.name}
          username={opponent.name}
          cardCount={opponent.cardCount}
        />
      ))}
    </div>
  );
}

export default Opponents;
