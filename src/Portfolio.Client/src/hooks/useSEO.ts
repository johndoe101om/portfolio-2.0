import { useEffect } from 'react';
import type { SectionId } from '../types';

interface SEOConfig {
  title: string;
  description: string;
}

const SITE = 'https://codrsatyam.netlify.app/';

const SEO_MAP: Record<SectionId, SEOConfig> = {
  hero: {
    title: 'Satyam Kumar Chaudhary | Full-Stack Developer & DevOps Engineer in Chennai',
    description:
      'Official portfolio of Satyam Kumar Chaudhary, also known as Satyam Kumar, Satyam Chaudhary, and codersatyam. Full-stack developer and DevOps engineer in Chennai, India.',
  },
  about: {
    title: 'About Satyam Kumar Chaudhary | Full-Stack Developer & DevOps Engineer',
    description:
      'Learn about Satyam Kumar Chaudhary, a Chennai-based full-stack developer and DevOps engineer skilled in React, ASP.NET Core, PostgreSQL, Docker, cloud, and CI/CD.',
  },
  resume: {
    title: 'Resume | Satyam Kumar Chaudhary, DevOps & Full-Stack Developer',
    description:
      'Education, services, and engineering background for Satyam Kumar Chaudhary, full-stack developer and DevOps engineer from Chennai, India.',
  },
  portfolio: {
    title: 'Projects | Satyam Kumar Chaudhary, React & ASP.NET Core Developer',
    description:
      'Selected projects by Satyam Kumar Chaudhary, including Tutor Finder, CollegeLake, Online Signature, Skill Navigator, and DevOps-ready portfolio work.',
  },
  blog: {
    title: 'Blog | Satyam Kumar Chaudhary on Web Development and Career Growth',
    description:
      'Articles by Satyam Kumar Chaudhary on web design, coding logic, technical interviews, software engineering, and professional growth.',
  },
  contact: {
    title: 'Contact Satyam Kumar Chaudhary | Hire Full-Stack Developer in Chennai',
    description:
      'Contact Satyam Kumar Chaudhary, also known as codersatyam, for full-stack web development, DevOps, cloud, and software engineering work.',
  },
  godmode: {
    title: 'Admin | Satyam Kumar Portfolio',
    description: 'Private portfolio administration.',
  },
};

function upsertMeta(selector: string, attrs: Record<string, string>) {
  let element = document.querySelector<HTMLMetaElement>(selector);
  if (!element) {
    element = document.createElement('meta');
    document.head.appendChild(element);
  }

  Object.entries(attrs).forEach(([key, value]) => {
    element?.setAttribute(key, value);
  });
}

function ensureSingleCanonical() {
  const canonicals = Array.from(document.querySelectorAll<HTMLLinkElement>('link[rel="canonical"]'));
  const canonical = canonicals[0] ?? document.createElement('link');
  canonical.rel = 'canonical';
  canonical.href = SITE;
  if (!canonical.parentNode) document.head.appendChild(canonical);
  canonicals.slice(1).forEach((node) => node.remove());
}

function ensureHreflang() {
  const alternates = Array.from(document.querySelectorAll<HTMLLinkElement>('link[rel="alternate"][hreflang="en-IN"]'));
  const alternate = alternates[0] ?? document.createElement('link');
  alternate.rel = 'alternate';
  alternate.hreflang = 'en-IN';
  alternate.href = SITE;
  if (!alternate.parentNode) document.head.appendChild(alternate);
  alternates.slice(1).forEach((node) => node.remove());
}

export function useSEO(section: SectionId) {
  useEffect(() => {
    const config = SEO_MAP[section] ?? SEO_MAP.hero;

    document.title = config.title;

    upsertMeta('meta[name="description"]', {
      name: 'description',
      content: config.description,
    });

    upsertMeta('meta[property="og:title"]', {
      property: 'og:title',
      content: config.title,
    });

    upsertMeta('meta[property="og:description"]', {
      property: 'og:description',
      content: config.description,
    });

    upsertMeta('meta[name="twitter:title"]', {
      name: 'twitter:title',
      content: config.title,
    });

    upsertMeta('meta[name="twitter:description"]', {
      name: 'twitter:description',
      content: config.description,
    });

    ensureSingleCanonical();
    ensureHreflang();
  }, [section]);
}
