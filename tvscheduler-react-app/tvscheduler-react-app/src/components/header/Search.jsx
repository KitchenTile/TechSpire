import {
  useState,
  useContext,
  useRef,
  useMemo,
  memo,
  useCallback,
} from "react";
import "./Search.css";
import ChannelsContext from "../../contexts/channelsContext";
import useShowLookup from "../../hooks/useShowLookup";
import useDebounce from "../../hooks/useDebounce";
import SearchCard from "./SearchCard";
import useMergeAndFilter from "../../hooks/useMergeAndFilter";

const Search = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const debounceSearachTerm = useDebounce(searchTerm, 500);
  const channels = useContext(ChannelsContext);
  const containerRef = useRef(null);

  const mergeShows = useMergeAndFilter("All");

  //this function replaces previous
  const mergeAndSort = useMemo(() => {
    // filter the search input
    const filteredBySearch = mergeShows?.filter(
      (event) =>
        event.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        event.tagName.toLowerCase().includes(searchTerm.toLowerCase())
    );

    //go through all shows found and sigle out each unique ones
    const uniqueShows = [];
    const seenShowIds = new Set();

    filteredBySearch?.map((show) => {
      if (!seenShowIds.has(show.showId)) {
        uniqueShows.push(show);
        seenShowIds.add(show.showId);
      }
    });

    // create an object to retrun each unique show and its instance
    const showObjectAndEvents = {
      uniqueShows: uniqueShows,
      shows: filteredBySearch,
    };
    console.log(showObjectAndEvents.shows);

    return showObjectAndEvents;
  }, [channels, debounceSearachTerm]);

  const handleBlur = useCallback((e) => {
    // Delay to allow focus to move between children
    setTimeout(() => {
      if (
        containerRef.current &&
        !containerRef.current.contains(document.activeElement)
      ) {
        setSearchTerm("");
      }
    }, 200);
  }, []);

  return (
    <li className="search-container" ref={containerRef} onBlur={handleBlur}>
      <div className="search-body">
        <input
          type="text"
          placeholder="Channels, genres, etc."
          className="search-input"
          value={searchTerm}
          onChange={(event) => setSearchTerm(event.target.value)}
        />
        <svg
          width="30"
          height="30"
          viewBox="0 0 33 32"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            fillRule="evenodd"
            clipRule="evenodd"
            d="M19.0575 18.3905C19.5782 17.8698 20.4224 17.8698 20.9431 18.3905L28.2765 25.7239C28.7972 26.2446 28.7972 27.0888 28.2765 27.6095C27.7558 28.1302 26.9115 28.1302 26.3909 27.6095L19.0575 20.2761C18.5368 19.7554 18.5368 18.9112 19.0575 18.3905Z"
            fill="#212529"
          />
          <path
            fillRule="evenodd"
            clipRule="evenodd"
            d="M14.0003 6.66667C10.3184 6.66667 7.33366 9.65143 7.33366 13.3333C7.33366 17.0152 10.3184 20 14.0003 20C17.6822 20 20.667 17.0152 20.667 13.3333C20.667 9.65143 17.6822 6.66667 14.0003 6.66667ZM4.66699 13.3333C4.66699 8.17868 8.84567 4 14.0003 4C19.155 4 23.3337 8.17868 23.3337 13.3333C23.3337 18.488 19.155 22.6667 14.0003 22.6667C8.84567 22.6667 4.66699 18.488 4.66699 13.3333Z"
            fill="#212529"
          />
        </svg>
        <div className="search-results">
          {searchTerm !== "" ? (
            mergeAndSort.uniqueShows.length > 0 ? (
              mergeAndSort.uniqueShows
                .slice(0, 5)
                .map((show) => (
                  <SearchCard
                    key={"search" + show.showId}
                    show={show}
                    showEvents={mergeAndSort.shows.filter(
                      (showEvent) => showEvent.showId === show.showId
                    )}
                  />
                ))
            ) : (
              <div className="not-found-container">
                No results for {searchTerm}
              </div>
            )
          ) : null}
        </div>
      </div>
    </li>
  );
};

export default memo(Search);
