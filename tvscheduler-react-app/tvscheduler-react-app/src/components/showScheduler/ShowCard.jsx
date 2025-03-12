import { memo, useContext, useEffect, useState } from "react";
import "./ShowCard.css";
import useIntersectionObserver from "../../hooks/useIntersectionObserver";
import DummyCardLoading from "../loadingComponents/DummyCardLoading";
import AddRemoveShowsContext from "../../contexts/AddRemoveShowsContext";

const ShowCard = ({ show, isAdded, rowRef, style = null }) => {
  const [expanded, setExpanded] = useState(false);
  const { addRemoveShow } = useContext(AddRemoveShowsContext);

  console.log(`card ${show.name} redering` + new Date().toISOString());

  //options for intersection observer custom hook
  const options = {
    // imporntat: we use the parent ref as the ref in our options so that the lazy loading
    //  is NOT relative to the window, we add the conditional to avoid undefined error
    //  when adding shows

    root: rowRef ? rowRef.current : null,
    threshold: 0,
    rootMargin: "250px",
  };

  const [cardRef, isVisible] = useIntersectionObserver(options);

  // Unix conversion
  const unixToHuman = new Date(show.timeStart * 1000);
  const unixToHumanEnd = new Date((show.timeStart + show.duration) * 1000);

  // Manage description tags
  const descriptionTags = ["HD", "S", "AD", "SL"];

  const formattedDescription = show.description.replace(
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

  const handleAddShow = () => {
    addRemoveShow(show.showEventId);
  };

  return (
    <div className="card-container" style={style} ref={cardRef}>
      {isVisible ? (
        <>
          <span className="img-container">
            <img
              src={`https://msaas.img.freeviewplay.net/cache/${show.imageUrl}`}
              alt={show.name}
              loading="lazy"
              decoding="async"
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
            <div className={`show-description ${expanded ? "expanded" : ""}`}>
              {expanded
                ? show.description.split("[", 1)
                : show.description.slice(0, 70).split("[", 1) + " "}
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
            </div>
          </div>
          <button
            className="add-button small"
            tooltip-text={isAdded ? "Remove show" : "Add to schedule"}
            onClick={handleAddShow}
          >
            {isAdded ? "-" : "+"}
          </button>
        </>
      ) : (
        <DummyCardLoading />
      )}
    </div>
  );
};

const areEqual = (prevProps, nextProps) => {
  return (
    prevProps.show.showEventId === nextProps.show.showEventId &&
    prevProps.isAdded === nextProps.isAdded
  );
};

export default memo(ShowCard, areEqual);
