import { useState } from "react";
import "./loginPage.css";
import InputComponent from "../components/login/InputComponent";
import { useNavigate } from "react-router-dom";

const LoginRegisterPage = () => {
  const navigate = useNavigate();
  const [registered, setRegistered] = useState(true);
  const [errorMessages, setErrorMessages] = useState({});

  const validateInput = (loginData) => {
    const errors = {};
    // if (!loginData.email || !loginData.email.includes("@")) {
    //   errors.email = "*Please enter a valid email";
    // }
    if (
      !registered &&
      (!loginData.username ||
        loginData.username.length < 3 ||
        loginData.username.length > 10)
    ) {
      errors.username = "*Username should be 3-10 characters long";
    }
    if (registered) {
      if (
        loginData.password.length < 8 ||
        !/\d/.test(loginData.password) ||
        !/[!@#$%^&*]/.test(loginData.password)
      ) {
        errors.password = "*Please enter a correct password";
      }
    } else {
      if (
        loginData.password.length < 8 ||
        !/\d/.test(loginData.password) ||
        !/[!@#$%^&*]/.test(loginData.password)
      ) {
        errors.password =
          "*Password should be at least 8 characters, including 1 number and 1 special character";
      }
    }
    return errors;
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    console.log(registered);

    const formData = new FormData(event.target);
    const loginData = Object.fromEntries(formData.entries());
    console.log(loginData);

    const validationErrors = validateInput(loginData);
    if (Object.keys(validationErrors).length > 0) {
      setErrorMessages(validationErrors);
      return;
    }

    try {
      const response = await fetch(
        registered
          ? "http://localhost:5171/Account/login"
          : "http://localhost:5171/register",
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(loginData),
        }
      );

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || "Failed to post data");
      }

      const data = await response.json();
      const JWToken = data.token;
      if (JWToken) {
        localStorage.setItem("JWToken", JWToken);
        console.log(JWToken);
        navigate("/main");
      } else {
        console.error("No JWT");
      }
      setErrorMessages({}); // Clear error messages on successful login/register
    } catch (error) {
      console.error("Login/Register error: ", error);
      setErrorMessages({ form: error.message });
    }
  };

  const toggleRegistered = () => {
    setRegistered(!registered);
    setErrorMessages({}); // Clear error messages when toggling between login and register
  };

  const inputs = [
    [
      {
        name: "email",
        label: "Email",
        errorMessage: errorMessages.email,
      },
      {
        name: "username",
        label: "Username",
        errorMessage: errorMessages.username,
      },
      {
        name: "password",
        label: "Password",
        errorMessage: errorMessages.password,
        type: "password",
      },
    ],
    [
      { name: "name", label: "Email", errorMessage: errorMessages.email },
      {
        name: "password",
        label: "Password",
        errorMessage: errorMessages.password,
        type: "password",
      },
    ],
  ];

  return (
    <div className="login-page-container">
      {!registered ? (
        <form
          className="login-form-container"
          id="createForm"
          onSubmit={handleSubmit}
        >
          <h1 className="form-title h1">Join us now!</h1>
          {inputs[0].map((input, index) => (
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
            Sign Up!
          </button>
          <p className="text-button">
            Already a member?
            <button className="login" onClick={toggleRegistered}>
              Login here!
            </button>
          </p>
          {errorMessages.form && (
            <div className="error-message">{errorMessages.form}</div>
          )}
        </form>
      ) : (
        <form
          className="login-form-container"
          id="createForm"
          onSubmit={handleSubmit}
        >
          <h1 className="form-title h1">Join us now!</h1>
          {inputs[1].map((input, index) => (
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
            Log In!
          </button>
          <p className="text-button">
            Don't have an account?{" "}
            <button className="login" onClick={toggleRegistered}>
              Register here!
            </button>
          </p>
          {errorMessages.form && (
            <div className="error-message">{errorMessages.form}</div>
          )}
        </form>
      )}
    </div>
  );
};

export default LoginRegisterPage;
