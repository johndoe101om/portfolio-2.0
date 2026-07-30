import { useEffect, useState } from 'react';
import styles from './LoadingScreen.module.css';

export function LoadingScreen() {
  const [removing, setRemoving] = useState(false);
  const [removed, setRemoved] = useState(false);

  useEffect(() => {
    const timer1 = setTimeout(() => setRemoving(true), 800);
    const timer2 = setTimeout(() => setRemoved(true), 1400);
    return () => { clearTimeout(timer1); clearTimeout(timer2); };
  }, []);

  if (removed) return null;

  return (
    <div
      className={`${styles.loader} ${removing ? styles.preloaded : ''}`}
      aria-hidden="true"
      role="presentation"
    >
      <div className={styles.middleLine} />
    </div>
  );
}
