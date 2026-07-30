import { useInView } from '../../hooks/useInView';
import { useCounter } from '../../hooks/useCounter';
import styles from './SkillProgress.module.css';

interface Props { name: string; percentage: number; }

export function SkillProgress({ name, percentage }: Props) {
  const { ref, inView } = useInView(0.3);
  const count = useCounter(percentage, 2000, inView);
  return (
    <div className={styles.box} ref={ref as React.RefObject<HTMLDivElement>}>
      <div className={styles.header}>
        <span className={styles.name}>{name}</span>
        <span className={styles.pct} aria-live="polite">{count}%</span>
      </div>
      <div className={styles.track} role="progressbar" aria-valuenow={inView ? percentage : 0} aria-valuemin={0} aria-valuemax={100} aria-label={`${name}: ${percentage}%`}>
        <div className={styles.fill} style={{ width: inView ? `${percentage}%` : '0%' }} />
      </div>
    </div>
  );
}
