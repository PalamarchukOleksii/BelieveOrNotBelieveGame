import React from "react";
import GameCard from "../GameCard/GameCard";
import "./PlayersCard.css";

function PlayersCard({ cards }) {
  const marginRight =
    -168 + 1000 / (cards.length - 1) - 168 / (cards.length - 1);

  return (
    <div className="palyer-cards">
      {cards?.map((card) => (
        <GameCard
          key={card.id}
          cardSuit={card.cardSuit}
          cardValue={card.cardValue}
          marginRight={marginRight}
        />
      ))}
    </div>
  );
}

export default PlayersCard;
