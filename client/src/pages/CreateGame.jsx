import {useEffect, useState} from "react";
import {toast} from "react-toastify";
import {useNavigate} from "react-router-dom";
import {useSignalR} from "../contexts/SignalRContext.jsx";

function CreateGame() {
    const [username, setUsername] = useState("");
    const [gameName, setGameName] = useState("");
    const [numOfCards, setNumOfCards] = useState("");
    const [maxNumOfPlayers, setMaxNumOfPlayers] = useState();
    const [addBot, setAddBot] = useState(false);
    const {connection, isConnected} = useSignalR();
    const navigate = useNavigate();

    useEffect(() => {
        if (!isConnected) return;

        const handleGameCreated = () => toast.success("Game created successfully!");
        const handleGameNotCreated = (msg) => toast.error(`Game not created: ${msg}`);
        const handleJoin = () => navigate("/gametable");
        const handleNotJoin = (msg) => toast.error(`Failed to join game: ${msg}`);

        connection.on("RecieveNewGameCreated", handleGameCreated);
        connection.on("RecieveNewGameNotCreated", handleGameNotCreated);
        connection.on("ReceiveJoin", handleJoin);
        connection.on("ReceiveNotJoin", handleNotJoin);

        return () => {
            connection.off("RecieveNewGameCreated", handleGameCreated);
            connection.off("RecieveNewGameNotCreated", handleGameNotCreated);
            connection.off("ReceiveJoin", handleJoin);
            connection.off("ReceiveNotJoin", handleNotJoin);
        };
    }, [connection, navigate]);

    const onSubmit = (e) => {
        e.preventDefault();
        if (!username) return toast("Enter username");
        if (!numOfCards) return toast("Select number of cards");
        if (!maxNumOfPlayers || maxNumOfPlayers < 2 || maxNumOfPlayers > 10) {
            return toast("Max players must be between 2 and 10");
        }

        const createDto = {
            CreatorName: username,
            GameName: gameName || `${username}'s game`,
            NumOfCards: numOfCards,
            MaxNumOfPlayers: maxNumOfPlayers,
            AddBot: addBot,
        };

        connection.invoke("CreateGame", createDto).catch((err) => {
            console.error("Create error:", err);
            toast.error("Error creating game.");
        });
    };

    return (
        <div className="flex items-center justify-center min-h-screen">
            <div className="w-full max-w-md p-4 rounded border">
                <h1 className="text-lg font-medium mb-4 text-center">
                    Create a Game
                </h1>
                <form onSubmit={onSubmit} className="flex flex-col gap-3">
                    <input
                        type="text"
                        name="username"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        placeholder="Username"
                        required
                        className="border rounded px-2 py-1"
                    />

                    <input
                        type="text"
                        name="gameName"
                        value={gameName}
                        onChange={(e) => setGameName(e.target.value)}
                        placeholder="Game Name"
                        className="border rounded px-2 py-1"
                    />

                    <select
                        name="numOfCards"
                        value={numOfCards}
                        onChange={(e) => setNumOfCards(e.target.value)}
                        required
                        className="border rounded px-2 py-1"
                    >
                        <option value="">Select number of cards</option>
                        <option value="24">24 Cards</option>
                        <option value="36">36 Cards</option>
                        <option value="52">52 Cards</option>
                    </select>

                    <input
                        type="number"
                        name="maxNumOfPlayers"
                        value={maxNumOfPlayers}
                        onChange={(e) => setMaxNumOfPlayers(e.target.value)}
                        min="2"
                        max="10"
                        placeholder="Max Number of Players"
                        required
                        className="border rounded px-2 py-1"
                    />

                    <label className="flex items-center gap-2">
                        <input
                            type="checkbox"
                            checked={addBot}
                            onChange={(e) => setAddBot(e.target.checked)}
                        />
                        Add Bot
                    </label>

                    <button
                        type="submit"
                        className="border rounded px-4 py-1"
                    >
                        Create Game
                    </button>
                </form>
            </div>
        </div>
    );
}

export default CreateGame;
