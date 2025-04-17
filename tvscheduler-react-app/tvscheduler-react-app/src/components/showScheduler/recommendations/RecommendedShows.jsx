import { useContext, useMemo } from "react";
import ChannelsContext from "../../../contexts/channelsContext";
import useMergeAndFilter from "../../../hooks/useMergeAndFilter";
import SearchCard from "../../header/SearchCard";
import ShowCard from "../ShowCard";
import MyShowsContext from "../../../contexts/myShowsContext";
import ChannelShowComponent from "../ChannelShowComponent";

// file to manage recommendations and display recommended shows
const RecommendedShows = () => {
  const { channels } = useContext(ChannelsContext);
  const { myShows } = useContext(MyShowsContext);

  const mergedAndFilteredShows = useMergeAndFilter("All");

  const recommendedShowOfTheDaySHOW = channels?.shows.filter(
    (show) => show.showId === channels.individualRecommendation
  );

  const recommendedShowOfTheDay = mergedAndFilteredShows.filter(
    (showEvent) => showEvent.showId === channels.individualRecommendation
  );

  const recommendations = () => {
    let userTags = [];

    userTags = channels?.favTags.map((tag) => {
      return tag.tagName;
    });

    return mergedAndFilteredShows.filter((show) =>
      userTags.includes(show.tagName)
    );
  };

  const recommnededShows = recommendations();

  return (
    <div>
      {channels.favTags.length !== 0 ? (
        <div className="show-recommnedation">
          <h1 className="title h1">Show of the day</h1>
          <SearchCard
            show={recommendedShowOfTheDaySHOW[0]}
            showEvents={recommendedShowOfTheDay}
          />
        </div>
      ) : null}
      <h1 className="title h1">
        {channels.favTags.length === 0 ? "All Channels" : "Our Picks For You"}
      </h1>
      <div
        className="grid-container"
        id={channels.favTags.length === 0 ? "withoutFavs" : "mainPage"}
        style={channels.favTags.length === 0 ? { flexDirection: "column" } : {}}
      >
        {channels.favTags.length === 0
          ? channels.channels.map((channel) => (
              <ChannelShowComponent key={channel.channelId} channel={channel} />
            ))
          : recommnededShows.map((show) => (
              <ShowCard
                key={show.showEventId}
                show={show}
                isAdded={myShows.includes(show.showEventId)}
              />
            ))}
      </div>
    </div>
  );
};

export default RecommendedShows;
