import { createContext } from "react";

const MyShowsContext = createContext({
  myShows: [],
  addRemoveShow: () => {},
});

export default MyShowsContext;
