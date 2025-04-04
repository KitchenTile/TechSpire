import { useEffect, useState } from "react";

// Made this hook to have an alterative for throttling, currently being used for the search component's input -- BLUE
const useDebounce = (value, delay = 500) => {
  const [debounceValue, setDebounceValue] = useState(value);

  useEffect(() => {
    const timeOut = setTimeout(() => {
      setDebounceValue(value);
    }, delay);

    return () => {
      clearTimeout(timeOut);
    };
  }, [value, delay]);

  return debounceValue;
};

export default useDebounce;
