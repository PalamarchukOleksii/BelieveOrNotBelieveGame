import {useNavigate} from "react-router-dom";

function Landing() {
    const navigate = useNavigate();

    return (
        <div className="flex items-center justify-center min-h-screen">
            <div className="flex flex-col gap-4 rounded p-6 w-full max-w-sm">
                <h1 className="text-xl font-semibold text-center">Welcome to Believe or Not Believe</h1>
                <p className="text-center">Please choose an option:</p>
                <div className="flex justify-between">
                    <button
                        className="px-4 py-2 border rounded"
                        onClick={() => navigate("/join")}
                    >
                        Join Game
                    </button>
                    <button
                        className="px-4 py-2 border rounded"
                        onClick={() => navigate("/create")}
                    >
                        Create Game
                    </button>
                </div>
            </div>
        </div>
    );
}

export default Landing;
