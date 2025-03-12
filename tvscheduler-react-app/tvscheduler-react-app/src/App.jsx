import "./App.css";
import { lazy } from "react";
const MainPage = lazy(() => import("./pages/mainSchedulePage"));
import LoginRegisterPage from "./pages/loginPage.jsx";
import DaySegmentPage from "./pages/DaySegmentPage.jsx";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import FetchedInfoProvider from "./contexts/FetchedInfoProvider.jsx";
import MyShowsContextProvider from "./contexts/MyShowsContextProvider.jsx";

function App() {
  return (
    <Router>
      <Routes>
        <Route
          path="/main"
          element={
            <FetchedInfoProvider>
              <MyShowsContextProvider>
                <MainPage />
              </MyShowsContextProvider>
            </FetchedInfoProvider>
          }
        />
        <Route
          path="/time-segments/:section"
          element={
            <FetchedInfoProvider>
              <MyShowsContextProvider>
                <DaySegmentPage />
              </MyShowsContextProvider>
            </FetchedInfoProvider>
          }
        />
        <Route path="/" element={<LoginRegisterPage />} />
        <Route path="/*" element={<LoginRegisterPage />} />
      </Routes>
    </Router>
  );
}

export default App;
