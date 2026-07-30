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
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    let rafId: number;
    let stars: Star[] = [];

    function init() {
      if (!canvas) return;
      canvas.width = window.innerWidth;
      canvas.height = window.innerHeight;
      stars = Array.from({ length: 150 }, () => ({
        x: Math.random() * canvas.width,
        y: Math.random() * canvas.height,
        r: Math.random() * 1.4 + 0.2,
        o: Math.random(),
        s: Math.random() * 0.005 + 0.001,
        d: Math.random() > 0.5 ? 1 : -1,
      }));
    }

    function draw() {
      if (!canvas || !ctx) return;
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      stars.forEach((s) => {
        s.o += s.s * s.d;
        if (s.o >= 1 || s.o <= 0) s.d *= -1;
        ctx.beginPath();
        ctx.arc(s.x, s.y, s.r, 0, Math.PI * 2);
        ctx.fillStyle = `rgba(0,212,255,${s.o * 0.55})`;
        ctx.fill();
      });
      rafId = requestAnimationFrame(draw);
    }

    init();
    draw();

    const onResize = () => init();
    window.addEventListener('resize', onResize);
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
