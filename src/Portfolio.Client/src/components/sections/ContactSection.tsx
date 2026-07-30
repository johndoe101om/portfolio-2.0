import styles from './ContactSection.module.css';
import { ContactForm } from '../contact/ContactForm';
import { useProfile } from '../../api/queries';

const INFO = [
  { emoji: '📧', title: 'Email me', key: 'email' as const, color: 'rgba(0,212,255,.1)' },
  { emoji: '📱', title: 'Call or text', key: 'phone' as const, color: 'rgba(245,158,11,.1)' },
  { emoji: '📍', title: 'Based in', key: 'city' as const, color: 'rgba(139,92,246,.1)' },
];

export function ContactSection() {
  const { data: profile } = useProfile();
  const vals: Record<string, string> = {
    email: profile?.email ?? 'sirsatyamchaudhary@gmail.com',
    phone: profile?.phone ?? '+91 9113394936',
    city:  `${profile?.city ?? 'Chennai'}, ${profile?.country ?? 'India'}`,
  };
  return (
    <section aria-labelledby="contact-heading" className={styles.wrap}>
      <div className={styles.header}>
        <div className="section-eyebrow">Let's connect</div>
        <h2 id="contact-heading" className="section-title">Get in touch</h2>
        <p className="section-sub">Have a project in mind? I'd love to hear about it.</p>
      </div>
      <div className={styles.layout}>
        <div className={styles.infoCol}>
          {INFO.map((item) => (
            <div key={item.key} className={`${styles.infoCard} glass-card`}>
              <div className={styles.infoIcon} style={{ background: item.color }}>{item.emoji}</div>
              <div>
                <div className={styles.infoTitle}>{item.title}</div>
                <div className={styles.infoVal}>{vals[item.key]}</div>
              </div>
            </div>
          ))}
          <div className={`${styles.infoCard} glass-card`}>
            <div className={styles.infoIcon} style={{ background: 'rgba(99,102,241,.1)' }}>💼</div>
            <div>
              <div className={styles.infoTitle}>LinkedIn</div>
              <a href="https://www.linkedin.com/in/satyam-webdeveloper/" target="_blank" rel="noopener noreferrer" className={styles.infoLink}>satyam-webdeveloper</a>
            </div>
          </div>
        </div>
        <div className={`${styles.formWrap} glass-card`}>
          <ContactForm />
        </div>
      </div>
    </section>
  );
}
