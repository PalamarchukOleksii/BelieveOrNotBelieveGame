import {createContext, useContext, useEffect, useState} from "react";
import * as signalR from "@microsoft/signalr";

const SignalRContext = createContext(null);

export const SignalRProvider = ({children}) => {
    const [connection, setConnection] = useState(null);
    const [isConnected, setIsConnected] = useState(false);

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5175/game-hub")
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        setConnection(newConnection);
    }, []);

    useEffect(() => {
        if (!connection) return;

        const start = async () => {
            try {
                await connection.start();
                console.log("SignalR connected");
                setIsConnected(true);
            } catch (err) {
                console.error("SignalR connection failed:", err);
                setTimeout(start, 5000);
            }
        };

        start();

        return () => {
            connection.stop();
        };
    }, [connection]);

    return (
        <SignalRContext.Provider value={{connection, isConnected}}>
            {children}
        </SignalRContext.Provider>
    );
};

export const useSignalR = () => useContext(SignalRContext);
