import { useEffect, useRef, useState } from 'react';
import styles from './CustomCursor.module.css';

/**
 * Magic cursor — disabled on touch devices and reduced-motion users.
 */
export function CustomCursor() {
  const innerRef = useRef<HTMLDivElement>(null);
  const outerRef = useRef<HTMLDivElement>(null);
  const [hovered, setHovered] = useState(false);
  const [visible, setVisible] = useState(false);

  const prefersReducedMotion =
    typeof window !== 'undefined' &&
    window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  const isTouch =
    typeof window !== 'undefined' &&
    ('ontouchstart' in window || navigator.maxTouchPoints > 0);

  useEffect(() => {
    if (prefersReducedMotion || isTouch) return;

    const handleMouseMove = (e: MouseEvent) => {
      setVisible(true);
      const x = e.clientX;
      const y = e.clientY;
      const transform = `translate(${x}px, ${y}px)`;
      if (innerRef.current) innerRef.current.style.transform = transform;
      if (outerRef.current) outerRef.current.style.transform = transform;
    };

    const handleMouseOver = (e: MouseEvent) => {
      const target = e.target as Element;
      if (target.closest('a, button, [data-cursor-hover]')) setHovered(true);
    };

    const handleMouseOut = (e: MouseEvent) => {
      const target = e.target as Element;
      if (target.closest('a, button, [data-cursor-hover]')) setHovered(false);
    };

    document.addEventListener('mousemove', handleMouseMove);
    document.addEventListener('mouseover', handleMouseOver);
    document.addEventListener('mouseout', handleMouseOut);

    return () => {
      document.removeEventListener('mousemove', handleMouseMove);
      document.removeEventListener('mouseover', handleMouseOver);
      document.removeEventListener('mouseout', handleMouseOut);
    };
  }, [prefersReducedMotion, isTouch]);

  if (prefersReducedMotion || isTouch) return null;

  const cursorClass = [
    styles.cursor,
    visible ? styles.visible : '',
    hovered ? styles.hovered : '',
  ].join(' ');

  return (
    <>
      <div ref={outerRef} className={`${styles.outer} ${cursorClass}`} aria-hidden="true" />
      <div ref={innerRef} className={`${styles.inner} ${cursorClass}`} aria-hidden="true" />
    </>
  );
}
