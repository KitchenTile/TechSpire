import ShowCard from "./ShowCard";
import rightArrow from "../../assets/rightArrow.svg";
import { useMemo } from "react";
import "./MyShowsComponent.css";

const MyShowsComponent = ({ channels, myShows, addRemoveShow }) => {
  // sorting function for the sort my schedule sort method
  const compareStartTime = (a, b) => {
    return a.startTime - b.startTime;
  };

  const flatAndMap = useMemo(() => {
    console.log("floatandmap");
    if (!channels) {
      return [];
    }
    return Object.entries(channels.programData)
      .map(([channelId, channelData]) =>
        channelData[0].event.filter((show) => myShows.includes(show.evtId))
      )
      .flat()
      .sort(compareStartTime);
  }, [channels, myShows]);

  return (
    <>
      <h1 className="title h1">My Shows</h1>
      <div className="myshow-container">
        {/* if my shows is not empty, flatten the show per channel object and match the ids of the show's we added to our My Shows array to all fetched shows. Then display cards */}
        {myShows.length > 0 ? (
          <>
            {flatAndMap.map((show) => (
              <ShowCard
                key={show.evtId}
                show={show}
                addRemoveShow={addRemoveShow}
                isAdded={myShows.includes(show.evtId)}
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
    </>
  );
};

export default MyShowsComponent;
