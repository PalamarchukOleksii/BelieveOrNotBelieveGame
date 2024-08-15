import "./GameCard.css";

function GameCard({ cardValue, cardSuit, marginRight }) {
  return (
    <div>
      <img
        src={`/src/assets/${cardSuit}/${cardValue}.svg`}
        alt={`${cardValue} ${cardSuit}`}
        className="card"
        style={{ marginRight }}
      />
    </div>
  );
}

export default GameCard;
