import { useState } from "react";
import "./loginPage.css";
import InputComponent from "../components/login/InputComponent";
import { useNavigate } from "react-router-dom";

const LoginRegisterPage = () => {
  const navigate = useNavigate();
  const [registered, setRegistered] = useState(true);
  const [validPassword, setValidPassword] = useState(false);

  const handleSubmit = async (event) => {
    event.preventDefault();
    console.log(registered);

    const formData = new FormData(event.target);
    const loginData = Object.fromEntries(formData.entries());

    console.log(Object.fromEntries(formData.entries()));

    if (registered) {
      try {
        const response = await fetch("http://localhost:5171/Account/login", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(loginData),
        });

        if (!response.ok) {
          throw new Error("Failed to post data 1");
        }
        console.log("Login succesfull");
        const data = await response.json();
        console.log(data);
        const JWToken = data.token;
        if (JWToken) {
          localStorage.setItem("JWToken", JWToken);
          console.log(JWToken);
          navigate("/main");
        } else {
          console.error("No JWT");
        }
      } catch (error) {
        console.error("Login error: ", error);
      }
    } else if (!registered) {
      try {
        const response = await fetch("http://localhost:5171/register", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(loginData),
        });

        if (!response.ok) {
          throw new Error("Failed to post data");
        }
        console.log("Register succesfull");
        const data = await response.json();
        console.log(data);
        const JWToken = data.token;
        if (JWToken) {
          localStorage.setItem("JWToken", JWToken);
          console.log(JWToken);
        } else {
          console.error("No JWT");
        }
      } catch (error) {
        console.error("Login error: ", error);
      }
    }
  };

  const inputs = [
    [
      {
        name: "email",
        label: "Email",
        errorMessage: "Please enter a valid email",
      },
      {
        name: "username",
        label: "Username",
        errorMessage: "Username should be 3-10 characters long",
      },
      {
        name: "password",
        label: "Password",
        errorMessage:
          "Password should be at least 8 characters, including 1 number and 1 special character",
      },
    ],
    [
      { name: "name", label: "Email" },
      { name: "password", label: "Password" },
    ],
  ];

  const handleLogin = (e) => {
    e.preventDefault();

    setRegistered(!registered);
    console.log(registered);
  };

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
            />
          ))}

          <span> -- ♦ --</span>
          <button type="submit" className="button" onClick={handleLogin}>
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
