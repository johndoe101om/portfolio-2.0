import { useEffect, useRef } from 'react';

interface Star {
  x: number; y: number; r: number;
  o: number; s: number; d: number;
}

export function StarCanvas() {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;
    const ctx = canvas.getContext('2d', { alpha: true });
    if (!ctx) return;

    let rafId: number;
    let stars: Star[] = [];
    let lastTime = 0;

    const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    function init() {
      if (!canvas) return;
      const width = window.innerWidth;
      const height = window.innerHeight;
      canvas.width = width;
      canvas.height = height;

      const starCount = width < 768 ? 30 : 60;
      stars = Array.from({ length: starCount }, () => ({
        x: Math.random() * width,
        y: Math.random() * height,
        r: Math.random() * 1.2 + 0.3,
        o: Math.random(),
        s: Math.random() * 0.004 + 0.001,
        d: Math.random() > 0.5 ? 1 : -1,
      }));
    }

    function renderFrame() {
      if (!canvas || !ctx) return;
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      for (let i = 0; i < stars.length; i++) {
        const s = stars[i];
        if (!prefersReducedMotion) {
          s.o += s.s * s.d;
          if (s.o >= 1 || s.o <= 0) s.d *= -1;
        }
        ctx.beginPath();
        ctx.arc(s.x, s.y, s.r, 0, Math.PI * 2);
        ctx.fillStyle = `rgba(0,212,255,${s.o * 0.5})`;
        ctx.fill();
      }
    }

    function loop(timestamp: number) {
      if (document.hidden) {
        rafId = requestAnimationFrame(loop);
        return;
      }

      // Throttle to ~30 FPS (33ms interval)
      if (timestamp - lastTime >= 33) {
        lastTime = timestamp;
        renderFrame();
      }

      if (!prefersReducedMotion) {
        rafId = requestAnimationFrame(loop);
      }
    }

    init();
    renderFrame();

    if (!prefersReducedMotion) {
      rafId = requestAnimationFrame(loop);
    }

    const onResize = () => {
      init();
      renderFrame();
    };

    window.addEventListener('resize', onResize, { passive: true });
    return () => {
      cancelAnimationFrame(rafId);
      window.removeEventListener('resize', onResize);
    };
  }, []);

  return (
    <canvas
      ref={canvasRef}
      aria-hidden="true"
      style={{
        position: 'fixed', inset: 0,
        pointerEvents: 'none', zIndex: 0,
      }}
    />
  );
}
