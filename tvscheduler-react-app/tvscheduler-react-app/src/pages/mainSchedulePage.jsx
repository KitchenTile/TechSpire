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
import MyShowsContext from "../contexts/myShowsContext";
import GenreSelectionCompoenet from "../components/misc/GenreSelectionCompoenet";
import SearchCard from "../components/header/SearchCard";

const MainSchedulePage = () => {
  const { channels } = useContext(ChannelsContext);
  const [isVisible, setIsVisible] = useState(false);
  const [openModal, setOpenModal] = useState(true);
  const navigate = useNavigate();
  const mergedAndFilteredShows = useMergeAndFilter("All");
  const { myShows } = useContext(MyShowsContext);

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

    if (window.innerWidth <= 430) {
      setIsVisible((prev) =>
        prev !== scrollPosition > 100 ? scrollPosition > 100 : prev
      );
    } else {
      //if the scroll position of the window is > 500 then set isVisible to true
      setIsVisible((prev) =>
        prev !== scrollPosition > 500 ? scrollPosition > 500 : prev
      );
    }
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

  const recommendedShowOfTheDaySHOW = channels?.shows.filter(
    (show) => show.showId === channels.individualRecommendation
  );

  const recommendedShowOfTheDay = mergedAndFilteredShows.filter(
    (showEvent) => showEvent.showId === channels.individualRecommendation
  );

  const recommnededShows = recommendations();

  return (
    <div className="page-container">
      {channels ? (
        <AddRemoveShowsContextProvider>
          <>
            {channels.favTags.length !== 0 ? null : (
              <Modal open={openModal} handleModalClose={handleModalClose}>
                <>
                  <h1 className="title">Welcome to ViewQue!</h1>
                  <p>
                    Select your favourite tv genres so we can tailor
                    recommendations for you!
                  </p>
                  <GenreSelectionCompoenet
                    handleModalClose={handleModalClose}
                    inModal={true}
                  />
                </>
              </Modal>
            )}
            <Header isVisible={isVisible} />

            {/* day section carrousel */}
            <SectionCarouselComponent />
            {/* my shows display */}
            <MyShowsComponent />
            {/* show recommendations section */}
            {/* {channels.favTags.length !== 0 ? (
              <div className="show-recommnedation">
                <h1 className="title h1">Show of the day</h1>
                <SearchCard
                  show={recommendedShowOfTheDaySHOW[0]}
                  showEvents={recommendedShowOfTheDay}
                />
              </div>
            ) : null} */}
            <h1 className="title h1">
              {channels.favTags.length === 0
                ? "All Channels"
                : "Our Picks For You"}
            </h1>
            <div
              className="grid-container"
              id={channels.favTags.length === 0 ? "withoutFavs" : "mainPage"}
              style={
                channels.favTags.length === 0 ? { flexDirection: "column" } : {}
              }
            >
              {channels.favTags.length === 0
                ? channels.channels.map((channel) => (
                    <ChannelShowComponent
                      key={channel.channelId}
                      channel={channel}
                    />
                  ))
                : recommnededShows.map((show) => (
                    <ShowCard
                      key={show.showEventId}
                      show={show}
                      isAdded={myShows.includes(show.showEventId)}
                    />
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
