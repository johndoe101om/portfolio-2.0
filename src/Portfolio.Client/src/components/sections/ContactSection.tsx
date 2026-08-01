import styles from './ContactSection.module.css';
import { ContactForm } from '../contact/ContactForm';
import { useProfile } from '../../api/queries';

const INFO = [
  { icon: 'EM', title: 'Email me', key: 'email' as const, color: 'rgba(0,212,255,.1)' },
  { icon: 'PH', title: 'Call or text', key: 'phone' as const, color: 'rgba(245,158,11,.1)' },
  { icon: 'IN', title: 'Based in', key: 'city' as const, color: 'rgba(139,92,246,.1)' },
];

export function ContactSection() {
  const { data: profile } = useProfile();
  const vals: Record<string, string> = {
    email: profile?.email ?? 'sirsatyamchaudhary@gmail.com',
    phone: profile?.phone ?? '+91 9113394936',
    city: `${profile?.city ?? 'Chennai'}, ${profile?.country ?? 'India'}`,
  };

  return (
    <section aria-labelledby="contact-heading" className={styles.wrap}>
      <div className={styles.header}>
        <div className="section-eyebrow">Let's connect</div>
        <h2 id="contact-heading" className="section-title">Contact Satyam Kumar Chaudhary</h2>
        <p className="section-sub">Have a project in mind? I would love to hear about it.</p>
      </div>
      <div className={styles.layout}>
        <div className={styles.infoCol}>
          {INFO.map((item) => (
            <div key={item.key} className={`${styles.infoCard} glass-card`}>
              <div className={styles.infoIcon} style={{ background: item.color }}>{item.icon}</div>
              <div>
                <div className={styles.infoTitle}>{item.title}</div>
                <div className={styles.infoVal}>{vals[item.key]}</div>
              </div>
            </div>
          ))}
          <div className={`${styles.infoCard} glass-card`}>
            <div className={styles.infoIcon} style={{ background: 'rgba(99,102,241,.1)' }}>LI</div>
            <div>
              <div className={styles.infoTitle}>LinkedIn</div>
              <a href="https://www.linkedin.com/in/satyam-webdeveloper/" target="_blank" rel="noopener noreferrer" className={styles.infoLink}>
                LinkedIn profile for Satyam Kumar Chaudhary
              </a>
            </div>
          </div>
          <div className={`${styles.infoCard} glass-card`}>
            <div className={styles.infoIcon} style={{ background: 'rgba(15,23,42,.4)' }}>GH</div>
            <div>
              <div className={styles.infoTitle}>GitHub</div>
              <a href="https://github.com/satyam6290" target="_blank" rel="noopener noreferrer" className={styles.infoLink}>
                GitHub projects by Satyam Kumar Chaudhary
              </a>
            </div>
          </div>
          <div className={`${styles.infoCard} glass-card`}>
            <div className={styles.infoIcon} style={{ background: 'rgba(14,165,233,.12)' }}>X</div>
            <div>
              <div className={styles.infoTitle}>X / Twitter</div>
              <a href="https://x.com/codersatyam" target="_blank" rel="noopener noreferrer" className={styles.infoLink}>
                codersatyam on X
              </a>
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
