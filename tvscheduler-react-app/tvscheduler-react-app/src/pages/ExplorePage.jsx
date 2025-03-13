import { useContext, useMemo, useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import ChannelsContext from "../contexts/channelsContext";
import LogoLoadingComponent from "../components/loadingComponents/LogoLoadingComponent";
import ShowCard from "../components/showScheduler/ShowCard";
import MyShowsContextProvider from "../contexts/MyShowsContextProvider";
import AddRemoveShowsContextProvider from "../contexts/AddRemoveShowsContextProvider";
import "./ExplorePage.css";
import Header from "../components/header/Header";
import MyShowsComponent from "../components/showScheduler/myShowsComponent";
import MyShowsContext from "../contexts/myShowsContext";
import useMergeAndFilter from "../hooks/useMergeAndFilter";
import ChannelShowComponent from "../components/showScheduler/ChannelShowComponent";

const ExplorePage = () => {
  const section = useParams();

  const channels = useContext(ChannelsContext);
  const { myShows } = useContext(MyShowsContext);
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

  const mergedAndFilteredShows = useMergeAndFilter(section.section);

  console.log(mergedAndFilteredShows);

  return (
    <div className="page-container" id="section-page">
      {channels ? (
        <AddRemoveShowsContextProvider>
          <Header />
          <MyShowsComponent />
          {section.section !== "Channels" ? (
            <>
              <h1 className="title">{section.section} Shows</h1>
              <div className="content-container">
                {mergedAndFilteredShows?.map((show) => (
                  <ShowCard
                    show={show}
                    rowRef={window}
                    isAdded={myShows.includes(show.showEventId)}
                    style={{ minWidth: "auto" }}
                  />
                ))}
              </div>
            </>
          ) : (
            <>
              <h1 className="title">All Channels</h1>
              <div className="grid-container">
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
