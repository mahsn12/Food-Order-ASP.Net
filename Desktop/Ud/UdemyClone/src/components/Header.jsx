import { Search, ShoppingCart, Globe } from 'lucide-react';
import { navLinks } from '../data';

export default function Header() {
  return (
    <header className="header">
      <div className="header-inner">
        <div className="header-left">
          <div className="logo">
            <svg className="logo-svg" viewBox="0 0 91 34" fill="none">
              {/* SVG paths */}
            </svg>
          </div>
          <button className="explore-btn">Explore</button>
          <div className="search-container">
            <Search className="search-icon" />
            <input type="text" placeholder="Search for anything" />
          </div>
        </div>
        <div className="header-right">
          {navLinks.map((link, index) => (
            <a key={index} href={link.href}>
              {link.text}
            </a>
          ))}
          <button className="cart-btn">
            <ShoppingCart />
          </button>
          <button className="login-btn">Log in</button>
          <button className="signup-btn">Sign up</button>
          <button className="globe-btn">
            <Globe />
          </button>
        </div>
      </div>
    </header>
  );
}
