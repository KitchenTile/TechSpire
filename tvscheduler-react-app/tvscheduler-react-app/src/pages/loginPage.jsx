import { useState } from "react";
import "./loginPage.css";
import InputComponent from "../components/login/InputComponent";

const LoginRegisterPage = () => {
  const [registered, setRegistered] = useState(false);

  const handleSubmit = (event) => {
    event.preventDefault();

    const formData = new FormData(event.target);
    console.log(Object.fromEntries(formData.entries()));
  };

  const inputs = [
    [
      { name: "fullName", label: "Full Name" },
      { name: "email", label: "Email" },
      { name: "username", label: "Username" },
      { name: "password", label: "Password" },
    ],
    [
      { name: "email", label: "Email" },
      { name: "password", label: "Password" },
    ],
  ];

  const handleLogin = (e) => {
    e.preventDefault();

    setRegistered(!registered);
  };

  return (
    <div className="login-page-container">
      {registered ? (
        <form
          className="login-form-container"
          id="createForm"
          onSubmit={handleSubmit}
        >
          <h1 className="form-title h1">Join us now!</h1>
          {inputs[0].map((input, index) => (
            <InputComponent key={index} name={input.name} label={input.label} />
          ))}

          <span> -- ♦ --</span>
          <button type="submit" className="button">
            Sign Up!
          </button>
          <p className="text-button">
            Already a member?
            <button className="login" onClick={handleLogin}>
              Login here!
            </button>
          </p>
          <div className="error-message" id="error message"></div>
        </form>
      ) : (
        <form
          className="login-form-container"
          id="createForm"
          onSubmit={handleSubmit}
        >
          <h1 className="form-title h1">Join us now!</h1>
          {inputs[1].map((input, index) => (
            <InputComponent key={index} name={input.name} label={input.label} />
          ))}

          <span> -- ♦ --</span>
          <button type="submit" className="button">
            Log In!
          </button>
          <p className="text-button">
            Don't have an account?{" "}
            <button className="login" onClick={handleLogin}>
              Register here!
            </button>
          </p>
          <div className="error-message" id="error message"></div>
        </form>
      )}
    </div>
  );
};

export default LoginRegisterPage;
