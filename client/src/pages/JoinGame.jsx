import {useEffect, useState} from "react";
import {toast} from "react-toastify";
import {useNavigate} from "react-router-dom";
import {useSignalR} from "../contexts/SignalRContext.jsx";

function JoinGame() {
    const [username, setUsername] = useState("");
    const [gameName, setGameName] = useState("");
    const {connection, isConnected} = useSignalR();
    const navigate = useNavigate();

    useEffect(() => {
        if (!isConnected) return;

        const handleJoin = (message) => {
            toast.success(message || "Joined game successfully!");
            navigate("/gametable");
        };

        const handleNotJoin = (message) => {
            toast.error(message || "Failed to join game.");
        };

        connection.on("ReceiveJoin", handleJoin);
        connection.on("ReceiveNotJoin", handleNotJoin);

        return () => {
            connection.off("ReceiveJoin", handleJoin);
            connection.off("ReceiveNotJoin", handleNotJoin);
        };
    }, [connection, navigate]);

    const onSubmit = (e) => {
        e.preventDefault();
        if (!username) return toast("Enter username");
        if (!gameName) return toast("Enter game name");

        connection
            .invoke("JoinGameTable", username, gameName)
            .catch((err) => {
                console.error("Join game error:", err);
                toast.error("Error joining game.");
            });
    };

    return (
        <div className="flex items-center justify-center min-h-screen">
            <div className="w-full max-w-md p-4 rounded border">
                <h1 className="text-lg font-medium mb-4 text-center">Join the Game</h1>
                <form onSubmit={onSubmit} className="flex flex-col gap-3">
                    <input
                        type="text"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        placeholder="Username"
                        required
                        className="border rounded px-2 py-1"
                    />
                    <input
                        type="text"
                        value={gameName}
                        onChange={(e) => setGameName(e.target.value)}
                        placeholder="Game Name"
                        required
                        className="border rounded px-2 py-1"
                    />
                    <button
                        type="submit"
                        className="border rounded px-4 py-1"
                    >
                        Join
                    </button>
                </form>
            </div>
        </div>
    );
}

export default JoinGame;
