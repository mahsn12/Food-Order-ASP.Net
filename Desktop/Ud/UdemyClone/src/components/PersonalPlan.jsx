import React from "react";
import { Star, Brain, Award } from "lucide-react";

const PersonalPlan = () => {
  const features = [
    { icon: <Star />, text: "Unlimited access to courses" },
    { icon: <Brain />, text: "Learn at your own pace" },
    { icon: <Award />, text: "Certifications included" },
  ];

  return (
    <section className="personal-plan">
      <div className="personal-plan-card">
        <div className="personal-plan-content">
          <div className="personal-plan-text">
            <h2>Your Personal Plan</h2>
            <p className="description">
              Upgrade your skills with our personalized plan designed just for you.
            </p>
            <ul className="features-list">
              {features.map((f, idx) => (
                <li key={idx} className="feature-item">
                  <div className="feature-icon">{f.icon}</div>
                  <span className="ud-text-md">{f.text}</span>
                </li>
              ))}
            </ul>
            <div className="plan-footer">
              <button className="learn-more-btn">Learn More</button>
              <p className="price">Starting from $19.99/month</p>
            </div>
          </div>
          <div className="personal-plan-image">
            <img src="https://frontends.udemycdn.com/staticx/udemy/images/ai-career-banner/ai-career@1x.webp" alt="Plan" />
          </div>
        </div>
      </div>
    </section>
  );
};

export default PersonalPlan;
