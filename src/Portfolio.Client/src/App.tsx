import { useEffect, useState, Suspense, lazy } from 'react';
import { StarCanvas } from './components/ui/StarCanvas';
import { Sidebar } from './components/layout/Sidebar';
import { LoadingScreen } from './components/ui/LoadingScreen';
import { ErrorBoundary } from './components/ui/ErrorBoundary';
import { MobileNavigation } from './components/layout/MobileNavigation';
import type { SectionId } from './types';
import { useSEO } from './hooks/useSEO';
import { HeroSection } from './components/sections/HeroSection';
import { AboutSection } from './components/sections/AboutSection';
import { ResumeSection } from './components/sections/ResumeSection';
import { PortfolioSection } from './components/sections/PortfolioSection';
import { BlogSection } from './components/sections/BlogSection';
import { ContactSection } from './components/sections/ContactSection';
import { NAV_ITEMS } from './api/staticData';
import { useProfile, useSocialLinks } from './api/queries';
import styles from './App.module.css';

const AdminPanel = lazy(() => import('./components/admin/AdminPanel').then((m) => ({ default: m.AdminPanel })));

// SectionId is imported from types

const SECTIONS: SectionId[] = ['hero', 'about', 'resume', 'portfolio', 'blog', 'contact'];
const RENDERED_SECTIONS: SectionId[] = [...SECTIONS, 'godmode'];

function sectionFromUrl(): SectionId {
  if (typeof window === 'undefined') return 'hero';

  if (window.location.pathname.replace(/\/$/, '') === '/godmode') {
    return 'godmode';
  }

  const hash = window.location.hash.replace('#', '') as SectionId;
  return SECTIONS.includes(hash) ? hash : 'hero';
}

function sectionUrl(section: SectionId) {
  if (section === 'godmode') return '/godmode';
  return section === 'hero' ? '/' : `/#${section}`;
}

export default function App() {
  const [active, setActive] = useState<SectionId>(() => sectionFromUrl());
  const [mobileOpen, setMobileOpen] = useState(false);
  useSEO(active);
  const { data: profile } = useProfile();
  const { data: socialLinks = [] } = useSocialLinks();

  const nav = (s: SectionId) => {
    setActive(s);
    setMobileOpen(false);
    if (typeof window !== 'undefined') window.history.pushState(null, '', sectionUrl(s));
  };

  const navPrev = () => {
    const idx = SECTIONS.indexOf(active);
    nav(SECTIONS[(idx - 1 + SECTIONS.length) % SECTIONS.length]);
  };
  const navNext = () => {
    const idx = SECTIONS.indexOf(active);
    nav(SECTIONS[(idx + 1) % SECTIONS.length]);
  };

  useEffect(() => {
    const handlePopState = () => setActive(sectionFromUrl());
    window.addEventListener('popstate', handlePopState);
    return () => window.removeEventListener('popstate', handlePopState);
  }, []);

  return (
    <ErrorBoundary>
      <a className="skip-link" href="#main-content">Skip to main content</a>
      <LoadingScreen />
      <StarCanvas />

      <div className={styles.shell}>
        {/* Desktop sidebar */}
        <Sidebar active={active} onNavigate={nav} />

        {/* Mobile nav */}
        <MobileNavigation
          isOpen={mobileOpen}
          activeSection={active as Parameters<typeof nav>[0]}
          navItems={NAV_ITEMS}
          profileName={profile?.fullName ?? 'Satyam Kumar Chaudhary'}
          profileImageUrl={profile?.profileImageUrl ?? '/assets/images/profile-lcp.jpg'}
          cvUrl={profile?.cvUrl ?? '#'}
          onToggle={() => setMobileOpen((o) => !o)}
          onNavigate={nav}
          onPrev={navPrev}
          onNext={navNext}
        />

        {/* Main content */}
        <main id="main-content" className={styles.main} role="main">
          <Suspense fallback={<div className={styles.loading}>Loading…</div>}>
            {RENDERED_SECTIONS.map((id) => (
              <div
                key={id}
                className={`${styles.section} ${active === id ? styles.active : ''}`}
                aria-hidden={active !== id}
                id={`section-${id}`}
              >
                {id === 'hero'      && <HeroSection onNavigate={nav} />}
                {id === 'about'     && <AboutSection />}
                {id === 'resume'    && <ResumeSection />}
                {id === 'portfolio' && <PortfolioSection />}
                {id === 'blog'      && <BlogSection />}
                {id === 'contact'   && <ContactSection />}
                {id === 'godmode' && active === 'godmode' && <AdminPanel />}
              </div>
            ))}
          </Suspense>
        </main>
      </div>
    </ErrorBoundary>
  );
}
