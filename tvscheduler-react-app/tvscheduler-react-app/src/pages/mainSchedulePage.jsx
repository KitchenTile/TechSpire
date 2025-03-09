import { useCallback, useContext, useEffect, useState, useMemo } from "react";
import "./mainSchedulePage.css";
import ChannelShowComponent from "../components/showScheduler/ChannelShowComponent";
import LogoLoadingComponent from "../components/loadingComponents/LogoLoadingComponent";
import MyShowsComponent from "../components/showScheduler/myShowsComponent";
import SectionCarouselComponent from "../components/showScheduler/Carousels/SectionCarouselComponent";
import ChannelsContext from "../contexts/channelsContext";
import Header from "../components/header/Header";
import MyShowsContext from "../contexts/myShowsContext";
import AddRemoveShowsContext from "../contexts/AddRemoveShowsContext";
import FetchedInfoProvider from "../contexts/FetchedInfoProvider";
import AddRemoveShowsContextProvider from "../contexts/AddRemoveShowsContextProvider";
import MyShowsContextProvider from "../contexts/MyShowsContextProvider";

const MainSchedulePage = () => {
  const channels = useContext(ChannelsContext);

  // const myShowsValue = useMemo(() => ({ myShows, addRemoveShow }), [myShows]);

  return (
    <div className="page-container">
      {channels ? (
        <MyShowsContextProvider channels={channels}>
          <AddRemoveShowsContextProvider>
            <>
              <Header />

              {/* day section carrousel */}
              <SectionCarouselComponent />
              {/* my shows display */}
              <MyShowsComponent />
              <h1 className="title h1">All Shows</h1>
              <div className="grid-container">
                {/* get the first x elements of the guide data array -- 129 is too long man */}
                {channels.channels.map((channel) => (
                  <ChannelShowComponent
                    key={channel.channelId}
                    channel={channel}
                  />
                ))}
              </div>
            </>
          </AddRemoveShowsContextProvider>
        </MyShowsContextProvider>
      ) : (
        // <></>
        <LogoLoadingComponent />
      )}
    </div>
  );
};

export default MainSchedulePage;
