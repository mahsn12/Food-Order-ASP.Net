import { careerSkills } from '../data';
import { Users, ArrowRight } from 'lucide-react';

export default function CareerSkillsSection() {
  return (
    <div className="career-skills-section">
      <div className="skills-intro">
        <h2>Learn essential career and life skills</h2>
        <p>
          Udemy helps you build in-demand skills fast and advance your career in a changing job market.
        </p>
      </div>

      <div className="skills-grid">
        {careerSkills.map((skill) => (
          <div key={skill.id} className="skill-card">
            <img src={skill.image} alt={skill.title} />
            <div className="skill-info">
              <div>
                <Users />
                <span>{skill.studentCount}</span>
              </div>
              <div>
                <h3>{skill.title}</h3>
                <ArrowRight />
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
