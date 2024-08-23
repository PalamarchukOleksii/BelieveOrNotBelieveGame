import React from "react";
import "./OpponentsCard.css";

function OpponentsCard({ count }) {
  const marginRight = -60 + 150 / (count - 1) - 60 / (count - 1);

  return (
    <div className="opponent-cards">
      {Array.from({ length: count }, (_, index) => (
        <div>
          <img
            key={index}
            src="src/assets/CardBack.svg"
            alt="Card"
            className="opponents-card-image"
            style={{ marginRight }}
          />
        </div>
      ))}
    </div>
  );
}

export default OpponentsCard;
