import JoinGame from "../components/JoinGame/JoinGame";

function Startup({ connection }) {
  return (
    <div>
      <JoinGame connection={connection} />
    </div>
  );
}

export default Startup;
