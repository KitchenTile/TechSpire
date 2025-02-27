import "./App.css";
import { lazy } from "react";
const MainPage = lazy(() => import("./pages/mainSchedulePage"));
import LoginRegisterPage from "./pages/loginPage.jsx";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/main" element={<MainPage />} />
        <Route path="/" element={<LoginRegisterPage />} />
        <Route path="/*" element={<LoginRegisterPage />} />
      </Routes>
    </Router>
  );
}

export default App;
