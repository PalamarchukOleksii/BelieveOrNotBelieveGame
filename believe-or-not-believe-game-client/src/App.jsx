import {Route, Routes} from "react-router-dom";
import GameTable from "./pages/GameTable";
import Landing from "./pages/Landing.jsx";
import JoinGame from "./pages/JoinGame.jsx";
import CreateGame from "./pages/CreateGame.jsx";
import {ToastContainer} from "react-toastify";
import {SignalRProvider} from "./contexts/SignalRContext.jsx";

function App() {
    return (
        <SignalRProvider>
            <Routes>
                <Route path="/" element={<Landing/>}/>
                <Route path="/gametable" element={<GameTable/>}/>
                <Route path="/join" element={<JoinGame/>}/>
                <Route path="/create" element={<CreateGame/>}/>
            </Routes>
            <ToastContainer/>
        </SignalRProvider>
    );
}

export default App;
