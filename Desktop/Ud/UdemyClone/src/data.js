// src/data.js
import { Brain, Award, Monitor, TrendingUp } from "lucide-react";

// Banner Section
export const bannerData = {
  title: "Jump into learning — for less",
  description:
    "If you're new to Udemy, we've got good news: For a limited time, courses start at just E£259.99 for new learners!",
  buttonText: "Sign up now",
};

// Promo Banner Section
export const promoData = {
  text: "New-learner offer | Courses from E£259.99. Click button to see savings.",
  timer: "Ends in 5h 56m 33s.",
  buttonText: "Click to redeem",
};

// Nav links for Header
export const navLinks = [
  { text: "Plans & Pricing", href: "#" },
  { text: "Udemy Business", href: "#" },
  { text: "Teach on Udemy", href: "#" },
];

// Career Skills Section
export const careerSkills = [
  {
    id: 1,
    title: "Generative AI",
    studentCount: "1M+",
    image:
      "https://images.pexels.com/photos/8386440/pexels-photo-8386440.jpeg?auto=compress&cs=tinysrgb&w=600",
  },
  {
    id: 2,
    title: "IT Certifications",
    studentCount: "14.4M+",
    image:
      "https://images.pexels.com/photos/5989931/pexels-photo-5989931.jpeg?auto=compress&cs=tinysrgb&w=600",
  },
  {
    id: 3,
    title: "Data Science",
    studentCount: "8M+",
    image:
      "https://images.pexels.com/photos/6801648/pexels-photo-6801648.jpeg?auto=compress&cs=tinysrgb&w=600",
  },
];

// Personal Plan Section
export const personalPlanData = {
  title: "Reimagine your career in the AI era",
  description:
    "Future-proof your skills with Personal Plan. Get access to a variety of fresh content from real-world experts.",
  features: [
    { icon: "brain", text: "Learn AI and more" },
    { icon: "award", text: "Prep for a certification" },
    { icon: "monitor", text: "Practice with AI coaching" },
    { icon: "trending-up", text: "Advance your career" },
  ],
  buttonText: "Learn more",
  price: "Starting at E£204.00/month",
};

// Skill categories for Courses Section
export const skillCategories = [
  "Artificial Intelligence (AI)",
  "Python",
  "Microsoft Excel",
  "AI Agents & Agentic AI",
  "Digital Marketing",
  "Amazon AWS",
];

// Courses Section
export const courses = [
  {
    id: 1,
    title: "The AI Engineer Course 2025: Complete AI Engineer Bootcamp",
    instructor: "365 Careers",
    rating: 4.6,
    ratingCount: "10,801 ratings",
    price: "E£349.99",
    badge: "Bestseller",
    image:
      "https://images.pexels.com/photos/8386440/pexels-photo-8386440.jpeg?auto=compress&cs=tinysrgb&w=600",
  },
  {
    id: 2,
    title: "Intro to AI Agents and Agentic AI",
    instructor: "365 Careers",
    rating: 4.4,
    ratingCount: "895 ratings",
    price: "E£349.99",
    badge: "Bestseller",
    image:
      "https://images.pexels.com/photos/8438918/pexels-photo-8438918.jpeg?auto=compress&cs=tinysrgb&w=600",
  },
  {
    id: 3,
    title: "Artificial Intelligence (AI) Foundations for Developers",
    instructor: "Rahul Rajat Singh",
    rating: 5.0,
    ratingCount: "21 ratings",
    price: "E£349.99",
    badge: "Highest Rated",
    image:
      "https://images.pexels.com/photos/8439093/pexels-photo-8439093.jpeg?auto=compress&cs=tinysrgb&w=600",
  },
  {
    id: 4,
    title: "The Complete Guide To AI Powered Salesforce Development",
    instructor: "Matt Gerry",
    rating: 4.8,
    ratingCount: "74 ratings",
    price: "E£349.99",
    badge: "Hot & New",
    image:
      "https://images.pexels.com/photos/7172831/pexels-photo-7172831.jpeg?auto=compress&cs=tinysrgb&w=600",
  },
];

// Trusted Companies Section
export const trustedCompanies = [
  {
    name: "Volkswagen",
    logo: "https://www.google.com/imgres?q=volkswagen&imgurl=https%3A%2F%2Fupload.wikimedia.org%2Fwikipedia%2Fcommons%2F6%2F6d%2FVolkswagen_logo_2019.svg&imgrefurl=https%3A%2F%2Fen.wikipedia.org%2Fwiki%2FVolkswagen&docid=J-TUIWPHsSUgJM&tbnid=d8fM8Ym4n16s0M&vet=12ahUKEwiq3NKuzYGRAxWhS_EDHfFNIRMQM3oECBMQAA..i&w=800&h=800&hcb=2&ved=2ahUKEwiq3NKuzYGRAxWhS_EDHfFNIRMQM3oECBMQAA",
  },
  {
    name: "Samsung",
    logo: "https://images.samsung.com/is/image/samsung/assets/global/about-us/brand/logo/256_144_2.png?$512_N_PNG$",
  },
  {
    name: "Cisco",
    logo: "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQiAU8Uy2yHZp0DcgPqYT-WGtLc0NaYDKRfGQ&s",
  },
  {
    name: "Vimeo",
    logo: "https://upload.wikimedia.org/wikipedia/commons/9/9c/Vimeo_Logo.svg",
  }
];

// Map for icons in Personal Plan Section
export const iconMap = {
  brain: Brain,
  award: Award,
  monitor: Monitor,
  "trending-up": TrendingUp,
};
