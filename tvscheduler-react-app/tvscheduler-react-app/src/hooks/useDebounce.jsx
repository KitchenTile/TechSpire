import { useEffect, useState } from "react";

const useDebounce = (value, delay = 500) => {
  const [debounceValue, setDebounceValue] = useState(value);

  useEffect(() => {
    const timeOut = setTimeout(() => {
      console.log("setting timeout");
      setDebounceValue(value);
    }, delay);

    return () => {
      console.log("cleaing timeout");
      clearTimeout(timeOut);
    };
  }, [value, delay]);

  return debounceValue;
};

export default useDebounce;
