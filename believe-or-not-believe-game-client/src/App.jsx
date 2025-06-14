import React, { useEffect, useState } from "react";
import { Routes, Route } from "react-router-dom";
import { HubConnectionBuilder } from "@microsoft/signalr";
import GameTable from "./pages/GameTable";
import Startup from "./pages/Startup";
import "./App.css";

function StartConnection() {
  try {
    const connection = new HubConnectionBuilder()
      .withUrl("http://localhost:5175/game-hub")
      .build();

    connection.start().catch((err) => console.error(err));

    return connection;
  } catch (e) {
    console.log(e);
    return null;
  }
}

function App() {
  const [connection, setConnection] = useState(null);

  useEffect(() => {
    const conn = StartConnection();
    setConnection(conn);

    return () => {
      if (connection) {
        connection.stop();
      }
    };
  }, []);

  return (
    <Routes>
      <Route path="/" element={<Startup connection={connection} />} />
      <Route
        path="/gametable"
        element={<GameTable connection={connection} />}
      />
    </Routes>
  );
}

export default App;
