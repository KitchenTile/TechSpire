import "./App.css";
import MainSchedulePage from "./pages/mainSchedulePage";
import LoginRegisterPage from "./pages/loginPage.jsx";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/main" element={<MainSchedulePage />} />
        <Route path="/login" element={<LoginRegisterPage />} />
      </Routes>
    </Router>
  );
}

export default App;
