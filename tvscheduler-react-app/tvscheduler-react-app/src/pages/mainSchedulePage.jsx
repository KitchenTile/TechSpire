import { useContext, useEffect, useState } from "react";
import "./mainSchedulePage.css";
import ChannelShowComponent from "../components/showScheduler/ChannelShowComponent";
import LogoLoadingComponent from "../components/loadingComponents/LogoLoadingComponent";
import MyShowsComponent from "../components/showScheduler/myShowsComponent";
import SectionCarouselComponent from "../components/showScheduler/Carousels/SectionCarouselComponent";
import ChannelsContext from "../contexts/channelsContext";
import Header from "../components/header/Header";
import AddRemoveShowsContextProvider from "../contexts/AddRemoveShowsContextProvider";
import useThrottle from "../hooks/useThrottle";
import Modal from "../components/misc/Modal";
import { useNavigate } from "react-router-dom";
import useMergeAndFilter from "../hooks/useMergeAndFilter";
import ShowCard from "../components/showScheduler/ShowCard";

const MainSchedulePage = () => {
  const channels = useContext(ChannelsContext);
  const [isVisible, setIsVisible] = useState(false);
  const [openModal, setOpenModal] = useState(true);
  const navigate = useNavigate();
  const mergedAndFilteredShows = useMergeAndFilter("All");

  //effect to go back to the login page if the JWT expires
  useEffect(() => {
    const timeOut = setTimeout(() => {
      if (!channels) {
        console.log("Going back to login");
        navigate("/");
      } else {
        console.log(channels);
      }
    }, 2500);

    return () => clearTimeout(timeOut);
  }, [channels]);

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

  const handleModalClose = () => {
    setOpenModal(false);
  };

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
    <div className="page-container">
      {channels ? (
        <AddRemoveShowsContextProvider>
          <>
            {channels.favTags.length !== 0 ? null : (
              <Modal open={openModal} handleModalClose={handleModalClose} />
            )}
            <Header isVisible={isVisible} />

            {/* day section carrousel */}
            <SectionCarouselComponent />
            {/* my shows display */}
            <MyShowsComponent />
            <h1 className="title h1">Our Picks For You</h1>
            <div className="grid-container">
              {/* get the first x elements of the guide data array -- 129 is too long man */}
              {/* {channels.channels.map((channel) => (
                <ChannelShowComponent
                  key={channel.channelId}
                  channel={channel}
                />
              ))} */}
              {recommnededShows.map((show) => (
                <ShowCard show={show} />
              ))}
            </div>
          </>
        </AddRemoveShowsContextProvider>
      ) : (
        <LogoLoadingComponent />
      )}
    </div>
  );
};

export default MainSchedulePage;
