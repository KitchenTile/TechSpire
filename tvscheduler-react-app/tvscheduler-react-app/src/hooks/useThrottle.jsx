import { useRef, useEffect } from "react";

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
    const component = ref.current;
    if (!component) return;
    component.addEventListener("scroll", handleScroll);
    console.log("throttling");
    return () => component.removeEventListener("scroll", handleScroll);
  }, [ref, func]);
};

export default useThrottle;
