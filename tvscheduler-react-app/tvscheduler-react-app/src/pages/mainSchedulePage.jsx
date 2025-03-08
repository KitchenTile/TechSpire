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

const MainSchedulePage = () => {
  const [myShows, setMyShows] = useState([]);
  const channels = useContext(ChannelsContext);

  useEffect(() => {
    console.log(myShows);
  }, [myShows]);

  useEffect(() => {
    if (channels) {
      const scheduledShowIds = channels.schedule.map(
        (show) => show.showEvent.showEventId
      );
      setMyShows(scheduledShowIds);
    }
  }, [channels]);

  const addShowCall = async (showEventId) => {
    const token = localStorage.getItem("JWToken");
    try {
      const response = await fetch(
        "http://localhost:5171/Account/add-show-to-schedule",
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ showEventId }),
        }
      );
      if (!response.ok) {
        throw new Error("Failed to post show addition");
      }
    } catch (err) {
      console.error(err);
    }
  };

  const removeShowCall = async (showEventId) => {
    const token = localStorage.getItem("JWToken");
    try {
      const response = await fetch(
        "http://localhost:5171/remove-show-from-schedule",
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ showEventId }),
        }
      );
      if (!response.ok) {
        throw new Error("Failed to post show removal");
      }
    } catch (err) {
      console.error(err);
    }
  };

  //add shows to state pass -- pass function to component as prop (ShowCard)
  const addRemoveShow = useCallback(async (showEventId) => {
    setMyShows((prevMyShows) => {
      if (!prevMyShows.includes(showEventId)) {
        addShowCall(showEventId);
        return [...prevMyShows, showEventId];
      } else {
        removeShowCall(showEventId);
        return prevMyShows.filter((id) => id !== showEventId);
      }
    });
  }, []);

  const myShowsValue = useMemo(() => ({ myShows, addRemoveShow }), [myShows]);
  const addRemoveValue = useMemo(() => ({ addRemoveShow }), []);

  return (
    <div className="page-container">
      {channels ? (
        <ChannelsContext.Provider value={channels}>
          <MyShowsContext.Provider value={myShowsValue}>
            <AddRemoveShowsContext.Provider value={addRemoveValue}>
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
            </AddRemoveShowsContext.Provider>
          </MyShowsContext.Provider>
        </ChannelsContext.Provider>
      ) : (
        // <></>
        <LogoLoadingComponent />
      )}
    </div>
  );
};

export default MainSchedulePage;
