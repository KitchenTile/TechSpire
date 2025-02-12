import { useEffect, useRef, useState } from "react";
import "./mainSchedulePage.css";
import ShowCard from "../components/ShowCard";
import rightArrow from "../assets/rightArrow.svg";
import LoadingComponent from "../components/loadingComponent";

const MainSchedulePage = () => {
  const [channels, setChannels] = useState(null);
  const [myShows, setMyShows] = useState([]);

  const showContainerRef = useRef(null);
  const [showLeftArrow, setShowLeftArrow] = useState(true);
  const [showRightArrow, setShowRightArrow] = useState(true);

  //fetch Data on page load
  useEffect(() => {
    const loadChannels = async () => {
      try {
        const response = await fetch("http://localhost:8080");
        if (!response.ok) {
          throw new Error("Failed to fetch channels :(");
        }
        const data = await response.json();
        setChannels(data);
        console.log("fetched data: ", data);
      } catch (error) {
        console.log("error: ", error);
      }
    };
    loadChannels();
  }, []);

  //console log the array every time it's modified -- debugging
  useEffect(() => {
    console.log(myShows);
  }, [myShows]);

  useEffect(() => {
    const handleScroll = () => {
      const container = showContainerRef.current;

      if (container) {
        const { scrollLeft, clientWidth, scrollWidth } = container;

        scrollLeft > 5 ? setShowLeftArrow(true) : setShowLeftArrow(false);

        console.log(scrollLeft);

        setShowRightArrow(scrollLeft + clientWidth > scrollWidth - 5);
      }
    };
    const container = showContainerRef.current;

    container && console.log(container);
    container && container.addEventListener("scroll", handleScroll);

    handleScroll();

    return () =>
      container && container.removeEventListener("scroll", handleScroll);
  }, []);

  //add shows to state pass -- pass function to component as prop (ShowCard)
  const addRemoveShow = (showId) => {
    if (!myShows.includes(showId)) {
      setMyShows((myShows) => [...myShows, showId]);
    } else {
      setMyShows(myShows.filter((id) => id !== showId));
    }
  };

  const compareStartTime = (a, b) => {
    return a.startTime - b.startTime;
  };

  return (
    <div className="page-container">
      {channels ? (
        <>
          {/* my shows display */}
          <h1 className="title h1">My Shows</h1>
          <div className="myshow-container">
            {/* if my shows is not empty, flatten the show per channel object and match the ids of the show's we added to our My Shows array to all fetched shows. Then display cards */}
            {myShows.length > 0 ? (
              Object.entries(channels.programData)
                .map(([channelId, channelData]) =>
                  channelData[0].event.filter((show) =>
                    myShows.includes(show.evtId)
                  )
                )
                .flat()
                .sort(compareStartTime)
                .map((show) => (
                  <ShowCard
                    key={show.evtId}
                    show={show}
                    addRemoveShow={addRemoveShow}
                    isAdded={myShows.includes(show.evtId)}
                  />
                ))
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
          <h1 className="title h1">All Shows</h1>
          <div className="grid-container">
            {/* get the first x elements of the guide data array -- 129 is too long man */}
            {channels.guideData.slice(0, 5).map((channel) => (
              <div key={channel.channelid} className="channel-show-container">
                <div className="title-image-container">
                  <h3>{channel.channelname}</h3>
                  <span className="image-container">
                    <img src={channel.logourl} alt={channel.channelname} />
                  </span>
                </div>
                <div className="show-container" ref={showContainerRef}>
                  {showLeftArrow && (
                    <span className="left-arrow">
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
                  )}

                  <div className="shows-row">
                    {/* if the channel has shows, get the first x shows for each channel -- consider writing a variable for more readable code */}
                    {channels.programData[channel.channelid].length > 0 ? (
                      channels.programData[channel.channelid][0].event
                        .slice(0, 5)
                        .map((show, id) => (
                          <ShowCard
                            key={id}
                            show={show}
                            addRemoveShow={addRemoveShow}
                            isAdded={myShows.includes(show.evtId)}
                          />
                        ))
                    ) : (
                      <p>No shows available for ${channel.name}</p>
                    )}
                  </div>
                  <span className="right-arrow">
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
              </div>
            ))}
          </div>
        </>
      ) : (
        <LoadingComponent />
      )}
    </div>
  );
};

export default MainSchedulePage;
