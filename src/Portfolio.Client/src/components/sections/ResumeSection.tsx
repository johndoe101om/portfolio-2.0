import styles from './ResumeSection.module.css';
import { useServices, useEducation, useSoftSkills } from '../../api/queries';
import type { Service, Education, Experience } from '../../types';

export function ResumeSection() {
  const { data: services = [] } = useServices();
  const { data: education = [] } = useEducation();
  const { data: softSkills = [] } = useSoftSkills();
  return (
    <section aria-labelledby="resume-heading" className={styles.wrap}>
      <div className={styles.header}>
        <div className="section-eyebrow">My journey</div>
        <h2 id="resume-heading" className="section-title">Education & services</h2>
      </div>
      <div className={styles.cols}>
        <div>
          <h3 className={styles.colTitle}>🎓 Education</h3>
          <div className={styles.timeline}>
            {(education as Education[]).map((e) => (
              <div key={e.id} className={styles.tlItem}>
                <div className={styles.tlDot} aria-hidden="true" />
                <div className={styles.tlDate}>{e.period}</div>
                <div className={styles.tlTitle}>{e.institution}</div>
                <div className={styles.tlDesc}>{e.description}</div>
              </div>
            ))}
          </div>
        </div>
        <div>
          <h3 className={styles.colTitle}>⚡ Services</h3>
          <div className={styles.servicesList}>
            {(services as Service[]).map((s) => (
              <div key={s.id} className={`${styles.svcCard} glass-card`}>
                <i className={`${s.iconClass} ${styles.svcIcon}`} aria-hidden="true" />
                <div>
                  <h4 className={styles.svcTitle}>{s.title}</h4>
                  <p className={styles.svcDesc}>{s.description}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </section>
  );
}
