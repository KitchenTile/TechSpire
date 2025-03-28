import { useCallback, useContext, useEffect, useMemo, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import ChannelsContext from "../contexts/channelsContext";
import LogoLoadingComponent from "../components/loadingComponents/LogoLoadingComponent";
import ShowCard from "../components/showScheduler/ShowCard";
import AddRemoveShowsContextProvider from "../contexts/AddRemoveShowsContextProvider";
import "./ExplorePage.css";
import Header from "../components/header/Header";
import MyShowsContext from "../contexts/myShowsContext";
import useMergeAndFilter from "../hooks/useMergeAndFilter";
import ChannelShowComponent from "../components/showScheduler/ChannelShowComponent";
import GenreFilterComponent from "../components/explore/GenreFilterComponent";

// this compoenent is made to be reused and display different components baed on URL path -- BLUE
const ExplorePage = () => {
  const section = useParams();
  const [filter, setFilter] = useState("All");
  const { channels } = useContext(ChannelsContext);
  const { myShows } = useContext(MyShowsContext);
  const mergedAndFilteredShows = useMergeAndFilter(section.section);
  const navigate = useNavigate();

  useEffect(() => {
    const timeout = setTimeout(() => {
      if (!channels) {
        console.log("going back to login");
        navigate("/");
      } else {
        console.log(channels);
      }
    }, 2500);

    return () => clearTimeout(timeout);
  }, [channels]);

  const handleFilter = useCallback((value) => {
    setFilter(value);
  }, []);

  const filteredByGenre = useMemo(
    () =>
      filter === "All"
        ? mergedAndFilteredShows
        : mergedAndFilteredShows.filter((show) => show.tagName === filter),
    [filter, mergedAndFilteredShows]
  );

  return (
    <div className="page-container" id="section-page">
      {channels ? (
        <AddRemoveShowsContextProvider>
          <Header />
          {section.section !== "Channels" ? (
            <>
              <div className="title-genres">
                <h1 className="title">{section.section} Shows</h1>
                <GenreFilterComponent handleFilter={handleFilter} />
              </div>
              <div
                className="content-container"
                style={
                  filteredByGenre.length === 1 ? { width: "fit-content" } : {}
                }
              >
                {filteredByGenre.length > 0 ? (
                  filteredByGenre.map((show) => (
                    <ShowCard
                      key={show.showEventId}
                      show={show}
                      rowRef={window}
                      isAdded={myShows.includes(show.showEventId)}
                      style={{ minWidth: "auto" }}
                    />
                  ))
                ) : (
                  <p>No {filter} shows found</p>
                )}
              </div>
            </>
          ) : (
            <>
              <h1 className="title">All Channels</h1>
              <div
                className="grid-container"
                style={{ flexDirection: "column" }}
              >
                {channels.channels.map((channel) => (
                  <ChannelShowComponent
                    key={channel.channelId}
                    channel={channel}
                  />
                ))}
              </div>
            </>
          )}
        </AddRemoveShowsContextProvider>
      ) : (
        <>
          <LogoLoadingComponent />
        </>
      )}
    </div>
  );
};

export default ExplorePage;
