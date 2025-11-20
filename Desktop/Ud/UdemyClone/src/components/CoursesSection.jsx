import { courses, skillCategories } from '../data';
import { Star, ChevronRight } from 'lucide-react';

export default function CoursesSection() {
  return (
    <div className="courses-section">
      <h2>Skills to transform your career and life</h2>
      <p>
        From critical skills to technical topics, Udemy supports your professional development.
      </p>

      <div className="course-categories">
        {skillCategories.map((category, index) => (
          <button key={index}>{category}</button>
        ))}
      </div>

      <div className="courses-grid">
        {courses.map((course) => (
          <div key={course.id} className="course-card">
            <img src={course.image} alt={course.title} />
            <h3>{course.title}</h3>
            <p>{course.instructor}</p>
            <div className="course-rating">
              <span>{course.rating}</span>
              {[...Array(5)].map((_, i) => (
                <Star key={i} />
              ))}
              <span>({course.ratingCount})</span>
            </div>
            <div className="course-price-badge">
              <span>{course.price}</span>
              <span>{course.badge}</span>
            </div>
          </div>
        ))}
      </div>

      <button className="show-all-btn">
        Show all Artificial Intelligence (AI) courses
        <ChevronRight />
      </button>
    </div>
  );
}
