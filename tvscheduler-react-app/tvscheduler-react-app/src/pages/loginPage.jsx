import { useState } from "react";
import "./loginPage.css";
import InputComponent from "../components/login/InputComponent";

const LoginRegisterPage = () => {
  const [register, setRegister] = useState(false);
  const [username, setUsername] = useState(null);
  const [password, setPassword] = useState(null);
  const [email, setEmail] = useState(null);

  const handleSubmit = (event) => {
    event.preventDefault();

    const formData = new FormData(event.target);
    console.log(Object.fromEntries(formData.entries()));
  };

  const inputs = [
    { name: "fullName", label: "Full Name" },
    { name: "email", label: "Email" },
    { name: "username", label: "Username" },
    { name: "password", label: "Password" },
  ];

  return (
    <div className="page-container">
      <form className="form-container" id="createForm" onSubmit={handleSubmit}>
        <h1 className="form-title h1">Join us now!</h1>
        {inputs.map((input, index) => (
          <InputComponent key={index} name={input.name} label={input.label} />
        ))}

        <span> -- ♦ --</span>
        <button type="submit" className="button">
          Sign Up!
          {/* <svg
            width="100"
            height="100"
            viewBox="0 0 100 100"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <g id="arrow-right-normal">
              <path
                id="union-1"
                fill-rule="evenodd"
                clip-rule="evenodd"
                d="M51.2204 30.3871C52.8476 28.7599 55.4858 28.7599 57.1129 30.3871L73.7796 47.0537C75.4068 48.6809 75.4068 51.3191 73.7796 52.9463L57.1129 69.613C55.4858 71.2401 52.8476 71.2401 51.2204 69.613C49.5932 67.9858 49.5932 65.3476 51.2204 63.7204L60.7741 54.1667H29.1667C26.8655 54.1667 25 52.3012 25 50C25 47.6988 26.8655 45.8333 29.1667 45.8333H60.7741L51.2204 36.2796C49.5932 34.6524 49.5932 32.0142 51.2204 30.3871Z"
                fill="white"
              />
            </g>
          </svg> */}
        </button>
        <div className="error-message" id="error message"></div>
      </form>
    </div>
  );
};

export default LoginRegisterPage;
