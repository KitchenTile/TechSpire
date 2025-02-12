import { useState } from "react";
import "./ShowCard.css";

const ShowCard = ({ show, addShow, isAdded }) => {
  const [expanded, setExpanded] = useState(false);

  // Unix conversion
  const unixToHuman = new Date(show.startTime * 1000);
  const unixToHumanEnd = new Date((show.startTime + show.duration) * 1000);

  // Manage description tags
  const descriptionTags = ["HD", "S", "AD", "SL"];

  const formattedDescription = show.description.replace(
    //insane regex lmao
    /\[([^\]]+)\]/g,
    (_, tags) =>
      tags
        .split(",")
        .map((tag) => `[${tag.trim()}]`)
        .join(" ")
  );

  const checkActiveTags = descriptionTags.map((tag) => ({
    tag,
    active: formattedDescription.includes(`[${tag}]`),
  }));

  const readMore = () => {
    setExpanded(!expanded);
  };

  const addShowFunction = () => {
    addShow(show.evtId);
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
          {expanded
            ? show.description.split("[", 1)
            : show.description.slice(0, 80).split("[", 1) + " "}
          <span className="read-more" onClick={readMore}>
            {expanded ? "" : "Read More..."}
          </span>
          <span className="cross" onClick={readMore}></span>
          <div className="tags">
            {checkActiveTags.map(({ tag, active }) => (
              <span
                className={`littleTag small ${active ? "" : "disabled"}`}
                id={tag}
                key={tag}
              >
                {tag}
              </span>
            ))}
          </div>
        </p>
      </div>
      <button
        className="add-button small"
        tooltip-text={isAdded ? "Remove show" : "Add to schedule"}
        onClick={addShowFunction}
      >
        {isAdded ? "-" : "+"}
      </button>
    </div>
  );
};

export default ShowCard;
