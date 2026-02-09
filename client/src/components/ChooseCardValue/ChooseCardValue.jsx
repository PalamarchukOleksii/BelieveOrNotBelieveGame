import "./ChooseCardValue.css";

function ChooseCardValue({ cardValue, values, onSelectValue }) {
  return (
    <>
      <select
        className="card-value"
        value={cardValue}
        onChange={onSelectValue}
        required
      >
        {typeof values === "string" ? (
          <option value={values}>{values}</option>
        ) : (
          <>
            <option value="">Choose card value</option>
            {values.map((value) => (
              <option key={value} value={value}>
                {value}
              </option>
            ))}
          </>
        )}
      </select>
    </>
  );
}

export default ChooseCardValue;
