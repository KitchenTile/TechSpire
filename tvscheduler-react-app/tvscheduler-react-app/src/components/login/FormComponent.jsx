import InputComponent from "./InputComponent";
import "./FormComponent.css" ;
import "./InputComponent.css";

const RegisterForm = ({ inputs, title, buttonText, toggleQuestion, toggleText, onSubmit, toggleAction, errorMessages,  }) => {
  return (
    <form className="login-form-container" id="createForm" onSubmit={onSubmit}>
      <h1 className="form-title h1">
        {title}
      </h1>
      {inputs.map((input, index) => (
        <InputComponent
          key={index}
          name={input.name}
          label={input.label}
          errorMessage={input.errorMessage}
          type={input.type}
        />
      ))}
      <span> -- ♦ --</span>
      <button type="submit" className="button">
        {buttonText}
      </button>
      <p className="text-button">
        {toggleQuestion}{" "}
        <button type="button" className="login" onClick={toggleAction}>
            {toggleText}
        </button>
      </p>
      {errorMessages.form && <div className="error-message">{errorMessages.form}</div>}
    </form>
  );
};

export default RegisterForm;