import { memo, useCallback, useState } from "react";
import "./Navigation.css";
import { Link } from "react-router-dom";

// Navigation is an expandable menu on the header; when hovered, it shows different navigation options
const Navigation = ({ mobile = false }) => {
  const [expanded, setExpanded] = useState(false);

  const links = ["Channels", "Morning", "Afternoon", "Evening", "All"];

  const mouseEnter = useCallback(() => setExpanded(true), []);
  const mouseLeave = useCallback(() => setExpanded(false), []);

  return (
    <li
      className={`navigation-container ${expanded ? "expanded" : ""} ${
        mobile ? "mobile" : ""
      }`}
      onMouseEnter={mouseEnter}
      onMouseLeave={mouseLeave}
    >
      <span>Explore</span>
      <div className="navigation-links">
        {links.map((link, index) => (
          <Link to={`/explore/${link}`} className="link" key={index}>
            {link}
          </Link>
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

export default memo(Navigation);
