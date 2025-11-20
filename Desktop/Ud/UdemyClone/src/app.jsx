import React from "react";
import OfferBanner from "./components/OfferBanner.jsx";
import Navbar from "./components/NavBar.jsx";
import HeroSection from "./components/HeroSection.jsx";
import EssentialSkills from "./components/EssentialSkills.jsx";
import PersonalPlan from "./components/PersonalPlan.jsx";
import FeaturedCourses from "./components/FeaturedCourses.jsx";
import TrustedCompaniesSection from "./components/TrustedCompaniesSection.jsx";
import "./global.css";

const App = () => {
  return (
    <div className="App">
      <OfferBanner />
      <Navbar />
      <HeroSection />
      <EssentialSkills />
      <PersonalPlan />
      <FeaturedCourses />
      <TrustedCompaniesSection />
    </div>
  );
};

export default App;
