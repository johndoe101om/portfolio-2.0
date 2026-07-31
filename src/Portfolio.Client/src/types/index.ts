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

export type ProjectCategory = 'webdesign' | 'mobiledesign' | 'webapp' | 'gamedesign' | string;

export interface ProjectImage {
  id: number;
  storagePath: string;
  publicUrl: string;
  altText?: string;
  isThumbnail: boolean;
  displayOrder: number;
  width?: number;
  height?: number;
}

export interface ProjectLink {
  id: number;
  linkType: 'Live' | 'GitHub' | 'GitLab' | 'Bitbucket' | 'Documentation' | 'YouTube' | 'PlayStore' | 'AppStore' | 'Figma' | 'CaseStudy' | 'Blog' | string;
  url: string;
  label?: string;
}

export interface ProjectFeature {
  id: number;
  title: string;
  description?: string;
  iconClass?: string;
  displayOrder: number;
}

export interface ProjectAchievement {
  id: number;
  title: string;
  description?: string;
  dateAchieved?: string;
  displayOrder: number;
}

export interface Project {
  id: number;
  slug: string;
  title: string;
  description: string;
  shortDescription?: string;
  fullDescription?: string;
  imageUrl: string;
  thumbnailUrl?: string;
  categories: ProjectCategory[];
  technologies: string[];
  liveUrl?: string;
  displayOrder: number;
  emoji?: string;

  status?: 'Completed' | 'Planning' | 'In Progress' | 'Draft' | 'Archived' | string;
  visibility?: 'Public' | 'Private' | 'Unlisted' | string;
  isPublished?: boolean;
  isFeatured?: boolean;
  isDeleted?: boolean;
  resumeCategory?: string;
  experienceType?: string;
  startDate?: string;
  endDate?: string;
  isCurrentlyWorking?: boolean;
  durationText?: string;
  readmeMarkdown?: string;
  metaTitle?: string;
  metaDescription?: string;
  metaKeywords?: string;
  ogImageUrl?: string;
  createdAt?: string;
  updatedAt?: string;
  skills?: string[];
  images?: ProjectImage[];
  links?: ProjectLink[];
  features?: ProjectFeature[];
  achievements?: ProjectAchievement[];
}

export interface ProjectFilterParams {
  search?: string;
  category?: string;
  technology?: string;
  status?: string;
  year?: number;
  featured?: boolean;
  experienceType?: string;
  resumeCategory?: string;
  page?: number;
  pageSize?: number;
  sortBy?: 'Newest' | 'Oldest' | 'Alphabetical' | 'Updated' | 'Duration' | 'Featured' | 'Manual' | string;
  includeDeleted?: boolean;
  includeUnpublished?: boolean;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface ProjectDashboardStats {
  totalProjects: number;
  publishedProjects: number;
  draftProjects: number;
  featuredProjects: number;
  archivedProjects: number;
  totalTechnologies: number;
  totalCategories: number;
  recentProjects: Project[];
}

export interface ImageUploadResult {
  success: boolean;
  storagePath: string;
  publicUrl: string;
  message: string;
}

export interface AuditLog {
  id: number;
  entityName: string;
  entityId: string;
  action: string;
  performedBy: string;
  changesJson: string;
  timestamp: string;
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
