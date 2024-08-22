import "./JoinGame.css";

function JoinGame({ username, onInputChange, onSubmit }) {
  return (
    <div className="join">
      <h1>Welcome to Believe or Not Believe game</h1>
      <p>To join the game, enter your username</p>
      <form className="username-form" onSubmit={onSubmit}>
        <input
          type="text"
          value={username}
          onChange={onInputChange}
          placeholder="Username"
          required
        />
        <button className="join-button" type="submit">
          Join
        </button>
      </form>
    </div>
  );
}

export default JoinGame;
