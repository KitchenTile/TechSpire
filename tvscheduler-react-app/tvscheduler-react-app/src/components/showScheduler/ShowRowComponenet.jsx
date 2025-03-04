import {
  useCallback,
  useEffect,
  useRef,
  useState,
  memo,
  useMemo,
  useContext,
} from "react";
import ShowCard from "./ShowCard";
import "./ShowRowComponent.css";
import useShowLookup from "../../hooks/useShowLookup";
import ChannelsContext from "../../contexts/channelsContext";

const ShowRowComponent = ({ channel, myShows, addRemoveShow }) => {
  const showContainerRef = useRef(null);
  const [showLeftArrow, setShowLeftArrow] = useState(true);
  const [showRightArrow, setShowRightArrow] = useState(true);
  const [scrollPosition, setScrollPosition] = useState(0);

  const channels = useContext(ChannelsContext);

  //effect hook to determine the component's scroll position so we can show and hide side arrows
  useEffect(() => {
    const handleScroll = () => {
      //the container is the row we are referencing to
      const container = showContainerRef.current;

      if (container) {
        // get information that we need like the scroll amount from the left, the width of the container etc.
        const { scrollLeft, clientWidth, scrollWidth } = container;
        setScrollPosition(scrollLeft);
        setShowLeftArrow(scrollLeft > 5);
        setShowRightArrow(scrollLeft + clientWidth < scrollWidth - 5);
      }
    };

    const container = showContainerRef.current;

    //add and then remove scroll listener on cleanup function
    container && container.addEventListener("scroll", handleScroll);

    handleScroll();

    return () =>
      container && container.removeEventListener("scroll", handleScroll);
  }, []);

  // function to skip forwards and backwards between the show cards
  const handleClick = useCallback(
    (skipAmount) => {
      setScrollPosition(scrollPosition + skipAmount);

      showContainerRef.current.scrollLeft = scrollPosition + skipAmount;
      console.log(scrollPosition);
    },
    [scrollPosition]
  );

  const showsLookup = useShowLookup(channels);

  // Merge each event with its corresponding show details
  const events = channel.showEvents || [];
  const mergedShows = useMemo(() => {
    return events.map((event) => {
      const showDetails = showsLookup[event.showId] || {};
      //Add show instance's details
      return { ...event, ...showDetails };
    });
  }, [events]);

  // useThrottle(showContainerRef, handleScroll);

  return (
    <>
      <div className="show-container">
        <span
          className={`left-arrow ${showLeftArrow ? "visible" : ""}`}
          onClick={() => {
            handleClick(-750);
          }}
        >
          <svg
            width="15"
            height="30"
            viewBox="0 0 9 15"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
            className="arrow"
          >
            <path
              d="M8.57129 2.14285L3.21415 7.5L8.57129 12.8571L7.49986 15L-0.000140326 7.5L7.49986 -3.57628e-06L8.57129 2.14285Z"
              fill="white"
            />
          </svg>
        </span>

        <div className="shows-row" ref={showContainerRef}>
          {/* if the channel has shows, get the first x shows for each channel -- consider writing a variable for more readable code */}
          {mergedShows.length > 0 ? (
            mergedShows
              .slice(0, 10)
              .map((show) => (
                <ShowCard
                  key={show.showEventId}
                  show={show}
                  addRemoveShow={addRemoveShow}
                  isAdded={myShows.includes(show.showEventId)}
                  rowRef={showContainerRef}
                />
              ))
          ) : (
            <p>No shows available for ${channel.name}</p>
          )}
        </div>
        <span
          className={`right-arrow ${showRightArrow ? "visible" : ""}`}
          onClick={() => {
            handleClick(750);
          }}
        >
          <svg
            width="15"
            height="30"
            viewBox="0 0 22 37"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
            className="arrow"
          >
            <path
              d="M0 31.7143L13.75 18.5L0 5.28571L2.75 0L22 18.5L2.75 37L0 31.7143Z"
              fill="white"
            />
          </svg>
        </span>
      </div>
    </>
  );
};

export default memo(ShowRowComponent);
