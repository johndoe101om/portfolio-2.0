import { useInView } from '../../hooks/useInView';
import { useCounter } from '../../hooks/useCounter';
import styles from './StatCounter.module.css';
import type { StatItem } from '../../types';

interface Props {
  item: StatItem;
}

export function StatCounter({ item }: Props) {
  const { ref, inView } = useInView(0.3);
  const count = useCounter(item.value, 5000, inView);

  return (
    <div
      className={styles.statItem}
      ref={ref as React.RefObject<HTMLDivElement>}
    >
      <div className={styles.icon}>
        <i className={item.iconClass} aria-hidden="true" />
      </div>
      <span
        className={styles.value}
        aria-live="polite"
        aria-label={`${count} ${item.label}`}
      >
        {count}
      </span>
      <p className={styles.label}>{item.label}</p>
    </div>
  );
}
