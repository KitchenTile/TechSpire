import React from "react";
import "./WarningIcon.css";

const WarningIcon = ({ children }) => {
  return (
    <div className="warning-container">
      <h1 className="title">!</h1>
      <p>{children}</p>
    </div>
  );
};

export default WarningIcon;
