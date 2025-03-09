import ShowCard from "./ShowCard";
import rightArrow from "../../assets/rightArrow.svg";
import { useContext, useMemo } from "react";
import "./MyShowsComponent.css";
import useShowLookup from "../../hooks/useShowLookup";
import ChannelsContext from "../../contexts/channelsContext";
import MyShowsContext from "../../contexts/myShowsContext";
// import MyShowsContext from "../../contexts/myShowsContext";

const MyShowsComponent = () => {
  const channels = useContext(ChannelsContext);
  const { myShows } = useContext(MyShowsContext);
  // sorting function for the sort my schedule sort method
  const compareStartTime = (a, b) => {
    return a.timeStart - b.timeStart;
  };

  const showLookup = useShowLookup(channels);

  //this function replaces previous
  const mergeAndSort = useMemo(() => {
    const mergeShows = channels.channels
      .map((channel) => {
        // Check if the channel has events
        if (!channel.showEvents || !channel.showEvents) return [];
        // Map each event to merge its details from the lookup
        return channel.showEvents.map((event) => {
          // Merge event with show details
          return { ...event, ...showLookup[event.showId] };
        });
      })
      .flat();

    const filtered = mergeShows.filter((event) =>
      myShows.includes(event.showEventId)
    );

    return filtered.sort(compareStartTime);
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
