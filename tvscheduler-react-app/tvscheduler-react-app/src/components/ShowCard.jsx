import { useEffect, useState } from "react";
import "./ShowCard.css";

const ShowCard = ({ show, addShow }) => {
  const [expanded, setExpanded] = useState(false);

  // Unix conversion
  const unixToHuman = new Date(show.startTime * 1000);
  const unixToHumanEnd = new Date((show.startTime + show.duration) * 1000);

  const readMore = () => {
    setExpanded(!expanded);
  };

  return (
    <div className="card-container">
      <span className="img-container">
        <img
          src={`https://msaas.img.freeviewplay.net/cache/${show.image}`}
          alt={show.name}
        />
      </span>
      <div className="info-container">
        <h2 className="show-title">{show.name}</h2>
        <div className="time-stamps">
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
        </div>
        <p className={`show-description ${expanded ? "expanded" : ""}`}>
          {/* needs work */}
          {expanded ? show.description : show.description.slice(0, 80) + " "}
          <span className="read-more" onClick={readMore}>
            {expanded ? "" : "Read More..."}
          </span>
          <span className="cross" onClick={readMore}></span>
        </p>
      </div>
      <button className="add-button" onClick={() => addShow(show.evtId)}>
        +
      </button>
    </div>
  );
};

export default ShowCard;
