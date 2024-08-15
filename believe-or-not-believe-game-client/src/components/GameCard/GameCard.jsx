import "./GameCard.css";

function GameCard({ cardValue, cardSuit, marginRight, onClick, isSelected }) {
  return (
    <div>
      <img
        src={`/src/assets/${cardSuit}/${cardValue}.svg`}
        alt={`${cardValue} ${cardSuit}`}
        className={`card ${isSelected ? "selected" : ""}`}
        style={{ marginRight }}
        onClick={onClick}
      />
    </div>
  );
}

export default GameCard;
