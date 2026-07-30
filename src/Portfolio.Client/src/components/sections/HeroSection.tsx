import { useState, useEffect } from 'react';
import styles from './HeroSection.module.css';
import { useProfile } from '../../api/queries';

const ROLES = ['Full-Stack Developer', 'DevOps Engineer', 'Cloud Architect', 'App Developer'];

import type { SectionId } from '../../types';
interface Props { onNavigate: (s: SectionId) => void; }

export function HeroSection({ onNavigate }: Props) {
  const { data: profile } = useProfile();
  const [roleIdx, setRoleIdx] = useState(0);

  useEffect(() => {
    const t = setInterval(() => setRoleIdx((i) => (i + 1) % ROLES.length), 2500);
    return () => clearInterval(t);
  }, []);

  return (
    <section className={styles.hero} aria-labelledby="hero-name">
      <div className={styles.content}>
        <div className={styles.tag}>
          <span className={styles.dot} aria-hidden="true" />
          Open to opportunities
        </div>

        <h1 id="hero-name" className={styles.name}>
          Hi, I'm{' '}
          <span className={styles.nameHighlight}>
            {profile?.fullName ?? 'Satyam Kumar'}
          </span>
        </h1>

        <p className={styles.role}>
          {'Building the web · '}
          <strong
            className={styles.roleAnim}
            key={roleIdx}
            aria-live="polite"
            aria-atomic="true"
          >
            {ROLES[roleIdx]}
          </strong>
        </p>

        <p className={styles.desc}>
          {profile?.aboutText ?? 'Spirited software engineer with a love for clean code and bold ideas. I craft scalable, user-first applications and enjoy turning complex problems into elegant solutions.'}
        </p>

        <div className={styles.actions}>
          <button className="btn-primary" onClick={() => onNavigate('portfolio')}>
            View my work →
          </button>
          <button className="btn-outline" onClick={() => onNavigate('contact')}>
            Let's talk
          </button>
        </div>

        <div className={styles.socials}>
          <a href="https://www.linkedin.com/in/satyam-webdeveloper/" target="_blank" rel="noopener noreferrer" aria-label="in - LinkedIn" className={styles.socialLink}>in</a>
          <a href="https://www.instagram.com/be_stranger7964/" target="_blank" rel="noopener noreferrer" aria-label="ig - Instagram" className={styles.socialLink}>ig</a>
          <a href="https://wa.me/qr/TZU5O77ZT4MGN1" target="_blank" rel="noopener noreferrer" aria-label="wa - WhatsApp" className={styles.socialLink}>wa</a>
        </div>
      </div>

      <div className={styles.visual}>
        <div className={styles.avatarRing} aria-hidden="true" />
        <div className={styles.avatar}>🧑‍💻</div>
        <div className={styles.floatCard} style={{ top: '-18px', right: '-18px' }}>
          <span className={styles.floatVal}>40+</span>
          <span className={styles.floatLbl}>Projects</span>
        </div>
        <div className={styles.floatCard} style={{ bottom: '-18px', left: '-18px' }}>
          <span className={styles.floatVal}>4 yrs</span>
          <span className={styles.floatLbl}>Experience</span>
        </div>
      </div>
    </section>
  );
}
