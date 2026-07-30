import { useState, useCallback } from 'react';
import type { SectionId } from '../types';

const SECTIONS: SectionId[] = ['hero', 'about', 'resume', 'portfolio', 'blog', 'contact'];
const FULL_WIDTH_SECTIONS: SectionId[] = ['hero', 'contact'];

/**
 * Manages section navigation state.
 * Replaces jQuery openMenu/closeMenu/sidebarMenu logic with pure React state.
 */
export function useSectionNavigation() {
  const [activeSection, setActiveSection] = useState<SectionId>('hero');

  const isMenuOpen = FULL_WIDTH_SECTIONS.includes(activeSection);

  const navigateTo = useCallback((section: SectionId) => {
    setActiveSection(section);
    // Update URL hash for bookmarkability / back-button support
    if (typeof window !== 'undefined') {
      window.history.pushState(null, '', `#${section}`);
    }
  }, []);

  const navigateNext = useCallback(() => {
    const idx = SECTIONS.indexOf(activeSection);
    const next = SECTIONS[(idx + 1) % SECTIONS.length];
    navigateTo(next);
  }, [activeSection, navigateTo]);

  const navigatePrev = useCallback(() => {
    const idx = SECTIONS.indexOf(activeSection);
    const prev = SECTIONS[(idx - 1 + SECTIONS.length) % SECTIONS.length];
    navigateTo(prev);
  }, [activeSection, navigateTo]);

  const isFirst = activeSection === SECTIONS[0];
  const isLast  = activeSection === SECTIONS[SECTIONS.length - 1];

  return {
    activeSection,
    isMenuOpen,
    navigateTo,
    navigateNext,
    navigatePrev,
    isFirst,
    isLast,
    sections: SECTIONS,
  };
}
