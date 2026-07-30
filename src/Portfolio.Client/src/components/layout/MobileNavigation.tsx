import { useEffect } from 'react';
import styles from './MobileNavigation.module.css';
import type { SectionId, NavItem } from '../../types';

interface Props {
  isOpen: boolean;
  activeSection: SectionId;
  navItems: NavItem[];
  profileName: string;
  profileImageUrl: string;
  cvUrl: string;
  onToggle: () => void;
  onNavigate: (section: SectionId) => void;
  onPrev: () => void;
  onNext: () => void;
}

export function MobileNavigation({ isOpen, activeSection, navItems, profileName, profileImageUrl, cvUrl, onToggle, onNavigate, onPrev, onNext }: Props) {
  useEffect(() => { document.body.style.overflow = isOpen ? 'hidden' : ''; return () => { document.body.style.overflow = ''; }; }, [isOpen]);
  return (
    <>
      <button type="button" className={`${styles.hamburger} ${isOpen ? styles.open : ''}`} onClick={onToggle} aria-expanded={isOpen} aria-label={isOpen ? 'Close menu' : 'Open menu'} aria-controls="mobile-nav">
        <span /><span /><span />
      </button>
      {isOpen && <div className={styles.overlay} onClick={onToggle} aria-hidden="true" />}
      <nav id="mobile-nav" className={`${styles.drawer} ${isOpen ? styles.drawerOpen : ''}`} aria-label="Mobile navigation" aria-hidden={!isOpen}>
        <div className={styles.drawerTop}>
          <div className={styles.profileBadge}>
            <div className={styles.avatar}>🧑‍💻</div>
            <div>
              <div className={styles.profileName}>{profileName}</div>
              <a href={cvUrl} target="_blank" rel="noopener noreferrer" className={styles.cvLink} onClick={onToggle}>Download CV</a>
            </div>
          </div>
        </div>
        <ul className={styles.navList} role="list">
          {navItems.map((item) => (
            <li key={item.id}>
              <a href={`#${item.id}`} className={`${styles.navLink} ${activeSection === item.id ? styles.active : ''}`} aria-current={activeSection === item.id ? 'page' : undefined}
                onClick={(e) => { e.preventDefault(); onNavigate(item.id); onToggle(); }}>
                <i className={item.iconClass} aria-hidden="true" />
                {item.label}
              </a>
            </li>
          ))}
        </ul>
      </nav>
      <div className={styles.prevNext}>
        <button type="button" className={styles.navBtn} onClick={onPrev} aria-label="Previous section"><i className="bi bi-chevron-left" /></button>
        <button type="button" className={styles.navBtn} onClick={onNext} aria-label="Next section"><i className="bi bi-chevron-right" /></button>
      </div>
    </>
  );
}
