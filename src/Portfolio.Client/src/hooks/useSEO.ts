import { useEffect } from 'react';
import type { SectionId } from '../types';

interface SEOConfig {
  title: string;
  description: string;
  canonical?: string;
}

const SITE = 'https://www.codersatyam.com';

const SEO_MAP: Record<SectionId, SEOConfig> = {
  hero: {
    title: 'Satyam Kumar Chaudhary | Full-Stack Developer & DevOps Engineer — Chennai, India',
    description:
      'Satyam Kumar Chaudhary (also known as Satyam Kumar or Satyam Chaudhary) is a full-stack web developer & DevOps engineer in Chennai, India. React, Node.js, Docker, cloud. Open for work.',
    canonical: SITE,
  },
  about: {
    title: 'About Satyam Kumar Chaudhary (Satyam Kumar / Satyam Chaudhary) | Developer — Chennai',
    description:
      'Learn about Satyam Kumar — a B.E. Computer Science graduate from AVIT Chennai with expertise in React, Node.js, DevOps, cloud engineering, and generative AI.',
    canonical: SITE + '/#about',
  },
  resume: {
    title: 'Resume & Services | Satyam Kumar — Web Developer Chennai',
    description:
      "View Satyam Kumar's education, experience, and professional services: web development, DevOps engineering, data visualisation, generative AI, and game development.",
    canonical: SITE + '/#resume',
  },
  portfolio: {
    title: 'Portfolio Projects | Satyam Kumar — React & Node.js Developer',
    description:
      "Explore Satyam Kumar's portfolio: Tutor Finder, CollegeLake, Online Signature, Skill Navigator and more. Built with React, Node.js, React Native, Unity and AWS.",
    canonical: SITE + '/#portfolio',
  },
  blog: {
    title: 'Blog | Satyam Kumar — Web Development & Career Articles',
    description:
      'Articles by Satyam Kumar on web design, coding logic, career growth, and technical interview preparation for software engineers.',
    canonical: SITE + '/#blog',
  },
  contact: {
    title: 'Hire Satyam Kumar Chaudhary (Satyam Kumar / Satyam) | Full-Stack Developer',
    description:
      'Hire Satyam Kumar Chaudhary (Satyam Kumar / Satyam Chaudhary) for web dev, DevOps, or cloud. Email: sirsatyamchaudhary@gmail.com | Chennai, India.',
    canonical: SITE + '/#contact',
  },
  godmode: {
    title: 'Godmode Admin | Satyam Kumar Portfolio',
    description: 'Private portfolio administration.',
    canonical: SITE + '/godmode',
  },
};

/**
 * Updates document title, meta description, and canonical link
 * whenever the active portfolio section changes.
 * Benefits crawlers that re-index on hash navigation.
 */
export function useSEO(section: SectionId) {
  useEffect(() => {
    const config = SEO_MAP[section] ?? SEO_MAP.hero;

    // Title
    document.title = config.title;

    // Meta description
    const descEl = document.querySelector<HTMLMetaElement>('meta[name="description"]');
    if (descEl) descEl.content = config.description;

    // Canonical
    if (config.canonical) {
      let canonEl = document.querySelector<HTMLLinkElement>('link[rel="canonical"]');
      if (!canonEl) {
        canonEl = document.createElement('link');
        canonEl.rel = 'canonical';
        document.head.appendChild(canonEl);
      }
      canonEl.href = config.canonical;
    }

    // OG tags
    const ogTitle = document.querySelector<HTMLMetaElement>('meta[property="og:title"]');
    const ogDesc  = document.querySelector<HTMLMetaElement>('meta[property="og:description"]');
    if (ogTitle) ogTitle.content = config.title;
    if (ogDesc)  ogDesc.content  = config.description;
  }, [section]);
}
