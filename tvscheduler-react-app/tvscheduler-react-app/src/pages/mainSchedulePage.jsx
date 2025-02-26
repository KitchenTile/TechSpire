import { useCallback, useEffect, useMemo, useState } from "react";
import "./mainSchedulePage.css";
import ChannelShowComponent from "../components/showScheduler/ChannelShowComponent";
import LogoLoadingComponent from "../components/LogoLoadingComponent";
import MyShowsComponent from "../components/showScheduler/myShowsComponent";
import SectionCarouselComponent from "../components/showScheduler/Carousels/SectionCarouselComponent";

const MainSchedulePage = () => {
  const [channels, setChannels] = useState(null);
  const [myShows, setMyShows] = useState([]);

  //fetch Data on page load
  useEffect(() => {
    const loadChannels = async () => {
      const token = localStorage.getItem("JWToken");
      console.log(token);
      try {
        const response = await fetch("http://localhost:5171/main", {
          method: "GET",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        });
        if (!response.ok) {
          throw new Error("Failed to fetch channels :(");
        }
        const data = await response.json();
        setChannels(data);

        console.log("fetched data: ", data);
      } catch (error) {
        console.log("error: ", error);
      }
    };
    loadChannels();
  }, []);

  useEffect(() => {
    if (channels) {
      const scheduledShowIds = channels.schedule.map(
        (show) => show.showEvent.showEventId
      );
      setMyShows(scheduledShowIds);
    }
  }, [channels]);

  // channels
  //   ? channels.schedule.map((show) => {
  //       setMyShows((myShows) => [...myShows, show.showEvent.showEventId]);
  //     })
  //   : null;

  //console log the array every time it's modified -- debugging
  useEffect(() => {
    console.log(myShows);
  }, [myShows]);

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
  const addRemoveShow = useCallback(
    (showEventId) => {
      if (!myShows.includes(showEventId)) {
        setMyShows((myShows) => [...myShows, showEventId]);
        addShowCall(showEventId);
      } else {
        setMyShows(myShows.filter((id) => id !== showEventId));
        removeShowCall(showEventId);
      }
    },
    [myShows]
  );

  return (
    <div className="page-container">
      {channels ? (
        <>
          {/* day section carrousel */}
          <SectionCarouselComponent
            channels={channels}
            addRemoveShow={addRemoveShow}
          />
          {/* my shows display */}
          <MyShowsComponent
            channels={channels}
            myShows={myShows}
            addRemoveShow={addRemoveShow}
          />
          <h1 className="title h1">All Shows</h1>
          <div className="grid-container">
            {/* get the first x elements of the guide data array -- 129 is too long man */}
            {channels.channels.map((channel) => (
              <ChannelShowComponent
                key={channel.channelId}
                channels={channels}
                channel={channel}
                addRemoveShow={addRemoveShow}
                myShows={myShows}
              />
            ))}
          </div>
        </>
      ) : (
        // <></>
        <LogoLoadingComponent />
      )}
    </div>
  );
};

export default MainSchedulePage;
