import { useContext, useState } from "react";
import "./SearchCard.css";
// import MyShowsContext from "../../contexts/myShowsContext";
import AddRemoveShowsContext from "../../contexts/AddRemoveShowsContext";
import MyShowsContext from "../../contexts/myShowsContext";

const SearchCard = ({ show, showEvents }) => {
  const [expanded, setExpanded] = useState(false);
  const { addRemoveShow } = useContext(AddRemoveShowsContext);
  const { myShows } = useContext(MyShowsContext);

  //   console.log(showEvents);

  const handleClick = () => {
    setExpanded(!expanded);
  };

  return (
    <div className={`search-card-container ${expanded ? "expanded" : ""}`}>
      <span className="img-container">
        <img
          src={`https://msaas.img.freeviewplay.net/cache/${show.imageUrl}`}
          alt={show.name}
          loading="lazy"
          decoding="async"
        />
      </span>
      <div className="info-container">
        <h2 className="show-title">
          {show.name.length > 20 ? show.name.slice(0, 20) + "..." : show.name}
        </h2>
        <div className="show-event-container">
          {showEvents.slice(0, 4).map((showEvent, idx) => (
            <div className="show-event-slot" key={idx}>
              {new Date(showEvent.timeStart * 1000).toLocaleTimeString(
                "en-GB",
                {
                  hour: "2-digit",
                  minute: "2-digit",
                }
              )}
              <button
                className="add-button small"
                onClick={() => addRemoveShow(showEvent.showEventId)}
              >
                {myShows.includes(showEvent.showEventId) ? "-" : "+"}
              </button>
            </div>
          ))}
        </div>
        <button className="expand-button" onClick={() => handleClick()}>
          <svg
            width="7.5"
            height="12"
            viewBox="0 0 22 37"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
            className="arrow-down"
          >
            <path
              d="M0 31.7143L13.75 18.5L0 5.28571L2.75 0L22 18.5L2.75 37L0 31.7143Z"
              fill="white"
            />
          </svg>
        </button>
      </div>
    </div>
  );
};

export default SearchCard;
