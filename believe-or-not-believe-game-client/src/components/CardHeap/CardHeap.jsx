import React, { useEffect, useState } from "react";
import "./CardHeap.css";

function CardHeap({ count }) {
  const [cards, setCards] = useState([]);

  useEffect(() => {
    const container = document.querySelector(".card-heap");

    if (!container) return;

    const containerSize = 200;

    const generateRandomPosition = () => ({
      top: Math.random() * (containerSize - 100),
      left: Math.random() * (containerSize - 70),
    });

    const newCards = Array.from({ length: count }).map((_, index) => {
      const position = generateRandomPosition();
      return {
        style: {
          top: `${position.top}px`,
          left: `${position.left}px`,
          transform: `rotate(${Math.random() * 90 - 45}deg)`,
          zIndex: index,
        },
      };
    });

    setCards(newCards);
  }, [count]);

  return (
    <div className="card-heap">
      {cards.map((card, index) => (
        <img
          key={index}
          src="src/assets/CardBack.svg"
          alt="Card"
          className="card-image"
          style={card.style}
        />
      ))}
    </div>
  );
}

export default CardHeap;
