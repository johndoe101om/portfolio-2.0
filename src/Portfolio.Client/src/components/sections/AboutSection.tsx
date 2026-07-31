import { useRef } from 'react';
import styles from './AboutSection.module.css';
import { useProfile, useSkills, useStats, useKnowledgeAreas } from '../../api/queries';
import { useInView } from '../../hooks/useInView';
import { useCounter } from '../../hooks/useCounter';
import { SkillProgress } from '../ui/SkillProgress';
import type { Skill, StatItem } from '../../types';

function Counter({ item }: { item: StatItem }) {
  const { ref, inView } = useInView(0.3);
  const count = useCounter(item.value, 2000, inView);
  return (
    <div className={styles.statBox} ref={ref as React.RefObject<HTMLDivElement>}>
      <span className={styles.statVal} aria-live="polite">{count}</span>
      <span className={styles.statLbl}>{item.label}</span>
    </div>
  );
}

export function AboutSection() {
  const { data: profile } = useProfile();
  const { data: skills = [] } = useSkills();
  const { data: stats = [] } = useStats();
  const { data: knowledge = [] } = useKnowledgeAreas();

  const technical = (skills as Skill[]).filter((s) => s.category === 'technical');

  return (
    <section aria-labelledby="about-heading" className={styles.wrap}>
      <div className={styles.header}>
        <div className="section-eyebrow">Who I am</div>
        <h2 id="about-heading" className="section-title">A little about me</h2>
      </div>

      <div className={styles.bio}>
        <div className={styles.avatarWrap}>
          <div className={styles.avatarRing} aria-hidden="true" />
          <div className={styles.avatar}>
            <img
              src={profile?.profileImageUrl ?? '/assets/images/profile.jpg'}
              alt={profile?.fullName ?? 'Satyam Kumar'}
              className={styles.avatarImg}
            />
          </div>
        </div>
        <div className={styles.bioContent}>
          <p className={styles.bioText}>{profile?.aboutText}</p>
          <div className={styles.infoGrid}>
            <div className={styles.infoRow}><span className={styles.infoKey}>City</span><span className={styles.infoVal}>{profile?.city}, {profile?.country}</span></div>
            <div className={styles.infoRow}><span className={styles.infoKey}>Age</span><span className={styles.infoVal}>{profile?.age}</span></div>
            <div className={styles.infoRow}><span className={styles.infoKey}>Degree</span><span className={styles.infoVal}>{profile?.degree}</span></div>
            <div className={styles.infoRow}><span className={styles.infoKey}>Status</span><span className={styles.infoVal} style={{color:'#4ade80'}}>● {profile?.freelanceStatus}</span></div>
            <div className={styles.infoRow}><span className={styles.infoKey}>Email</span><span className={styles.infoVal} style={{color:'var(--cyan)'}}>{profile?.email}</span></div>
            <div className={styles.infoRow}><span className={styles.infoKey}>Website</span><span className={styles.infoVal}>{profile?.website}</span></div>
          </div>
          <a href={profile?.cvUrl ?? '#'} target="_blank" rel="noopener noreferrer" className="btn-primary" style={{display:'inline-flex'}}>
            Download CV
          </a>
        </div>
      </div>

      <div className={styles.stats} aria-label="Statistics">
        {(stats as StatItem[]).map((s) => <Counter key={s.id} item={s} />)}
      </div>

      <div className={styles.skillsSection}>
        <h3 className={styles.skillsTitle}>Technical skills</h3>
        <div className={styles.skillsList}>
          {technical.map((s) => <SkillProgress key={s.id} name={s.name} percentage={s.percentage} />)}
        </div>
        {knowledge.length > 0 && (
          <div className={styles.knowledge}>
            {knowledge.map((k) => <span key={k.id} className={styles.knowledgeTag}>{k.label}</span>)}
          </div>
        )}
      </div>
    </section>
  );
}
