import ShowCard from "./ShowCard";
import rightArrow from "../../assets/rightArrow.svg";
import { useContext, useMemo } from "react";
import "./MyShowsComponent.css";
import useMergeAndFilter from "../../hooks/useMergeAndFilter";
import ChannelsContext from "../../contexts/channelsContext";
import MyShowsContext from "../../contexts/myShowsContext";

const MyShowsComponent = () => {
  const channels = useContext(ChannelsContext);
  const { myShows } = useContext(MyShowsContext);
  const mergeShows = useMergeAndFilter("All");

  //this function replaces previous
  const mergeAndSort = useMemo(() => {
    const filtered = mergeShows?.filter((event) =>
      myShows.includes(event.showEventId)
    );

    return filtered;
  }, [channels, myShows]);

  return (
    <>
      <h1 className="title h1">My Shows</h1>
      <div className="myshow-container">
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
    </>
  );
};

export default MyShowsComponent;
