// src/components/EssentialSkills.jsx
import React from "react";
import { careerSkills } from "../data";

const EssentialSkills = () => {
  return (
    <section className="essential-skills">
      <div className="essential-skills-container">
        <div className="essential-skills-text">
          <h2>Essential Skills</h2>
          <p className="description">
            Gain the most in-demand skills to advance your career and personal growth.
          </p>
        </div>

        <div className="essential-skills-content">
          <div className="skills-grid">
            {careerSkills.map((skill) => (
              <div key={skill.id} className="skill-item">
                <div className="skill-image">
                  <img src={skill.image} alt={skill.title} />
                </div>
                <div className="skill-name">{skill.title}</div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </section>
  );
};

export default EssentialSkills;
