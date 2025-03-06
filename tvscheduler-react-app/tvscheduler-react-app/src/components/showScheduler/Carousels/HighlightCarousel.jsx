import { useState, useEffect, useMemo } from "react";
import "./HighlightCarousel.css";
import useIntersectionObserver from "../../../hooks/useIntersectionObserver";
import CarouselCard from "./CarouselCard";

const HighlightCarousel = ({ carouselRef, section, addRemoveShow }) => {
  const [activeShow, setActiveShow] = useState(0);

  useEffect(() => {
    setTimeout(() => {
      setActiveShow(activeShow === section.length - 1 ? 0 : activeShow + 1);
    }, 4000);
  });

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
              id={id}
              addRemoveShow={addRemoveShow}
            />
          ))}
        </>
      ) : null}
    </div>
  );
};

export default HighlightCarousel;
