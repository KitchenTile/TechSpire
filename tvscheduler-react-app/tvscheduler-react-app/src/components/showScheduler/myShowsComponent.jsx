import ShowCard from "./ShowCard";
import rightArrow from "../../assets/rightArrow.svg";
import { useContext, useMemo, useState } from "react";
import useMergeAndFilter from "../../hooks/useMergeAndFilter";
import ChannelsContext from "../../contexts/channelsContext";
import MyShowsContext from "../../contexts/myShowsContext";
import "./MyShowsComponent.css";

const MyShowsComponent = ({ position = "horizontal" }) => {
  const channels = useContext(ChannelsContext);
  const { myShows } = useContext(MyShowsContext);
  const mergeShows = useMergeAndFilter("All");
  const [expanded, setExpanded] = useState(false);

  //this function replaces previous
  const mergeAndSort = useMemo(() => {
    const filtered = mergeShows?.filter((event) =>
      myShows.includes(event.showEventId)
    );

    return filtered;
  }, [channels, myShows]);

  const handleClick = () => {
    setExpanded(true);
  };

  return (
    <div
      className={
        position === "vertical"
          ? `vertical-container ${expanded ? "expanded" : ""}`
          : ""
      }
    >
      {position === "vertical" ? (
        expanded ? (
          <div className="close-bttn" onClick={() => setExpanded(false)}></div>
        ) : (
          <div className="icon-container" onClick={() => setExpanded(true)}>
            <svg
              width="32"
              height="32"
              viewBox="0 0 32 32"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
              className="my-shows-icon"
            >
              <path
                fill-rule="evenodd"
                clip-rule="evenodd"
                d="M10.6668 9.3335C9.93045 9.3335 9.3335 9.93045 9.3335 10.6668C9.3335 11.4032 9.93045 12.0002 10.6668 12.0002H16.0002C16.7365 12.0002 17.3335 11.4032 17.3335 10.6668C17.3335 9.93045 16.7365 9.3335 16.0002 9.3335H10.6668ZM12.0002 16.0002C12.0002 15.2638 12.5971 14.6668 13.3335 14.6668H18.6668C19.4032 14.6668 20.0002 15.2638 20.0002 16.0002C20.0002 16.7365 19.4032 17.3335 18.6668 17.3335H13.3335C12.5971 17.3335 12.0002 16.7365 12.0002 16.0002ZM14.6668 21.3335C14.6668 20.5971 15.2638 20.0002 16.0002 20.0002H21.3335C22.0699 20.0002 22.6668 20.5971 22.6668 21.3335C22.6668 22.0699 22.0699 22.6668 21.3335 22.6668H16.0002C15.2638 22.6668 14.6668 22.0699 14.6668 21.3335Z"
                fill="white"
              />
              <path
                fill-rule="evenodd"
                clip-rule="evenodd"
                d="M13.8096 4H18.1904C19.6355 3.99999 20.801 3.99998 21.7448 4.07709C22.7166 4.15649 23.5702 4.32424 24.3599 4.72662C25.6144 5.36578 26.6342 6.38565 27.2734 7.64006C27.6758 8.42979 27.8435 9.28337 27.9229 10.2552C28 11.199 28 12.3645 28 13.8096V18.1904C28 19.6355 28 20.801 27.9229 21.7448C27.8435 22.7166 27.6758 23.5702 27.2734 24.3599C26.6342 25.6144 25.6144 26.6342 24.3599 27.2734C23.5702 27.6758 22.7166 27.8435 21.7448 27.9229C20.801 28 19.6355 28 18.1904 28H13.8096C12.3645 28 11.199 28 10.2552 27.9229C9.28337 27.8435 8.42979 27.6758 7.64006 27.2734C6.38565 26.6342 5.36578 25.6144 4.72662 24.3599C4.32424 23.5702 4.15649 22.7166 4.07709 21.7448C3.99998 20.801 3.99999 19.6355 4 18.1904V13.8096C3.99999 12.3645 3.99998 11.199 4.07709 10.2552C4.15649 9.28337 4.32424 8.42979 4.72662 7.64006C5.36578 6.38565 6.38565 5.36578 7.64006 4.72662C8.42979 4.32424 9.28337 4.15649 10.2552 4.07709C11.199 3.99998 12.3645 3.99999 13.8096 4ZM10.4723 6.7349C9.66543 6.80083 9.20185 6.92373 8.85071 7.10264C8.09806 7.48614 7.48614 8.09806 7.10264 8.85071C6.92373 9.20185 6.80083 9.66543 6.7349 10.4723C6.6677 11.2948 6.66667 12.3512 6.66667 13.8667V18.1333C6.66667 19.6488 6.6677 20.7052 6.7349 21.5277C6.80083 22.3346 6.92373 22.7982 7.10264 23.1493C7.48614 23.9019 8.09806 24.5139 8.85071 24.8974C9.20185 25.0763 9.66543 25.1992 10.4723 25.2651C11.2948 25.3323 12.3512 25.3333 13.8667 25.3333H18.1333C19.6488 25.3333 20.7052 25.3323 21.5277 25.2651C22.3346 25.1992 22.7982 25.0763 23.1493 24.8974C23.9019 24.5139 24.5139 23.9019 24.8974 23.1493C25.0763 22.7982 25.1992 22.3346 25.2651 21.5277C25.3323 20.7052 25.3333 19.6488 25.3333 18.1333V13.8667C25.3333 12.3512 25.3323 11.2948 25.2651 10.4723C25.1992 9.66543 25.0763 9.20185 24.8974 8.85071C24.5139 8.09806 23.9019 7.48614 23.1493 7.10264C22.7982 6.92373 22.3346 6.80083 21.5277 6.7349C20.7052 6.6677 19.6488 6.66667 18.1333 6.66667H13.8667C12.3512 6.66667 11.2948 6.6677 10.4723 6.7349Z"
                fill="white"
              />
            </svg>
          </div>
        )
      ) : null}
      <h1 className="title h1">My Shows</h1>
      <div
        className={`myshow-container ${
          position === "vertical" ? "vertical" : ""
        }`}
      >
        {/* if my shows is not empty, flatten the show per channel object and match the ids of the show's we added to our My Shows array to all fetched shows. Then display cards */}
        {myShows.length > 0 ? (
          <>
            {mergeAndSort.map((show) => (
              <ShowCard
                key={show.showEventId}
                show={show}
                isAdded={myShows.includes(show.showEventId)}
              />
            ))}
          </>
        ) : (
          <div className="dummy-show">
            <h3>Nothing here yet!</h3>
            <p>
              Click on the "+" to add shows{" "}
              <span>
                <img src={rightArrow}></img>
              </span>
            </p>
            <span className="add-button small">+</span>
          </div>
        )}
      </div>
    </div>
  );
};

export default MyShowsComponent;
