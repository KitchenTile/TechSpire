import { useCallback, useContext, useMemo } from "react";
import MyShowsContext from "./myShowsContext";
import AddRemoveShowsContext from "./AddRemoveShowsContext";

const AddRemoveShowsContextProvider = ({ children }) => {
  const { setMyShows, myShows } = useContext(MyShowsContext);

  const addShowCall = async (showEventId) => {
    const token = localStorage.getItem("JWToken");
    try {
      const response = await fetch(
        "http://localhost:5171/Account/add-show-to-schedule",
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ showEventId }),
        }
      );
      if (!response.ok) {
        throw new Error("Failed to post show addition");
      }
    } catch (err) {
      console.error(err);
    }
  };

  const removeShowCall = async (showEventId) => {
    const token = localStorage.getItem("JWToken");
    try {
      const response = await fetch(
        "http://localhost:5171/remove-show-from-schedule",
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ showEventId }),
        }
      );
      if (!response.ok) {
        throw new Error("Failed to post show removal");
      }
    } catch (err) {
      console.error(err);
    }
  };

  //add shows to state pass -- pass function to component as prop (ShowCard)
  const addRemoveShow = useCallback(async (showEventId) => {
    setMyShows((prevMyShows) => {
      if (!prevMyShows.includes(showEventId)) {
        addShowCall(showEventId);
        return [...prevMyShows, showEventId];
      } else {
        removeShowCall(showEventId);
        return prevMyShows.filter((id) => id !== showEventId);
      }
    });
  }, []);

  const addRemoveValue = useMemo(() => ({ addRemoveShow }), []);

  return (
    <AddRemoveShowsContext.Provider value={addRemoveValue}>
      {children}
    </AddRemoveShowsContext.Provider>
  );
};

export default AddRemoveShowsContextProvider;
