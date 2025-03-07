import { useState } from "react";
import "./SearchCard.css";

const SearchCard = ({ show, showEvents }) => {
  const [expanded, setExpanded] = useState(false);

  console.log(showEvents);

  const handleClick = () => {
    setExpanded(!expanded);
  };

  return (
    <div class={`search-card-container ${expanded ? "expanded" : ""}`}>
      <span className="img-container">
        <img
          src={`https://msaas.img.freeviewplay.net/cache/${show.imageUrl}`}
          alt={show.name}
          loading="lazy"
          decoding="async"
        />
        {/* <LowResImgHandler
                highResSrc={`https://msaas.img.freeviewplay.net/cache/${show.imageUrl}`}
                alt={show.name}
                // style={{ width: "100%", height: "auto" }}
              /> */}
      </span>
      <div className="info-container">
        <h2 className="show-title">{show.name}</h2>
        {/* <div className="time-stamps">
          <span className="from">
            {" "}
            {unixToHuman.toLocaleTimeString("en-GB", {
              hour: "2-digit",
              minute: "2-digit",
            })}
          </span>{" "}
          -{" "}
          <span className="to">
            {unixToHumanEnd.toLocaleTimeString("en-GB", {
              hour: "2-digit",
              minute: "2-digit",
            })}
          </span>
        </div> */}
        {showEvents.slice(0, 4).map((show) => show.showEventId)}
        <button className="add-button" onClick={() => handleClick()}>
          <svg
            width="7.5"
            height="12"
            viewBox="0 0 22 37"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
            class="arrow-down"
          >
            <path
              d="M0 31.7143L13.75 18.5L0 5.28571L2.75 0L22 18.5L2.75 37L0 31.7143Z"
              fill="white"
            />
          </svg>
        </button>
      </div>
      {/* <button
        className="add-button small"
        tooltip-text={isAdded ? "Remove show" : "Add to schedule"}
        onClick={handleAddShow}
      >
        {isAdded ? "-" : "+"}
      </button> */}
    </div>
  );
};

export default SearchCard;
