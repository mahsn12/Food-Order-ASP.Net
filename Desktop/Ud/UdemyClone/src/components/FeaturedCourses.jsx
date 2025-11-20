// src/components/FeaturedCourses.jsx
import React from "react";
import { courses, skillCategories } from "../data";

const FeaturedCourses = () => {
  return (
    <section className="featured-courses">
      <h2>Featured Courses</h2>
      <p className="description">Top courses picked for you</p>

      {/* Tags */}
      <div className="tags-container">
        {skillCategories.map((tag, idx) => (
          <span key={idx} className="tag">{tag}</span>
        ))}
      </div>

      {/* Courses Grid */}
      <div className="courses-grid">
        {courses.map((c) => (
          <div key={c.id} className="course-card">
            {c.badge && <span className="course-badge">{c.badge}</span>}
            <h3 className="course-title">{c.title}</h3>
            <p className="instructor">{c.instructor}</p>
            <div className="rating">
              <span className="stars">{c.rating}★</span>
              <span className="reviews">({c.ratingCount})</span>
            </div>
            <p className="price">{c.price}</p>
          </div>
        ))}
      </div>

      <button className="show-all-btn">Show all courses</button>
    </section>
  );
};

export default FeaturedCourses;
