import { useEffect, useState, useRef } from "react";
import "./SectionCarouselComponent.css";
import HighlightCarousel from "./HighlightCarousel";
import morning from "../../../assets/MORNING.png";

const SectionCarouselComponent = () => {
  const [activeSection, setActiveSection] = useState(1);
  const carouselSectionRef = useRef(null);
  const [scrollPosition, setScrollPosition] = useState(0);

  const sections = [
    { section: "Morning", src: morning },
    { section: "Afternoon", src: morning },
    { section: "evening", src: morning },
  ];

  const updateScrollPosition = () => {
    const container = carouselSectionRef.current;
    if (container) {
      const desiredScrollLeft = activeSection * container.clientWidth;
      container.scrollTo({ left: desiredScrollLeft, behavior: "smooth" });
    }
  };

  // Update scroll position when activeSection changes
  useEffect(() => {
    updateScrollPosition();
    console.log(activeSection);
  }, [activeSection]);

  return (
    <div className="carousel-section">
      <span
        className="arrows left"
        onClick={() => {
          activeSection > 0 ? setActiveSection(activeSection - 1) : null;
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
        {sections.map((section, id) => (
          <HighlightCarousel key={id} section={section} />
        ))}
      </div>
      <span
        className="arrows right"
        onClick={() => {
          activeSection < sections.length - 1
            ? setActiveSection(activeSection + 1)
            : null;
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
