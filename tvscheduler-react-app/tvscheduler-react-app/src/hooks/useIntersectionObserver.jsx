import { useState, useEffect, useRef } from "react";

const useIntersectionObserver = (options) => {
  //checks if the component is being observed
  const [isIntersecting, setIsIntersecting] = useState(false);

  //set compoenent we're gonna be observing
  const component = useRef(null);

  useEffect(() => {
    const observer = new IntersectionObserver(([entry]) => {
      setIsIntersecting(entry.isIntersecting);
    }, options);

    if (component.current) {
      observer.observe(component.current);
    } else if (!component.current) {
      return;
    }

    //cleanup func to disconnects the current component being observed
    return () => {
      if (component.current) {
        observer.unobserve(component.current);
        observer.disconnect();
      }
    };
  }, [options]);

  return [component, isIntersecting];
};

export default useIntersectionObserver;
