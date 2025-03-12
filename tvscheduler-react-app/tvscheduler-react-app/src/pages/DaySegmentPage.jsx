import { useContext, useMemo, useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import ChannelsContext from "../contexts/channelsContext";
import LogoLoadingComponent from "../components/loadingComponents/LogoLoadingComponent";
import useShowLookup from "../hooks/useShowLookup";
import ShowCard from "../components/showScheduler/ShowCard";
import MyShowsContextProvider from "../contexts/MyShowsContextProvider";
import AddRemoveShowsContextProvider from "../contexts/AddRemoveShowsContextProvider";
import "./DaySegmentPage.css";
import Header from "../components/header/Header";
import MyShowsComponent from "../components/showScheduler/myShowsComponent";
import MyShowsContext from "../contexts/myShowsContext";

const DaySegmentPage = () => {
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

  const showLookup = useShowLookup(channels);

  const mergeShowsPerSection = useMemo(() => {
    // get all the shows flattened with the lookup table
    const mergeShows = channels
      ? channels?.channels
          .map((channel) => {
            if (!channel.showEvents || !channel.showEvents) return [];
            return channel.showEvents.map((event) => {
              return { ...event, ...showLookup[event.showId] };
            });
          })
          .flat()
      : null;

    // separate the shows into different time sections
    const morningShows = mergeShows
      ?.filter((event) => {
        const time = new Date(event.timeStart * 1000);
        return 1 < time.getHours() && time.getHours() < 10;
      })
      .map((show) => ({ ...show, section: "morning" }));

    const afternoonShows = mergeShows
      ?.filter((event) => {
        const time = new Date(event.timeStart * 1000);
        return 10 < time.getHours() && time.getHours() < 17;
      })
      .map((show) => ({ ...show, section: "afternoon" }));

    const eveningShows = mergeShows
      ?.filter((event) => {
        const time = new Date(event.timeStart * 1000);
        return 17 < time.getHours() && time.getHours() < 23;
      })
      .map((show) => ({ ...show, section: "evening" }));

    // return this object with all the information ready to display
    const sectionsObject = {
      Morning: morningShows,
      Afternoon: afternoonShows,
      Evening: eveningShows,
    };
    return sectionsObject;
  }, [channels]);

  return (
    <div className="page-container" id="section-page">
      {channels ? (
        <AddRemoveShowsContextProvider>
          <Header />
          <MyShowsComponent />
          <h1 className="title">{section.section} Shows</h1>
          <div className="content-container">
            {mergeShowsPerSection[section.section].map((show) => (
              <ShowCard
                show={show}
                rowRef={window}
                isAdded={myShows.includes(show.showEventId)}
                style={{ minWidth: "auto" }}
              />
            ))}
          </div>
        </AddRemoveShowsContextProvider>
      ) : (
        <>
          <LogoLoadingComponent />
        </>
      )}
    </div>
  );
};

export default DaySegmentPage;
