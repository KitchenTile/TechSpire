import { useState } from "react";
import "./loginPage.css";
import InputComponent from "../components/login/InputComponent";
import { Link } from "react-router-dom";

const LoginRegisterPage = () => {
  const [registered, setRegistered] = useState(true);
  const [errorMessage, setErrorMessage] = useState("");

  const validateInput = (loginData) => {
    if (!loginData.email || !loginData.email.includes("@")) {
      return "Please enter a valid email";
    }
    if (loginData.password.length < 8 || !/\d/.test(loginData.password) || !/[!@#$%^&*]/.test(loginData.password)) {
      return "Password should be at least 8 characters, including 1 number and 1 special character";
    }
    if(loginData.username.length < 3 || loginData.username.length > 10){
      return "Username should be 3-10 characters long";
    }
    return null;
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    console.log(registered);

    const formData = new FormData(event.target);
    const loginData = Object.fromEntries(formData.entries());
    console.log(loginData);

    const validationError = validateInput(loginData);
    if (validationError) {
      setErrorMessage(validationError);
      return;
    }
    console.log(Object.fromEntries(formData.entries()));

    if (registered) {
      try {
        const response = await fetch("http://localhost:5171/Account/login", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(loginData),
        });

        if (!response.ok) {
          const errorData = await response.json();
          throw new Error(errorData.message || "Failed to post data");
        }
        console.log("Login succesfull");
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
    } else if (!registered) {
      try {
        const response = await fetch("http://localhost:5171/register", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(loginData),
        });

        if (!response.ok) {
          const errorData = await response.json();
          throw new Error(errorData.message || "Failed to post data");
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
        setErrorMessage(""); // Clear error message on successful login/register
      } catch (error) {
        console.error("Login/Register error: ", error);
        setErrorMessage(error.message);
      }
    }
  };

  // const validateJWT = async (token) => {
  //   try{
  //     const response = await fetch("http://localhost:5171/login")
  //   }
  // }

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
    setErrorMessage(""); // Clear error message when toggling between login and register
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
          <button type="submit" className="button">
            Sign Up!
          </button>
          <p className="text-button">
            Already a member?
            <button className="login" onClick={handleLogin}>
              Login here!
            </button>
          </p>v
          {errorMessage && <div className="error-message">{errorMessage}</div>}
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
          {/* <Link as={Link} to={"/main"}> */}
          <button type="submit" className="button">
            Log In!
          </button>
          {/* </Link> */}
          <p className="text-button">
            Don't have an account?{" "}
            <button className="login" onClick={handleLogin}>
              Register here!
            </button>
          </p>
          {errorMessage && <div className="error-message">{errorMessage}</div>}
        </form>
      )}
    </div>
  );
};

export default LoginRegisterPage;