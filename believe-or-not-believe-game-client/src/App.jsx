import React, { useEffect, useState } from "react";
import "./App.css";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import GameTable from "./pages/GameTable";
import Startup from "./pages/Startup";
import { Routes, Route } from "react-router-dom";

async function StartConnection() {
  try {
    const connection = new HubConnectionBuilder()
      .withUrl("https://localhost:7075/game-hub")
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect()
      .build();
    await connection.start();

    return connection;
  } catch (e) {
    console.log(e);
    return null;
  }
}

function App() {
  const [connection, setConnection] = useState(null);

  useEffect(() => {
    const setupConnection = async () => {
      const conn = await StartConnection();
      setConnection(conn);
    };

    setupConnection();

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
