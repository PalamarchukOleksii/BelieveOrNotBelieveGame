import "./LastMovePanel.css";

function LastMovePanel({ info }) {
  return (
    <div className="info-panel">
      <h1>Last move:</h1>
      <p>{info}</p>
    </div>
  );
}

export default LastMovePanel;
