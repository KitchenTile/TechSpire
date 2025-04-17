import { useState, useEffect, useContext, memo } from "react";
import "./HighlightCarousel.css";
import useIntersectionObserver from "../../../hooks/useIntersectionObserver";
import CarouselCard from "./CarouselCard";
import MyShowsContext from "../../../contexts/myShowsContext";

// These are containers for differnet shows at differnet times of day. they hold n amount of show cards to cycle through -- BLUE
const HighlightCarousel = ({ carouselRef, section }) => {
  const [activeShow, setActiveShow] = useState(0);
  const { myShows } = useContext(MyShowsContext);

  // uEffect that changes the displayed card every 4 seconds
  useEffect(() => {
    setTimeout(() => {
      setActiveShow(activeShow === section.length - 1 ? 0 : activeShow + 1);
    }, 4000);
  });

  // options for the Intersection Observer
  const options = {
    root: carouselRef ? carouselRef.current : null,
    threshold: 0,
    rootMargin: "-100px",
  };

  const [cardRef, isVisible] = useIntersectionObserver(options);

  return (
    <div className="highlight-carousel-container" ref={cardRef}>
      {isVisible ? (
        <>
          {section.map((show, id) => (
            <CarouselCard
              key={show.timeStart + show.showEventId}
              show={show}
              activeShow={activeShow}
              isAdded={myShows.includes(show.showEventId)}
              id={id}
            />
          ))}
        </>
      ) : null}
    </div>
  );
};

export default HighlightCarousel;
