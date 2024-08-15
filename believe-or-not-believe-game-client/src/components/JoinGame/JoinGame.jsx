import React, { useState } from "react";
import "./JoinGame.css";
import { useNavigate } from "react-router-dom";

function JoinGame({ connection }) {
  const [username, setUsername] = useState("");
  const navigate = useNavigate();

  const handleInputChange = (event) => {
    setUsername(event.target.value);
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    try {
      connection.on("ReceiveJoin", (msg) => {
        console.log("msg: ", msg);
        navigate("/gametable");
      });

      connection.on("ReceiveNotJoin", (msg) => {
        console.log("msg: ", msg);
        alert(msg);
      });

      await connection.invoke("JoinGameTable", username);
    } catch (e) {
      console.log("Failed to receive join method", e);
    }
  };

  return (
    <div className="join">
      <h1>Welcome to Believe or Not Believe game</h1>
      <p>To join the game, enter your username</p>
      <form className="username-form" onSubmit={handleSubmit}>
        <input
          type="text"
          value={username}
          onChange={handleInputChange}
          placeholder="Username"
          required
        />
        <button type="submit">Join</button>
      </form>
    </div>
  );
}

export default JoinGame;
