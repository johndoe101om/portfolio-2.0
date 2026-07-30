# Component Mapping

Maps every original HTML structure and jQuery behaviour to its React component equivalent.

---

## Layout Components

| Original Structure | React Component | File |
|-------------------|-----------------|------|
| `<aside class="left-side">` | `DesktopSidebar` | `layout/DesktopSidebar.tsx` |
| `<aside class="right-side">` | `RightUtilityRail` | `layout/RightUtilityRail.tsx` |
| `<main id="main">` wrapper | `AppShell` | `layout/AppShell.tsx` |
| Mobile hamburger + overlay | `MobileNavigation` | `layout/MobileNavigation.tsx` |
| Section switching logic | `useSectionNavigation` | `hooks/useSectionNavigation.ts` |
| `<div class="preloader">` | `LoadingScreen` | `ui/LoadingScreen.tsx` |
| `.magic-cursor` divs | `CustomCursor` | `ui/CustomCursor.tsx` |
| `<ul class="circles">` | Inline in `AppShell` | CSS in `global.css` |

---

## Section Components

| Original `#section` | React Component | Key Sub-components |
|--------------------|-----------------|-------------------|
| `#hero` | `HeroSection` | `AnimatedRole` (inline state) |
| `#about` | `AboutSection` | `StatCounter`, `SkillProgress`, `LanguageSkill`, `TestimonialCarousel` |
| `#resume` | `ResumeSection` | Services grid, Education timeline, Soft skills |
| `#portfolio` | `PortfolioSection` | Filter tabs, Project card grid |
| `#blog` | `BlogSection` | Blog card list |
| `#contact` | `ContactSection` | `ContactForm`, info boxes |

---

## UI Components

| jQuery Plugin / Widget | React Component | Approach |
|-----------------------|-----------------|----------|
| `typed.js` rotating text | Inline in `HeroSection` | `useState` + `setInterval` |
| `jquery.countTo` numbers | `StatCounter` | `useCounter` + `requestAnimationFrame` |
| Waypoints scroll trigger | `useInView` | `IntersectionObserver` |
| Skill bar animation | `SkillProgress` | CSS `transition: width` triggered by `useInView` |
| Language dot bars | `LanguageSkill` | Pure JSX + Bootstrap Icons circles |
| `owl.carousel` testimonials | `TestimonialCarousel` | `useState` index + dot nav |
| `isotope.pkgd` filtering | `PortfolioSection` | `Array.filter` on `activeFilter` state |
| `magnific-popup` lightbox | Hover overlay in `PortfolioSection` | CSS overlay (expandable to modal) |
| EmailJS form | `ContactForm` | `react-hook-form` + Zod + Axios → backend |
| Custom cursor | `CustomCursor` | `addEventListener` + `useRef` transforms |
| OWL carousel controls | `TestimonialCarousel` dots | `useState` |

---

## Prop Flow

```
AppShell
  ├── useSectionNavigation()          → activeSection, isMenuOpen, navigate*, prev, next
  ├── useProfile()                    → profile data
  ├── useSocialLinks()                → social links
  │
  ├── DesktopSidebar
  │     ├── isOpen: boolean
  │     ├── activeSection: SectionId
  │     ├── navItems: NavItem[]
  │     ├── profileName: string
  │     ├── profileImageUrl: string
  │     ├── cvUrl: string
  │     └── onNavigate(section): void
  │
  ├── RightUtilityRail
  │     ├── socialLinks: SocialLink[]
  │     ├── onPrev(): void
  │     └── onNext(): void
  │
  ├── MobileNavigation
  │     ├── isOpen: boolean
  │     ├── activeSection: SectionId
  │     ├── navItems: NavItem[]
  │     ├── onToggle(): void
  │     ├── onNavigate(section): void
  │     ├── onPrev(): void
  │     └── onNext(): void
  │
  └── <SectionWrapper id active>
        ├── HeroSection { onNavigate }
        ├── AboutSection {}
        ├── ResumeSection {}
        ├── PortfolioSection {}
        ├── BlogSection {}
        └── ContactSection {}
```

---

## Data Flow

```
Static seed data (api/staticData.ts)
  ↓ (when VITE_API_BASE_URL is unset)
TanStack Query hooks (api/queries.ts)
  ↓
React components

ASP.NET Core API (/api/*)
  ↑ (when VITE_API_BASE_URL is set)
TanStack Query hooks (api/queries.ts)
  ↓
React components
```

The `staticData.ts` file mirrors the exact same seed data as `PortfolioDbContext.SeedData()`,
ensuring visual parity whether the API is running or not.

---

## CSS Module Strategy

Every component has a co-located `.module.css` file. All CSS uses:
- `var(--token-name)` for colours, spacing, typography, and transitions
- No hardcoded colour values
- No `!important` except in utility overrides
- `@media (prefers-reduced-motion: reduce)` guard on all animations

Global CSS (`styles/global.css`) contains:
- Base reset
- Skip link
- Utility classes (`.base-color`, `.subtitle`, `.primary-button`, `.secondary-button`)
- Background circles animation
- `.sr-only` screen reader utility
