import { promoData } from '../data';

export default function PromoBanner() {
  const [first, second] = promoData.text.split('|');

  return (
    <div className="promo-banner">
      <div className="promo-content">
        <span className="promo-text-bold">{first}</span>
        <span className="promo-text">{second}</span>
        <span className="promo-timer">{promoData.timer}</span>
        <button className="promo-btn">{promoData.buttonText}</button>
      </div>
    </div>
  );
}
