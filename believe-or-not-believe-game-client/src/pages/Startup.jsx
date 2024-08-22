import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import JoinGame from "../components/JoinGame/JoinGame";

import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

function Startup({ connection }) {
  const [username, setUsername] = useState("");
  const [userJoin, setUserJoin] = useState(false);
  const navigate = useNavigate();

  const notifi = (msg) => {
    toast(msg);
  };

  useEffect(() => {
    try {
      connection.on("ReceiveJoin", (msg) => {
        console.log("msg: ", msg);
        navigate("/gametable");
      });

      connection.on("ReceiveNotJoin", (msg) => {
        console.log("msg: ", msg);
        notifi(msg);
      });
    } catch (e) {
      console.log(e);
    }

    return () => {
      if (connection && connection.off) {
        connection.off("ReceiveNotJoin");
        connection.off("ReceiveJoin");
      }
    };
  }, [userJoin]);

  const handleInputChange = (event) => {
    setUsername(event.target.value);
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    connection.invoke("JoinGameTable", username).catch((err) => {
      console.error("Error joining game:", err);
    });
    setUserJoin(true);
  };

  return (
    <>
      <JoinGame
        username={username}
        onSubmit={handleSubmit}
        onInputChange={handleInputChange}
      />
      <ToastContainer
        position="top-right"
        closeOnClick
        theme="dark"
        transition:Slide
      />
    </>
  );
}

export default Startup;
