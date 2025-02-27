import { useState } from "react";
import "./InputComponent.css";

const InputComponent = ({ name, label, errorMessage }) => {
  const [isFilled, setIsFilled] = useState(false);
  const inputType = name === 'password' ? 'password' : 'text';

  const handleInputChange = (event) => {
    setIsFilled(event.target.value === "" ? false : true);
  };

  const handleError = () => {
    //write down logic for error handling once we have the info on how the error's are passed
  };

  return (
    <div className={`input-field ${isFilled ? "filled" : ""} `}>
      <label htmlFor={name}>{label}</label>
      <input
        id={name}
        name={name}
        type={inputType}
        onChange={handleInputChange}
        onBlur={handleError}
      />
    </div>
  );
};

export default InputComponent;
