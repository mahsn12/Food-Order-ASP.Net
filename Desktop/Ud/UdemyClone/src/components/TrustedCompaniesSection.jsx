// src/components/TrustedCompaniesSection.jsx
import React from "react";
import { trustedCompanies } from "../data";

const TrustedCompaniesSection = () => {
  return (
    <section className="trusted">
      <p>Trusted by top companies worldwide</p>
      <div className="trusted-logos">
        {trustedCompanies.map((company, idx) => (
          <img
            key={idx}
            src={company.logo} // if you have real logos, replace these with URLs
            alt={company.name}
            title={company.name}
          />
        ))}
      </div>
    </section>
  );
};

export default TrustedCompaniesSection;
