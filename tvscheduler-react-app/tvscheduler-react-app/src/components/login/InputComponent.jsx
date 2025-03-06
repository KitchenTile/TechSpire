import { useState } from "react";
import "./InputComponent.css";

const InputComponent = ({ name, label, errorMessage }) => {
  const [isFilled, setIsFilled] = useState(false);
  const inputType = name === 'password' ? 'password' : 'text';

  const handleInputChange = (event) => {
    setIsFilled(event.target.value === "" ? false : true);
  };


  return (
    <div className={`input-field ${isFilled ? "filled" : ""} `}>
      <label htmlFor={name}>{label}</label>
      <input
        id={name}
        name={name}
        type={inputType}
        onChange={handleInputChange}
        className={errorMessage ? 'error' : ''} 
      />
      {errorMessage && <span className="error-message">{errorMessage}</span>}
    </div>
  );
};

export default InputComponent;
