import React from "react";
import { ShoppingCart, Globe } from "lucide-react";

const Navbar = () => {
  return (
    <nav className="navbar">
      <div className="left">
        <a href="#" className="logo">Udemy</a>
        <span className="explore">Explore</span>
        <div className="search-container">
          <input type="text" className="search" placeholder="Search courses..." />
        </div>
      </div>
      <div className="right">
        <a href="#" className="nav-link">Categories</a>
        <a href="#" className="nav-link">Wishlist</a>
        <div className="nav-icons">
          <ShoppingCart className="nav-icon" />
          <Globe className="nav-icon" />
        </div>
        <button className="login">Login</button>
        <button className="signup">Sign Up</button>
      </div>
    </nav>
  );
};

export default Navbar;
