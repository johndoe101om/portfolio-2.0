export interface ImageMetadata {
  width: number;
  height: number;
}

const IMAGE_METADATA: Record<string, ImageMetadata> = {
  '/assets/images/profile-lcp.jpg': { width: 512, height: 512 },
  '/assets/images/profile.jpg': { width: 512, height: 512 },
  '/assets/images/profile.png': { width: 512, height: 512 },
  '/assets/images/placeholder.png': { width: 400, height: 280 },
  '/assets/images/project-tutor-finder.png': { width: 892, height: 577 },
  '/assets/images/project-college-lake.png': { width: 448, height: 733 },
  '/assets/images/project-online-signature.png': { width: 1080, height: 886 },
  '/assets/images/project-skill-navigator-optimized.jpg': { width: 1200, height: 606 },
  '/assets/images/project-skill-navigator.png': { width: 1200, height: 607 },
  '/assets/images/project-game-optimized.jpg': { width: 1200, height: 612 },
  '/assets/images/project-game.png': { width: 1200, height: 612 },
  '/assets/images/project-portfolio-optimized.jpg': { width: 1200, height: 604 },
  '/assets/images/project-portfolio.png': { width: 1200, height: 604 },
};

export function getImageMetadata(src: string | undefined | null, fallback: ImageMetadata = { width: 1200, height: 800 }) {
  if (!src) return fallback;
  return IMAGE_METADATA[src] ?? fallback;
}

export function optimizedImageUrl(src: string | undefined | null) {
  switch (src) {
    case '/assets/images/profile.jpg':
    case '/assets/images/profile.png':
      return '/assets/images/profile-lcp.jpg';
    case '/assets/images/project-game.png':
      return '/assets/images/project-game-optimized.jpg';
    case '/assets/images/project-skill-navigator.png':
      return '/assets/images/project-skill-navigator-optimized.jpg';
    case '/assets/images/project-portfolio.png':
      return '/assets/images/project-portfolio-optimized.jpg';
    default:
      return src ?? '/assets/images/placeholder.png';
  }
}
