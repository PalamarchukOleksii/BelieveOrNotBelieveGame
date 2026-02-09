import "./MakeAssume.css";

function MakeAssume({ onAssume }) {
  return (
    <div className="make-assume">
      <h2>Make assume</h2>
      <button className="assume-button" onClick={() => onAssume(true)}>
        Believe
      </button>
      <button className="assume-button" onClick={() => onAssume(false)}>
        Not Believe
      </button>
    </div>
  );
}

export default MakeAssume;
