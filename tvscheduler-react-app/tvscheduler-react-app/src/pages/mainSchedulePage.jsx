import { useEffect, useState } from "react";
import "./MainSchedulePage.css";
import ShowCard from "../components/ShowCard";
import rightArrow from "../assets/rightArrow.svg";

const MainSchedulePage = () => {
  const [channels, setChannels] = useState(null);
  const [myShows, setMyShows] = useState([]);

  //fetch Data on page load
  useEffect(() => {
    const loadChannels = async () => {
      try {
        const response = await fetch("http://localhost:5171");
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
  const addShow = (showId) => {
    if (!myShows.includes(showId)) {
      setMyShows((myShows) => [...myShows, showId]);
    } else {
      setMyShows(myShows.filter((id) => id !== showId));
    }
  };

  return (
    <>
      {channels ? (
        <>
          {/* my shows display */}
          <h1 className="title h1">My Shows</h1>
          <div className="myshow-container">
            {/* if my shows is not empty, flatten the show per channel object and match the ids of the show's we added to our My Shows array to all fetched shows. Then display cards */}
            {myShows.length > 0 ? (
              Object.entries(channels.programData)
                .map(([channelId, channelData]) =>
                  channelData[0].event.filter((show) =>
                    myShows.includes(show.evtId)
                  )
                )
                .flat()
                .map((show) => (
                  <ShowCard
                    key={show.evtId}
                    show={show}
                    addShow={addShow}
                    isAdded={myShows.includes(show.evtId)}
                  />
                ))
            ) : (
              <div className="dummy-show">
                <h3>Nothing here yet!</h3>
                <p>
                  Click on the "+" to add shows{" "}
                  <span>
                    <img src={rightArrow}></img>
                  </span>
                </p>
                <span className="add-button small">+</span>
              </div>
            )}
          </div>
          <h1 className="title h1">All Shows</h1>
          <div className="grid-container">
            {/* get the first x elements of the guide data array -- 129 is too long man */}
            {channels.guideData.slice(0, 5).map((channel) => (
              <div key={channel.channelid} className="channel-show-container">
                <div className="title-image-container">
                  <h3>{channel.channelname}</h3>
                  <span className="image-container">
                    <img src={channel.logourl} alt={channel.channelname} />
                  </span>
                </div>
                <div className="shows-row">
                  {/* if the channel has shows, get the first x shows for each channel -- consider writing a variable for more readable code */}
                  {channels.programData[channel.channelid].length > 0 ? (
                    channels.programData[channel.channelid][0].event
                      .slice(0, 5)
                      .map((show, id) => (
                        <ShowCard
                          key={id}
                          show={show}
                          addShow={addShow}
                          isAdded={myShows.includes(show.evtId)}
                        />
                      ))
                  ) : (
                    <p>No shows available for ${channel.name}</p>
                  )}
                </div>
              </div>
            ))}
          </div>
        </>
      ) : (
        <p>loading...</p>
      )}
    </>
  );
};

export default MainSchedulePage;
