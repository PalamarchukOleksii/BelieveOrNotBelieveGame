import React, { useEffect, useState } from "react";
import "./CardHeap.css";

function CardHeap({ count, style = {} }) {
  const [cards, setCards] = useState([]);

  useEffect(() => {
    const containerHeight = Number(style.height) || 200;
    const containerWidth = Number(style.width) || 200;

    const generateRandomPosition = () => ({
      top: Math.random() * (containerHeight - 100),
      left: Math.random() * (containerWidth - 70),
    });

    if (count == 0) {
      setCards([]);
    } else {
      setCards((prevCards) => {
        const newCards = Array.from({ length: count - prevCards.length }).map(
          (_, index) => {
            const position = generateRandomPosition();
            return {
              style: {
                top: `${position.top}px`,
                left: `${position.left - Number(style.left)}px`,
                transform: `rotate(${Math.random() * 90 - 45}deg)`,
                zIndex: prevCards.length + index,
              },
            };
          }
        );

        return [...prevCards, ...newCards];
      });
    }
  }, [count]);

  return (
    <div className="card-heap" style={style}>
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
