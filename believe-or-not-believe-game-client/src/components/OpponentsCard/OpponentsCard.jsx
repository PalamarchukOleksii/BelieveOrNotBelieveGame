import React from "react";
import "./OpponentsCard.css";

const CARDSIZE = 60;

function OpponentsCard({ count }) {
  const marginRight = -CARDSIZE + 150 / (count - 1) - CARDSIZE / (count - 1);

  return (
    <div className="opponent-cards">
      {Array.from({ length: count }, (_, index) => (
        <img
          key={index}
          src="src/assets/CardBack.svg"
          alt="Card"
          className="opponents-card-image"
          style={{ marginRight }}
        />
      ))}
    </div>
  );
}

export default OpponentsCard;
