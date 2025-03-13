import {
  useState,
  useRef,
  useMemo,
  useCallback,
  useContext,
  memo,
} from "react";
import "./SectionCarouselComponent.css";
import HighlightCarousel from "./HighlightCarousel";
import useThrottle from "../../../hooks/useThrottle";
import useMergeAndFilter from "../../../hooks/useMergeAndFilter";

const SectionCarouselComponent = () => {
  const [activeSection, setActiveSection] = useState(0);
  const carouselSectionRef = useRef(null);
  const mergeShowsPerSection = useMergeAndFilter("carousel");

  const handleScroll = useCallback(() => {
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
    }
  }, []);

  useThrottle(carouselSectionRef, handleScroll);

  // function to skip forwards and backwards between the show cards
  const handleClick = useCallback(
    (skipAmount) => {
      carouselSectionRef.current.scrollLeft =
        carouselSectionRef.current.scrollLeft + skipAmount;
    },
    [carouselSectionRef]
  );

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
      <div className="carousel-container" ref={carouselSectionRef}>
        {Object.entries(mergeShowsPerSection).map((section, idx) => (
          <HighlightCarousel key={idx} section={section[1]} />
        ))}
      </div>
      <div className="section-indicators">
        {Object.keys(mergeShowsPerSection).map((section, idx) => (
          <span
            key={idx}
            className={`indicator ${activeSection === idx ? "active" : ""}`}
            onClick={() => {
              activeSection !== idx
                ? handleClick(
                    carouselSectionRef.current.clientWidth *
                      (idx - activeSection)
                  )
                : console.log("current section");
            }}
          >
            {section}
          </span>
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

export default memo(SectionCarouselComponent);
