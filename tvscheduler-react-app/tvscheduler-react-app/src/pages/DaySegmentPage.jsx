import { useContext, useMemo, useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import ChannelsContext from "../contexts/channelsContext";
import LogoLoadingComponent from "../components/loadingComponents/LogoLoadingComponent";
import useShowLookup from "../hooks/useShowLookup";
import ShowCard from "../components/showScheduler/ShowCard";
import MyShowsContextProvider from "../contexts/MyShowsContextProvider";
import AddRemoveShowsContextProvider from "../contexts/AddRemoveShowsContextProvider";
import "./DaySegmentPage.css";
import Header from "../components/header/Header";
import MyShowsComponent from "../components/showScheduler/myShowsComponent";

const DaySegmentPage = () => {
  const section = useParams();

  const channels = useContext(ChannelsContext);

  console.log(channels);

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
        <MyShowsContextProvider channels={channels}>
          <AddRemoveShowsContextProvider>
            <Header />
            <MyShowsComponent />
            <h1 className="title">{section.section} Shows</h1>
            <div className="content-container">
              {mergeShowsPerSection[section.section].map((show) => (
                <ShowCard show={show} style={{ minWidth: "auto" }} />
              ))}
            </div>
          </AddRemoveShowsContextProvider>
        </MyShowsContextProvider>
      ) : (
        <LogoLoadingComponent />
      )}
    </div>
  );
};

export default DaySegmentPage;
