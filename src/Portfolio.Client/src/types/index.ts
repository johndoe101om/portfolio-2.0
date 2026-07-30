// ===================================================
// DOMAIN TYPES — Satyam Portfolio
// Mirror ASP.NET Core entity shapes
// ===================================================

export interface Profile {
  id: number;
  fullName: string;
  title: string;
  subtitle: string;
  aboutText: string;
  phone: string;
  email: string;
  website: string;
  city: string;
  country: string;
  age: number;
  degree: string;
  freelanceStatus: string;
  profileImageUrl: string;
  cvUrl: string;
  mapLat: number;
  mapLng: number;
}

export interface SocialLink {
  id: number;
  platform: string;
  url: string;
  iconClass: string;
  displayOrder: number;
  emoji?: string;
}

export interface Skill {
  id: number;
  name: string;
  percentage: number;
  category: 'technical' | 'language';
  languageLevel?: string;
  filledDots?: number;
  totalDots?: number;
  displayOrder: number;
}

export interface StatItem {
  id: number;
  iconClass: string;
  value: number;
  label: string;
  displayOrder: number;
}

export interface Service {
  id: number;
  title: string;
  description: string;
  iconClass: string;
  displayOrder: number;
}

export interface Education {
  id: number;
  institution: string;
  period: string;
  description: string;
  displayOrder: number;
}

export interface Experience {
  id: number;
  title: string;
  company?: string;
  period?: string;
  description: string;
  category: 'experience' | 'softskill';
  displayOrder: number;
}

export type ProjectCategory = 'webdesign' | 'mobiledesign' | 'webapp' | 'gamedesign';

export interface Project {
  id: number;
  slug: string;
  title: string;
  description: string;
  imageUrl: string;
  categories: ProjectCategory[];
  liveUrl?: string;
  technologies: string[];
  displayOrder: number;
  emoji?: string;
}

export interface BlogPost {
  id: number;
  slug: string;
  title: string;
  excerpt: string;
  content?: string;
  imageUrl: string;
  publishedAt: string;
  author: string;
  tags: string[];
  isPublished?: boolean;
}

export interface ContactMessage {
  name: string;
  email: string;
  subject: string;
  message: string;
}

export interface ContactResponse {
  success: boolean;
  message: string;
}

export interface SiteSetting {
  key: string;
  value: string;
}

export interface TestimonialItem {
  id: number;
  quote: string;
  authorName: string;
  authorTitle: string;
  authorImageUrl: string;
}

export interface KnowledgeArea {
  id: number;
  label: string;
}

// Sections available in the SPA navigation
export type SectionId =
  | 'hero'
  | 'about'
  | 'resume'
  | 'portfolio'
  | 'blog'
  | 'contact'
  | 'godmode';

export interface NavItem {
  id: SectionId;
  label: string;
  iconClass: string;
}
