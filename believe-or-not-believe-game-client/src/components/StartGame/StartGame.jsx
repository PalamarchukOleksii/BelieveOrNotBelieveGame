import React, { useState } from "react";
import "./StartGame.css";

function StartGame({ onStart }) {
  const [cardCount, setCardCount] = useState("24");

  const handleSelectChange = (event) => {
    setCardCount(event.target.value);
  };

  const handleStartClick = () => {
    onStart(cardCount);
  };

  return (
    <div className="start-game">
      <label>Choose count of cards</label>
      <select
        className="card-count"
        value={cardCount}
        onChange={handleSelectChange}
        required
      >
        <option value="24">24</option>
        <option value="36">36</option>
        <option value="52">52</option>
      </select>
      <button type="button" className="start-button" onClick={handleStartClick}>
        Start game
      </button>
    </div>
  );
}

export default StartGame;
