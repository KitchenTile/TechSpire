import { useRef, useEffect } from "react";

// We do a bunch of scrilling literally everywhere and to improve performance this is a must. Throttles the amount of times scrolling func gets triggered -- BLUE
const useThrottle = (ref, func) => {
  const ticking = useRef(false);
  const handleScroll = () => {
    if (!ticking.current) {
      window.requestAnimationFrame(() => {
        func();
        ticking.current = false;
      });
      ticking.current = true;
    }
  };

  useEffect(() => {
    // Decide where to attach the scroll event listener:
    const element = ref && ref.current ? ref.current : window;
    element.addEventListener("scroll", handleScroll);
    return () => element.removeEventListener("scroll", handleScroll);
  }, [ref, func]);
};

export default useThrottle;
