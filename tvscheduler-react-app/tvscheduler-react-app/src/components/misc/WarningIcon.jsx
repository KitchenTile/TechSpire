import React from "react";
import "./WarningIcon.css";

// compoenet that expands to the side with a warning -- Rudraa
const WarningIcon = ({ children, position = null }) => {
  return (
    <div
      className={`warning-container ${
        position === "vertical" ? "vertical" : ""
      }`}
    >
      <h1 className="title">!</h1>
      <p>{children}</p>
    </div>
  );
};

export default WarningIcon;
