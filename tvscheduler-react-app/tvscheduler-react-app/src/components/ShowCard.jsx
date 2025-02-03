// import { useEffect, useState } from "react";
import { useState } from "react";
import "./ShowCard.css";

const ShowCard = (show) => {
  const [expanded, setExpanded] = useState(false);

  const unixToHuman = new Date(show.show.startTime * 1000);
  console.log(unixToHuman.toLocaleString());

  const unixToHumanEnd = new Date(
    (show.show.startTime + show.show.duration) * 1000
  );
  console.log(unixToHumanEnd.toLocaleString());

  const endTime = show.startTime - show.duration;
  //this will be devided into several components

  console.log((show = show.show));
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
        <p className="show-description">
          {/* needs work */}
          {expanded ? show.description : show.description.slice(0, 80) + " "}
          <span className="read-more" onClick={() => setExpanded(!expanded)}>
            {expanded ? "Read Less..." : "Read More..."}
          </span>
        </p>
      </div>
      <button className="add-button">+</button>
    </div>
  );
};

export default ShowCard;
