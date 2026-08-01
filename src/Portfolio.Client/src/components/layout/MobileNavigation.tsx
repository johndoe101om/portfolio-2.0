import { useEffect } from 'react';
import { ChevronLeft, ChevronRight, Home, User, FileText, Briefcase, BookOpenText, Mail } from 'lucide-react';
import styles from './MobileNavigation.module.css';
import type { SectionId, NavItem } from '../../types';
import type { LucideIcon } from 'lucide-react';
import { getImageMetadata, optimizedImageUrl } from '../../utils/imageMetadata';

const ICON_MAP: Record<SectionId, LucideIcon> = {
  hero: Home,
  about: User,
  resume: FileText,
  portfolio: Briefcase,
  blog: BookOpenText,
  contact: Mail,
  godmode: Home,
};

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

export function MobileNavigation({
  isOpen,
  activeSection,
  navItems,
  profileName,
  profileImageUrl,
  cvUrl,
  onToggle,
  onNavigate,
  onPrev,
  onNext,
}: Props) {
  const optimizedProfileImageUrl = optimizedImageUrl(profileImageUrl);
  const profileImage = getImageMetadata(optimizedProfileImageUrl, { width: 512, height: 512 });

  useEffect(() => {
    document.body.style.overflow = isOpen ? 'hidden' : '';
    return () => {
      document.body.style.overflow = '';
    };
  }, [isOpen]);

  return (
    <>
      <button
        type="button"
        className={`${styles.hamburger} ${isOpen ? styles.open : ''}`}
        onClick={onToggle}
        aria-expanded={isOpen}
        aria-label={isOpen ? 'Close menu' : 'Open menu'}
        aria-controls="mobile-nav"
      >
        <span />
        <span />
        <span />
      </button>

      {isOpen && <div className={styles.overlay} onClick={onToggle} aria-hidden="true" />}

      <nav
        id="mobile-nav"
        className={`${styles.drawer} ${isOpen ? styles.drawerOpen : ''}`}
        aria-label="Mobile navigation"
        aria-hidden={!isOpen}
      >
        <div className={styles.drawerTop}>
          <div className={styles.profileBadge}>
            <div className={styles.avatar}>
              <img
                src={optimizedProfileImageUrl}
                alt={`${profileName} profile photo`}
                width={profileImage.width}
                height={profileImage.height}
                decoding="async"
                className={styles.avatarImg}
              />
            </div>
            <div>
              <div className={styles.profileName}>{profileName}</div>
              <a
                href={cvUrl}
                target="_blank"
                rel="noopener noreferrer"
                className={styles.cvLink}
                onClick={onToggle}
              >
                Download CV
              </a>
            </div>
          </div>
        </div>

        <ul className={styles.navList} role="list">
          {navItems.map((item) => {
            const Icon = ICON_MAP[item.id] ?? Home;
            return (
              <li key={item.id}>
                <a
                  href={`#${item.id}`}
                  className={`${styles.navLink} ${activeSection === item.id ? styles.active : ''}`}
                  aria-current={activeSection === item.id ? 'page' : undefined}
                  onClick={(e) => {
                    e.preventDefault();
                    onNavigate(item.id);
                    onToggle();
                  }}
                >
                  <Icon size={18} aria-hidden="true" />
                  {item.label}
                </a>
              </li>
            );
          })}
        </ul>
      </nav>

      <div className={styles.prevNext}>
        <button type="button" className={styles.navBtn} onClick={onPrev} aria-label="Previous section">
          <ChevronLeft size={20} />
        </button>
        <button type="button" className={styles.navBtn} onClick={onNext} aria-label="Next section">
          <ChevronRight size={20} />
        </button>
      </div>
    </>
  );
}
