import CardValue from "../CardValue/CardValue";
import "./ChooseCardValue.css";

function ChooseCardValue({ values }) {
  return (
    <div className="choose-value">
      {values.map((value) => (
        <CardValue key={value} value={value} />
      ))}
    </div>
  );
}

export default ChooseCardValue;
