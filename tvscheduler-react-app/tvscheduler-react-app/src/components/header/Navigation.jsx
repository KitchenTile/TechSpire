import { useState } from "react";
import "./Navigation.css";

const Navigation = () => {
  const [expanded, setExpanded] = useState(false);

  const links = ["Channels", "Morning", "Afternoon", "Evening", "All"];

  const handleClick = () => {
    setExpanded(!expanded);
  };

  return (
    <li
      className={`navigation-container ${expanded ? "expanded" : ""}`}
      onMouseEnter={handleClick}
      onMouseLeave={handleClick}
    >
      <span>Explore</span>
      <div className="navigation-links">
        {links.map((link, index) => (
          <span className="link" key={index}>
            {link}
          </span>
        ))}
      </div>
      <svg
        width="5.5"
        height="12"
        viewBox="0 0 22 37"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
        className="expand-arrow"
      >
        <path
          d="M0 31.7143L13.75 18.5L0 5.28571L2.75 0L22 18.5L2.75 37L0 31.7143Z"
          fill="white"
        />
      </svg>
    </li>
  );
};

export default Navigation;
