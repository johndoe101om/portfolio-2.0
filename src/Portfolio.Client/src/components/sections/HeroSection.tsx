import { useState, useEffect } from 'react';
import { Github, Linkedin, Instagram, MessageCircle, Twitter } from 'lucide-react';
import styles from './HeroSection.module.css';
import { useProfile } from '../../api/queries';
import type { SectionId } from '../../types';
import { getImageMetadata, optimizedImageUrl } from '../../utils/imageMetadata';

const ROLES = ['Full-Stack Developer', 'DevOps Engineer', 'Cloud Architect', 'App Developer'];

interface Props {
  onNavigate: (s: SectionId) => void;
}

export function HeroSection({ onNavigate }: Props) {
  const { data: profile } = useProfile();
  const [roleIdx, setRoleIdx] = useState(0);
  const profileImageUrl = optimizedImageUrl(profile?.profileImageUrl ?? '/assets/images/profile-lcp.jpg');
  const profileImage = getImageMetadata(profileImageUrl, { width: 512, height: 512 });

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
          <span className={styles.nameHighlight}>Satyam Kumar Chaudhary</span>
          {' - Full-Stack Developer & DevOps Engineer'}
        </h1>

        <p className={styles.role}>
          {'Building production web platforms - '}
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
          {profile?.aboutText ??
            'Satyam Kumar Chaudhary is a full-stack developer and DevOps engineer in Chennai, India, building scalable web applications, APIs, automation pipelines, and cloud deployments.'}
        </p>

        <div className={styles.actions}>
          <button className="btn-primary" onClick={() => onNavigate('portfolio')}>
            View my work
          </button>
          <button className="btn-outline" onClick={() => onNavigate('contact')}>
            Let's talk
          </button>
        </div>

        <div className={styles.socials}>
          <a
            href="https://www.linkedin.com/in/satyam-webdeveloper/"
            target="_blank"
            rel="noopener noreferrer"
            aria-label="View Satyam Kumar Chaudhary on LinkedIn"
            className={styles.socialLink}
          >
            <Linkedin size={18} />
          </a>
          <a
            href="https://github.com/satyam6290"
            target="_blank"
            rel="noopener noreferrer"
            aria-label="View Satyam Kumar Chaudhary on GitHub"
            className={styles.socialLink}
          >
            <Github size={18} />
          </a>
          <a
            href="https://x.com/codersatyam"
            target="_blank"
            rel="noopener noreferrer"
            aria-label="View codersatyam on X"
            className={styles.socialLink}
          >
            <Twitter size={18} />
          </a>
          <a
            href="https://www.instagram.com/be_stranger7964/"
            target="_blank"
            rel="noopener noreferrer"
            aria-label="View Satyam Kumar Chaudhary on Instagram"
            className={styles.socialLink}
          >
            <Instagram size={18} />
          </a>
          <a
            href="https://wa.me/qr/TZU5O77ZT4MGN1"
            target="_blank"
            rel="noopener noreferrer"
            aria-label="Message Satyam Kumar Chaudhary on WhatsApp"
            className={styles.socialLink}
          >
            <MessageCircle size={18} />
          </a>
        </div>
      </div>

      <div className={styles.visual}>
        <div className={styles.avatarRing} aria-hidden="true" />
        <div className={styles.avatar}>
          <img
            src={profileImageUrl}
            alt="Portrait of Satyam Kumar Chaudhary, full-stack developer and DevOps engineer in Chennai"
            width={profileImage.width}
            height={profileImage.height}
            loading="eager"
            decoding="async"
            className={styles.avatarImg}
          />
        </div>
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
