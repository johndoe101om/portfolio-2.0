import { useState } from 'react';
import styles from './PortfolioSection.module.css';
import { useProjects } from '../../api/queries';
import type { Project } from '../../types';

const FILTERS: { value: string; label: string }[] = [
  { value: '*',            label: 'All' },
  { value: 'webdesign',    label: 'Web Design' },
  { value: 'webapp',       label: 'Web Apps' },
  { value: 'mobiledesign', label: 'Mobile' },
  { value: 'gamedesign',   label: 'Game' },
];

export function PortfolioSection() {
  const [active, setActive] = useState('*');
  const { data, isLoading } = useProjects(active);
  const filtered = (data ?? []) as Project[];

  return (
    <section aria-labelledby="portfolio-heading" className={styles.wrap}>
      <div className={styles.header}>
        <div>
          <div className="section-eyebrow">My work</div>
          <h2 id="portfolio-heading" className="section-title">Featured projects</h2>
        </div>
        <p className={styles.count}>{filtered.length} project{filtered.length !== 1 ? 's' : ''}</p>
      </div>

      <div role="tablist" aria-label="Filter by category" className={styles.filterBar}>
        {FILTERS.map((f) => (
          <button
            key={f.value}
            role="tab"
            aria-selected={active === f.value}
            className={`${styles.filterBtn} ${active === f.value ? styles.filterActive : ''}`}
            onClick={() => setActive(f.value)}
          >
            {f.label}
          </button>
        ))}
      </div>

      <div
        className={styles.grid}
        role="tabpanel"
        aria-label={`${FILTERS.find((f) => f.value === active)?.label ?? 'All'} projects`}
      >
        {isLoading && (
          <p className={styles.empty}>Loading projects...</p>
        )}
        {!isLoading && filtered.map((p) => (
          <article key={p.id} className={`${styles.card} glass-card`}>
            <div className={styles.thumb}>
              <span className={styles.thumbEmoji} aria-hidden="true">
                {(p as typeof p & { emoji?: string }).emoji ?? '\uD83D\uDCBB'}
              </span>
              <div className={styles.overlay} aria-hidden="true" />
            </div>
            <div className={styles.body}>
              <h3 className={styles.cardTitle}>{p.title}</h3>
              <p className={styles.cardDesc}>{p.description}</p>
              <div className={styles.techs}>
                {p.technologies.map((t) => (
                  <span key={t} className={styles.tech}>{t}</span>
                ))}
              </div>
              {p.liveUrl && (
                <a
                  href={p.liveUrl}
                  target="_blank"
                  rel="noopener noreferrer"
                  className={styles.liveLink}
                >
                  View live →
                </a>
              )}
            </div>
          </article>
        ))}
        {!isLoading && filtered.length === 0 && (
          <p className={styles.empty}>No projects in this category yet.</p>
        )}
      </div>
    </section>
  );
}
