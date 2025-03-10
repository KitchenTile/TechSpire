import { useContext, useEffect, useState } from "react";
import "./mainSchedulePage.css";
import ChannelShowComponent from "../components/showScheduler/ChannelShowComponent";
import LogoLoadingComponent from "../components/loadingComponents/LogoLoadingComponent";
import MyShowsComponent from "../components/showScheduler/myShowsComponent";
import SectionCarouselComponent from "../components/showScheduler/Carousels/SectionCarouselComponent";
import ChannelsContext from "../contexts/channelsContext";
import Header from "../components/header/Header";
import AddRemoveShowsContextProvider from "../contexts/AddRemoveShowsContextProvider";
import MyShowsContextProvider from "../contexts/MyShowsContextProvider";
import useThrottle from "../hooks/useThrottle";

const MainSchedulePage = () => {
  const channels = useContext(ChannelsContext);
  const [isVisible, setIsVisible] = useState(false);

  //we want to throttle the scrolling so we use our hook
  const throttleWindowScrroll = useThrottle(window, () => {
    const scrollPosition = window.scrollY;

    //if the scroll position of the window is > 500 then set isVisible to true
    setIsVisible((prev) =>
      prev !== scrollPosition > 500 ? scrollPosition > 500 : prev
    );
  });

  //header visibility logic
  useEffect(() => {
    window.addEventListener("scroll", throttleWindowScrroll);

    return () => window.removeEventListener("scroll", throttleWindowScrroll);
  });

  return (
    <div className="page-container">
      {channels ? (
        <MyShowsContextProvider channels={channels}>
          <AddRemoveShowsContextProvider>
            <>
              <Header isVisible={isVisible} />

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
        <LogoLoadingComponent />
      )}
    </div>
  );
};

export default MainSchedulePage;
