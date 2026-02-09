import "./MakeMove.css";
import ChooseCardValue from "../ChooseCardValue/ChooseCardValue";

function MakeMove({ onMove, onSelectValue, values, cardValue }) {
  return (
    <div className="make-move">
      <h2>Make move</h2>
      <ChooseCardValue
        onSelectValue={onSelectValue}
        values={values}
        cardValue={cardValue}
      />
      <button className="move-button" onClick={onMove}>
        Move
      </button>
    </div>
  );
}

export default MakeMove;
