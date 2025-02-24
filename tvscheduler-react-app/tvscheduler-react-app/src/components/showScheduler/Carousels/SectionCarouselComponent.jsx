import { useEffect, useState, useRef, useMemo } from "react";
import "./SectionCarouselComponent.css";
import HighlightCarousel from "./HighlightCarousel";
import useShowLookup from "../../../hooks/useShowLookup";

const SectionCarouselComponent = ({ channels }) => {
  const [activeSection, setActiveSection] = useState(0);
  const carouselSectionRef = useRef(null);

  //effect hook to determine the component's scroll position so we can show and hide side arrows
  useEffect(() => {
    const handleScroll = () => {
      //the container is the row we are referencing to
      const container = carouselSectionRef.current;

      if (container) {
        // get information that we need like the scroll amount from the left, the width of the container etc.
        const { scrollLeft, clientWidth } = container;
        if (scrollLeft < clientWidth) {
          setActiveSection(0);
        } else if (clientWidth <= scrollLeft && scrollLeft < clientWidth * 2) {
          setActiveSection(1);
        } else {
          setActiveSection(2);
        }
        console.log(clientWidth);
      }
    };

    const container = carouselSectionRef.current;

    //add and then remove scroll listener on cleanup function
    container && container.addEventListener("scroll", handleScroll);

    handleScroll();

    return () =>
      container && container.removeEventListener("scroll", handleScroll);
  }, []);

  // function to skip forwards and backwards between the show cards
  const handleClick = (skipAmount) => {
    carouselSectionRef.current.scrollLeft =
      carouselSectionRef.current.scrollLeft + skipAmount;
  };

  //Fisher-Yates shuffle to get random elements
  const getRandomElements = (arr, n) => {
    const copy = [...arr];
    for (let i = copy.length - 1; i > 0; i--) {
      const random = Math.floor(Math.random() * (i + 1));
      [copy[i], copy[random]] = [copy[random], copy[i]];
    }
    return copy.slice(0, n);
  };

  const showLookup = useShowLookup(channels);

  const mergeShowsPerSection = useMemo(() => {
    // get all the shows flattened with the lookup table
    const mergeShows = channels.channels
      .map((channel) => {
        if (!channel.showEvents || !channel.showEvents) return [];
        return channel.showEvents.map((event) => {
          return { ...event, ...showLookup[event.showId] };
        });
      })
      .flat();

    // separate the shows into different time sections
    const morningShows = mergeShows
      .filter((event) => {
        const time = new Date(event.timeStart * 1000);
        return 1 < time.getHours() && time.getHours() < 10;
      })
      .map((show) => ({ ...show, section: "morning" }));

    const afternoonShows = mergeShows
      .filter((event) => {
        const time = new Date(event.timeStart * 1000);
        return 10 < time.getHours() && time.getHours() < 17;
      })
      .map((show) => ({ ...show, section: "afternoon" }));

    const eveningShows = mergeShows
      .filter((event) => {
        const time = new Date(event.timeStart * 1000);
        return 17 < time.getHours() && time.getHours() < 23;
      })
      .map((show) => ({ ...show, section: "evening" }));

    // return this object with all the information ready to display
    const sectionsObject = {
      morning: getRandomElements(morningShows, 5),
      afternoon: getRandomElements(afternoonShows, 5),
      evening: getRandomElements(eveningShows, 5),
    };
    return sectionsObject;
  }, [channels, showLookup]);

  // console.log(Object.entries(mergeShowsPerSection));

  // {
  //   Object.entries(mergeShowsPerSection).map(
  //     (section, idx) =>
  //       console.log(section[1][1].showEventId + section[1][1].timeStart)
  //     // console.log(section[1][1])
  //   );
  // }

  return (
    <div className="carousel-section">
      <span
        className="arrows left"
        onClick={() => {
          handleClick(-carouselSectionRef.current.clientWidth);
        }}
      >
        <svg
          width="30"
          height="50"
          viewBox="0 0 9 15"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
          className="arrow"
        >
          <path
            d="M8.57129 2.14285L3.21415 7.5L8.57129 12.8571L7.49986 15L-0.000140326 7.5L7.49986 -3.57628e-06L8.57129 2.14285Z"
            fill="white"
          />
        </svg>
      </span>
      <h1 className="sectionSection">
        Highlights for your {Object.keys(mergeShowsPerSection)[activeSection]}
      </h1>
      <div className="carousel-container" ref={carouselSectionRef}>
        {Object.entries(mergeShowsPerSection).map((section, idx) => (
          <>
            <HighlightCarousel key={idx} section={section[1]} />
          </>
        ))}
      </div>
      <span
        className="arrows right"
        onClick={() => {
          handleClick(carouselSectionRef.current.clientWidth);
        }}
      >
        <svg
          width="30"
          height="50"
          viewBox="0 0 22 37"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
          className="arrow"
        >
          <path
            d="M0 31.7143L13.75 18.5L0 5.28571L2.75 0L22 18.5L2.75 37L0 31.7143Z"
            fill="white"
          />
        </svg>
      </span>
    </div>
  );
};

export default SectionCarouselComponent;
