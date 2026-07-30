import { BookOpenText, Briefcase, FileText, Home, Mail, User } from 'lucide-react';
import styles from './Sidebar.module.css';
import type { SectionId } from '../../types';
import type { LucideIcon } from 'lucide-react';

interface NavDef {
  id: SectionId;
  icon: LucideIcon;
  label: string;
}

const NAV: NavDef[] = [
  { id: 'hero', icon: Home, label: 'Home' },
  { id: 'about', icon: User, label: 'About' },
  { id: 'resume', icon: FileText, label: 'Resume' },
  { id: 'portfolio', icon: Briefcase, label: 'Portfolio' },
  { id: 'blog', icon: BookOpenText, label: 'Blog' },
  { id: 'contact', icon: Mail, label: 'Contact' },
];

interface Props {
  active: SectionId;
  onNavigate: (s: SectionId) => void;
}

export function Sidebar({ active, onNavigate }: Props) {
  return (
    <nav className={styles.sidebar} aria-label="Main navigation">
      <button
        className={styles.logo}
        onClick={() => onNavigate('hero')}
        aria-label="Go to home"
      >
        SK
      </button>

      <ul className={styles.navList} role="list">
        {NAV.map((item) => {
          const Icon = item.icon;

          return (
            <li key={item.id}>
              <button
                className={`${styles.navItem} ${active === item.id ? styles.active : ''}`}
                onClick={() => onNavigate(item.id)}
                aria-current={active === item.id ? 'page' : undefined}
                aria-label={item.label}
              >
                <span className={styles.emoji} aria-hidden="true">
                  <Icon size={18} strokeWidth={2} />
                </span>
                <span className={styles.tooltip}>{item.label}</span>
              </button>
            </li>
          );
        })}
      </ul>
    </nav>
  );
}
