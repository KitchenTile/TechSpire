import { useEffect, useMemo, useState } from "react";
import "./mainSchedulePage.css";
import ChannelShowComponent from "../components/showScheduler/ChannelShowComponent";
import LogoLoadingComponent from "../components/LogoLoadingComponent";
import MyShowsComponent from "../components/showScheduler/myShowsComponent";

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

  //console log the array every time it's modified -- debugging
  useEffect(() => {
    console.log(myShows);
  }, [myShows]);

  //add shows to state pass -- pass function to component as prop (ShowCard)
  const addRemoveShow = (showId) => {
    if (!myShows.includes(showId)) {
      setMyShows((myShows) => [...myShows, showId]);
    } else {
      setMyShows(myShows.filter((id) => id !== showId));
    }
  };

  // channels
  //   ? console.log(
  //       channels.channels.$values[0].showEvents.$values.forEach((value) => {
  //         console.log(value.$id);
  //       })
  //     )
  //   : null;

  return (
    <div className="page-container">
      {channels ? (
        <>
          {/* my shows display */}
          {/* <MyShowsComponent
            channels={channels}
            myShows={myShows}
            addRemoveShow={addRemoveShow}
          /> */}
          <h1 className="title h1">All Shows</h1>
          <div className="grid-container">
            {/* get the first x elements of the guide data array -- 129 is too long man */}
            {channels.channels.$values.map((channel) => (
              <ChannelShowComponent
                key={channel.channelid}
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
