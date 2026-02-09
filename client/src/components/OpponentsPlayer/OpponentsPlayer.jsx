import OpponentsCard from "../OpponentsCard/OpponentsCard";
import "./OpponentsPlayer.css";

function OpponentsPlayer({ username, cardCount }) {
  return (
    <div className="opponent-player">
      <h4 className="username">{username}</h4>
      <OpponentsCard count={cardCount} />
    </div>
  );
}

export default OpponentsPlayer;
